using NAudio.Wave;

namespace Drums4me.Classes;

/// <summary>Klangdaten und Mischwerte einer Spur für die Engine. Die Tonhöhe steckt schon in den Klangdaten.</summary>
internal sealed record EngineTrack(float[] Samples, float GainLeft, float GainRight, HatRole Hat = HatRole.None, bool Muted = false);

/// <summary>Unveränderlicher Stand, den die Oberfläche als Ganzes an den Audio-Thread übergibt.</summary>
internal sealed record EngineState(double Bpm, SwingFeel Swing, float Volume, StepLevel[][][] Patterns, int ActivePattern, int[] Sequence, bool RepeatSequence, int CountInBars, Meter[] Meters, EngineTrack[] Tracks, int TrainerStepBpm, int TrainerEveryBars, int TrainerTargetBpm, float ClickGain = 1f)
{
    public static EngineState From(Project project, DrumKit kit)
    {
        var tracks = new EngineTrack[Project.TrackCount];
        for (var track = 0; track < Project.TrackCount; track++)
        {
            var settings = project.Tracks[track];
            var angle = (settings.Pan + 1f) * MathF.PI / 4f;   // gleichbleibende Lautheit über das Panorama
            tracks[track] = new EngineTrack(
                kit.SamplesOf(track, settings.Pitch),
                settings.Volume * MathF.Cos(angle),
                settings.Volume * MathF.Sin(angle),
                kit.HatRoleOf(track),
                settings.Muted);
        }

        var patterns = new StepLevel[Project.PatternCount][][];
        for (var pattern = 0; pattern < Project.PatternCount; pattern++)
        {
            patterns[pattern] = new StepLevel[Project.TrackCount][];
            for (var track = 0; track < Project.TrackCount; track++)
            {
                patterns[pattern][track] = (StepLevel[])project.Patterns[pattern].Steps[track].Clone();
            }
        }

        return new EngineState(project.Bpm, project.Swing, project.VolumePercent / 100f,
            patterns, project.ActivePattern, project.ActiveSequence, project.SequenceRepeat, project.CountInBars, [.. project.Patterns.Select(pattern => pattern.Meter)], tracks,
            project.TrainerEnabled ? project.TrainerStepBpm : 0, project.TrainerEveryBars, project.TrainerTargetBpm);
    }
}

/// <summary>
/// Taktgeber und Mischer. Das Tempo ergibt sich allein aus dem Samplezähler im Audiostrom und ist damit unabhängig
/// von der Rechnerlast. Read läuft im Audio-Thread und darf deshalb weder blockieren noch Speicher anfordern.
/// </summary>
internal sealed class SequencerEngine(EngineState state) : ISampleProvider
{
    private const int MaxVoices = 32;
    private const int RewindToStart = 1;
    private const int RewindToSilence = 2;

    // Ein leiser Schlag (Ghost Note) liegt etwa 10 dB unter dem normalen, ein betonter etwa 3 dB darüber.
    private const float SoftGain = 0.3f;
    private const float AccentGain = 1.4f;

    // So schnell verklingt eine offene Hi-Hat, wenn ein anderer Hi-Hat-Schlag sie stoppt (15 ms). Ohne Ausblenden knackte es.
    private const float ChokeFadeStep = 1f / (DrumKit.SampleRate * 15 / 1000);

    // Stille nach dem letzten Sound, damit auch der Rest im Ausgabepuffer noch zu hören ist.
    private const int TrailingSilenceSamples = DrumKit.SampleRate / 5;

    // Beim Vorzählen klingt die Eins am höchsten und lautesten, der Anfang einer Gruppe wie 3+2 etwas schwächer.
    private static readonly EngineTrack AccentClick = CreateClick(1600f, 0.9f);
    private static readonly EngineTrack GroupClick = CreateClick(1300f, 0.75f);
    private static readonly EngineTrack BeatClick = CreateClick(1000f, 0.6f);

    private readonly Voice[] _voices = new Voice[MaxVoices];
    private volatile EngineState _state = state;
    private volatile int _currentStep = -1;
    private volatile int _currentPattern = -1;
    private volatile int _currentSequenceIndex = -1;
    private volatile bool _isFinished;
    private int _rewindPending;
    private bool _sequenceEnded;
    private int _trailingSamples;

    private long _samplePos;
    private double _nextStepPos;
    private int _nextStep;
    private int _playingPattern;
    private int _sequenceIndex = -1;
    private long _lastBarStart = -1;
    private int _countInStepsLeft;
    private Meter _countInMeter = Meter.FourFour;
    private volatile int _countInBar;
    private int _trainerBars;
    private int _trainerOffset;
    private volatile int _currentBpm;
    private EngineTrack? _pendingPreview;
    private volatile int _barsPlayed;
    private bool _silentBar;
    private volatile bool _isSilentBar;
    private long _barStart = -1;
    private long _endPosition = -1;

    public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(DrumKit.SampleRate, 2);

    /// <summary>Der zuletzt ausgelöste Schritt, -1 vor dem ersten Schritt.</summary>
    public int CurrentStep => _currentStep;

    /// <summary>Das zuletzt gespielte Pattern, -1 vor dem ersten Schritt.</summary>
    public int CurrentPattern => _currentPattern;

    /// <summary>Die Stelle des gerade gespielten Takts in der Abfolge ab 0, -1 ohne Abfolge und vor dem ersten Schritt.</summary>
    public int CurrentSequenceIndex => _currentSequenceIndex;

    /// <summary>Die Position des letzten Taktanfangs im Audiostrom in Samples, -1 vor dem ersten Takt.</summary>
    public long LastBarStart => Interlocked.Read(ref _lastBarStart);

    /// <summary>Der gerade vorgezählte Takt ab 1, 0 außerhalb des Vorzählens.</summary>
    public int CountInBar => _countInBar;

    /// <summary>Das gerade gespielte Tempo, beim Tempotrainer also samt seiner Erhöhung.</summary>
    public int CurrentBpm => _currentBpm;

    /// <summary>Der gerade gespielte Takt seit dem Start, 0 vor dem ersten Takt. Vorzählen zählt nicht mit.</summary>
    public int BarsPlayed => _barsPlayed;

    /// <summary>Gerade läuft ein stummer Takt der Abfolge.</summary>
    public bool IsSilentBar => _isSilentBar;

    /// <summary>Wo der laufende Takt im Audiostrom begonnen hat, in Samples. Anders als <see cref="LastBarStart"/> auch für stumme Takte. -1 vor dem ersten Takt.</summary>
    public long CurrentBarStart => Interlocked.Read(ref _barStart);

    /// <summary>Wo eine Abfolge ohne Wiederholung geendet hat, in Samples: die Taktgrenze nach ihrem letzten Takt. -1, solange sie läuft.</summary>
    public long EndPosition => Interlocked.Read(ref _endPosition);

    /// <summary>Die Abfolge ohne Wiederholung ist zu Ende, und alle Sounds sind ausgeklungen.</summary>
    public bool IsFinished => _isFinished;

    public void Update(EngineState state) => _state = state;

    /// <summary>Setzt die Wiedergabe beim nächsten Read auf den Anfang zurück und beendet klingende Sounds.</summary>
    public void Rewind()
    {
        Interlocked.Exchange(ref _pendingPreview, null);
        RequestReset(RewindToStart);
    }

    /// <summary>Spielt einen einzelnen Klang, ohne dass die Patterns laufen. IsFinished meldet das Ende.</summary>
    public void BeginPreview(EngineTrack track)
    {
        Interlocked.Exchange(ref _pendingPreview, track);
        RequestReset(RewindToSilence);
    }

    /// <summary>Spielt einen einzelnen Klang zusätzlich zu dem, was gerade läuft.</summary>
    public void Preview(EngineTrack track) => Interlocked.Exchange(ref _pendingPreview, track);

    private void RequestReset(int kind)
    {
        _currentStep = -1;
        _currentPattern = -1;
        _currentSequenceIndex = -1;
        _isFinished = false;
        _isSilentBar = false;
        _countInBar = 0;
        _barsPlayed = 0;
        Interlocked.Exchange(ref _lastBarStart, -1);
        Interlocked.Exchange(ref _rewindPending, kind);
    }

    public int Read(Span<float> buffer)
    {
        var state = _state;
        var reset = Interlocked.Exchange(ref _rewindPending, 0);
        if (reset != 0)
        {
            ResetTransport(state);
            _sequenceEnded = reset == RewindToSilence;
        }

        if (Interlocked.Exchange(ref _pendingPreview, null) is { } preview)
        {
            StartVoice(preview);
            _trailingSamples = 0;
            _isFinished = false;
        }

        var frames = buffer.Length / 2;
        for (var frame = 0; frame < frames; frame++)
        {
            if (!_sequenceEnded && _samplePos >= _nextStepPos)
            {
                TriggerStep(state);
            }
            MixFrame(state.Volume, buffer.Slice(frame * 2, 2));
            _samplePos++;
        }

        if (_sequenceEnded && !AnyVoiceActive())
        {
            _trailingSamples += frames;
            _isFinished = _trailingSamples >= TrailingSilenceSamples;
        }
        return frames * 2;
    }

    private bool AnyVoiceActive()
    {
        for (var slot = 0; slot < MaxVoices; slot++)
        {
            if (_voices[slot].Samples is not null)
            {
                return true;
            }
        }
        return false;
    }

    private void ResetTransport(EngineState state)
    {
        // Vorgezählt wird in der Taktart des Patterns, mit dem die Wiedergabe beginnt.
        _playingPattern = FirstPattern(state);
        _silentBar = false;
        _isSilentBar = false;
        _countInMeter = state.Meters[_playingPattern];
        _countInStepsLeft = state.CountInBars * _countInMeter.Steps;
        _countInBar = 0;
        _samplePos = 0;
        _nextStepPos = 0;
        _nextStep = 0;
        _currentStep = -1;
        _currentPattern = -1;
        _currentSequenceIndex = -1;
        _sequenceIndex = -1;
        _sequenceEnded = false;
        _trailingSamples = 0;
        _isFinished = false;
        _trainerBars = 0;
        _trainerOffset = 0;
        _barsPlayed = 0;
        Interlocked.Exchange(ref _lastBarStart, -1);
        Interlocked.Exchange(ref _barStart, -1);
        Interlocked.Exchange(ref _endPosition, -1);
        Array.Clear(_voices);
    }

    /// <summary>Das Pattern, mit dem die Wiedergabe beginnt: in einer Abfolge das erste, das kein stummer Takt ist.</summary>
    private static int FirstPattern(EngineState state)
    {
        foreach (var entry in state.Sequence)
        {
            if (entry != Project.SilentBar)
            {
                return entry;
            }
        }
        return state.ActivePattern;
    }

    /// <summary>
    /// Ohne Abfolge gilt das aktive Pattern sofort, mit Abfolge wechselt das Pattern nur am Taktanfang.
    /// Liefert false, wenn die Abfolge ohne Wiederholung ihren letzten Takt beendet hat.
    /// </summary>
    private bool SelectPattern(EngineState state, int step)
    {
        var sequence = state.Sequence;
        if (sequence.Length == 0)
        {
            _sequenceIndex = -1;
            _silentBar = false;

            // Bei einer anderen Taktart wartet der Wechsel auf den Taktanfang, damit kein Takt abreißt.
            if (step == 0 || ReferenceEquals(state.Meters[state.ActivePattern], state.Meters[_playingPattern]))
            {
                _playingPattern = state.ActivePattern;
            }
        }
        else if (step == 0)
        {
            if (!state.RepeatSequence && _sequenceIndex >= sequence.Length - 1)
            {
                return false;
            }
            _sequenceIndex = (_sequenceIndex + 1) % sequence.Length;

            // Ein stummer Takt hat kein eigenes Pattern. Er dauert so lange wie ein Takt des Patterns davor.
            var entry = sequence[_sequenceIndex];
            _silentBar = entry == Project.SilentBar;
            if (!_silentBar)
            {
                _playingPattern = entry;
            }
        }
        return true;
    }

    /// <summary>Ein Schritt des Vorzählens: Klick auf jeder Zählzeit, die Patterns bleiben still.</summary>
    private void TriggerCountInStep(EngineState state)
    {
        var step = _nextStep;
        if (step == 0)
        {
            Interlocked.Exchange(ref _lastBarStart, _samplePos);
            _countInBar++;
        }

        switch (_countInMeter.ClickLevels[step])
        {
            case Meter.ClickBar:
                StartVoice(AccentClick, state.ClickGain);
                break;
            case Meter.ClickGroup:
                StartVoice(GroupClick, state.ClickGain);
                break;
            case Meter.ClickBeat:
                StartVoice(BeatClick, state.ClickGain);
                break;
        }

        _countInStepsLeft--;
        _nextStepPos += StepLength(state, _countInMeter, step);
        _nextStep = (step + 1) % _countInMeter.Steps;
    }

    /// <summary>Der Klick auf die Eins in der eingestellten Lautstärke, zum Anhören beim Einstellen.</summary>
    public static EngineTrack CountInClick(float gain) => AccentClick with { GainLeft = AccentClick.GainLeft * gain, GainRight = AccentClick.GainRight * gain };

    private static EngineTrack CreateClick(float frequency, float level)
    {
        var samples = new float[DrumKit.SampleRate * 3 / 100];
        for (var index = 0; index < samples.Length; index++)
        {
            var time = index / (float)DrumKit.SampleRate;
            samples[index] = level * MathF.Sin(2f * MathF.PI * frequency * time) * MathF.Exp(-time / 0.008f);
        }

        var centered = MathF.Cos(MathF.PI / 4f);
        return new EngineTrack(samples, centered, centered);
    }

    private void TriggerStep(EngineState state)
    {
        if (_countInStepsLeft > 0)
        {
            TriggerCountInStep(state);
            return;
        }

        _countInBar = 0;
        var step = _nextStep;
        if (step >= state.Meters[_playingPattern].Steps)
        {
            step = 0;   // die Taktart wurde während der Wiedergabe verkürzt
        }

        if (!SelectPattern(state, step))
        {
            _sequenceEnded = true;
            Interlocked.Exchange(ref _endPosition, _samplePos);
            _currentStep = -1;
            return;
        }

        // Im stummen Takt läuft nur die Zeit weiter: kein Klang, kein Lauflicht und kein Taktblitz, an denen man sich
        // festhalten könnte. Gezählt wird er wie jeder andere Takt, auch für den Tempotrainer.
        if (step == 0)
        {
            Interlocked.Exchange(ref _barStart, _samplePos);
            if (!_silentBar)
            {
                Interlocked.Exchange(ref _lastBarStart, _samplePos);
            }
            _barsPlayed++;
            AdvanceTrainer(state);
        }

        if (!_silentBar)
        {
            var steps = state.Patterns[_playingPattern];
            for (var track = 0; track < Project.TrackCount; track++)
            {
                var level = steps[track][step];
                if (level != StepLevel.Off && !state.Tracks[track].Muted)   // stumm heißt auch: stoppt keine offene Hi-Hat
                {
                    StartVoice(state.Tracks[track], level == StepLevel.Soft ? SoftGain : level == StepLevel.Accent ? AccentGain : 1f);
                }
            }
        }

        _currentStep = _silentBar ? -1 : step;
        _isSilentBar = _silentBar;
        _currentPattern = _playingPattern;
        _currentSequenceIndex = _sequenceIndex;
        _nextStepPos += StepLength(state, state.Meters[_playingPattern], step);
        _nextStep = (step + 1) % state.Meters[_playingPattern].Steps;
    }

    /// <summary>Am Taktanfang: Nach der eingestellten Zahl gespielter Takte steigt das Tempo, bis das Ziel erreicht ist.</summary>
    private void AdvanceTrainer(EngineState state)
    {
        if (state.TrainerStepBpm == 0)
        {
            _trainerBars = 0;
            _trainerOffset = 0;
            return;
        }

        if (_trainerBars >= state.TrainerEveryBars)
        {
            _trainerBars = 0;
            if (state.Bpm + _trainerOffset < state.TrainerTargetBpm)
            {
                _trainerOffset += state.TrainerStepBpm;
            }
        }
        _trainerBars++;
    }

    /// <summary>Der Trainer überschreitet das Ziel nicht und senkt das Tempo nie unter den eingestellten Wert.</summary>
    private double EffectiveBpm(EngineState state) =>
        state.TrainerStepBpm == 0 ? state.Bpm : Math.Max(state.Bpm, Math.Min(state.Bpm + _trainerOffset, state.TrainerTargetBpm));

    private double StepLength(EngineState state, Meter meter, int step)
    {
        var bpm = EffectiveBpm(state);
        _currentBpm = (int)bpm;
        var sixteenth = 60.0 * DrumKit.SampleRate / bpm / 4.0;
        return sixteenth * meter.StepFactor(state.Swing, step);
    }

    private void StartVoice(EngineTrack track, float gain = 1f)
    {
        if (track.Samples.Length == 0)
        {
            return;
        }

        if (track.Hat == HatRole.Closing)
        {
            ChokeOpenHats();
        }

        // Freien Platz suchen, sonst den ältesten Sound ablösen.
        var slot = 0;
        for (var index = 0; index < MaxVoices; index++)
        {
            if (_voices[index].Samples is null)
            {
                slot = index;
                break;
            }
            if (_voices[index].StartedAt < _voices[slot].StartedAt)
            {
                slot = index;
            }
        }

        _voices[slot] = new Voice
        {
            Samples = track.Samples,
            GainLeft = track.GainLeft * gain,
            GainRight = track.GainRight * gain,
            StartedAt = _samplePos,
            IsOpenHat = track.Hat == HatRole.Open,
            Fade = 1f,
        };
    }

    /// <summary>
    /// Wie am Schlagzeug: Ein Hi-Hat-Schlag beendet die noch klingende offene Hi-Hat. Stehen beide auf demselben Schritt,
    /// bleibt die offene, sonst hinge es von der Reihenfolge der Spuren ab, ob man sie hört.
    /// </summary>
    private void ChokeOpenHats()
    {
        for (var slot = 0; slot < MaxVoices; slot++)
        {
            ref var voice = ref _voices[slot];
            if (voice.IsOpenHat && voice.Samples is not null && voice.StartedAt < _samplePos)
            {
                voice.FadeStep = ChokeFadeStep;
            }
        }
    }

    private void MixFrame(float volume, Span<float> frame)
    {
        var left = 0f;
        var right = 0f;
        for (var slot = 0; slot < MaxVoices; slot++)
        {
            ref var voice = ref _voices[slot];
            var samples = voice.Samples;
            if (samples is null)
            {
                continue;
            }

            if (voice.Position >= samples.Length)
            {
                voice.Samples = null;
                continue;
            }

            var sample = samples[voice.Position++];
            if (voice.FadeStep > 0f)
            {
                sample *= voice.Fade;
                voice.Fade -= voice.FadeStep;
                if (voice.Fade <= 0f)
                {
                    voice.Samples = null;
                }
            }
            left += sample * voice.GainLeft;
            right += sample * voice.GainRight;
        }

        frame[0] = Math.Clamp(left * volume, -1f, 1f);
        frame[1] = Math.Clamp(right * volume, -1f, 1f);
    }

    private struct Voice
    {
        public float[]? Samples;
        public int Position;
        public float GainLeft;
        public float GainRight;
        public long StartedAt;
        public bool IsOpenHat;
        public float Fade;       // 1 = volle Lautstärke; sinkt nur, während die Stimme gestoppt wird
        public float FadeStep;   // 0 = klingt normal aus
    }
}
