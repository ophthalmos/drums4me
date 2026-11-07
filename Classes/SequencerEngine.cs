using NAudio.Wave;

namespace Drums4me.Classes;

/// <summary>Klangdaten und Mischwerte einer Spur für die Engine.</summary>
internal sealed record EngineTrack(float[] Samples, float GainLeft, float GainRight, double Rate);

/// <summary>Unveränderlicher Stand, den die Oberfläche als Ganzes an den Audio-Thread übergibt.</summary>
internal sealed record EngineState(double Bpm, double Swing, float Volume, bool[][][] Patterns, int ActivePattern, int[] Sequence, bool RepeatSequence, int CountInBars, Meter[] Meters, EngineTrack[] Tracks)
{
    public static EngineState From(Project project, DrumKit kit)
    {
        var tracks = new EngineTrack[Project.TrackCount];
        for (var track = 0; track < Project.TrackCount; track++)
        {
            var settings = project.Tracks[track];
            var angle = (settings.Pan + 1f) * MathF.PI / 4f;   // gleichbleibende Lautheit über das Panorama
            tracks[track] = new EngineTrack(
                kit.SamplesOf(track),
                settings.Volume * MathF.Cos(angle),
                settings.Volume * MathF.Sin(angle),
                Math.Pow(2.0, settings.Pitch / 12.0));
        }

        var patterns = new bool[Project.PatternCount][][];
        for (var pattern = 0; pattern < Project.PatternCount; pattern++)
        {
            patterns[pattern] = new bool[Project.TrackCount][];
            for (var track = 0; track < Project.TrackCount; track++)
            {
                patterns[pattern][track] = (bool[])project.Patterns[pattern].Steps[track].Clone();
            }
        }

        // Bei 100 % dauern gerade Schritte das 1,5-Fache und ungerade die Hälfte.
        return new EngineState(project.Bpm, project.SwingPercent / 200.0, project.VolumePercent / 100f,
            patterns, project.ActivePattern, project.ActiveSequence, project.SequenceRepeat, project.CountInBars, project.Patterns.Select(pattern => pattern.Meter).ToArray(), tracks);
    }
}

/// <summary>
/// Taktgeber und Mischer. Das Tempo ergibt sich allein aus dem Samplezähler im Audiostrom und ist damit unabhängig
/// von der Rechnerlast. Read läuft im Audio-Thread und darf deshalb weder blockieren noch Speicher anfordern.
/// </summary>
internal sealed class SequencerEngine : ISampleProvider
{
    private const int MaxVoices = 32;

    // Stille nach dem letzten Sound, damit auch der Rest im Ausgabepuffer noch zu hören ist.
    private const int TrailingSilenceSamples = DrumKit.SampleRate / 5;

    // Beim Vorzählen klingt die Eins am höchsten und lautesten, der Anfang einer Gruppe wie 3+2 etwas schwächer.
    private static readonly EngineTrack AccentClick = CreateClick(1600f, 0.9f);
    private static readonly EngineTrack GroupClick = CreateClick(1300f, 0.75f);
    private static readonly EngineTrack BeatClick = CreateClick(1000f, 0.6f);

    private readonly Voice[] _voices = new Voice[MaxVoices];
    private volatile EngineState _state;
    private volatile int _currentStep = -1;
    private volatile int _currentPattern = -1;
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

    public SequencerEngine(EngineState state) => _state = state;

    public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(DrumKit.SampleRate, 2);

    /// <summary>Der zuletzt ausgelöste Schritt, -1 vor dem ersten Schritt.</summary>
    public int CurrentStep => _currentStep;

    /// <summary>Das zuletzt gespielte Pattern, -1 vor dem ersten Schritt.</summary>
    public int CurrentPattern => _currentPattern;

    /// <summary>Die Position des letzten Taktanfangs im Audiostrom in Samples, -1 vor dem ersten Takt.</summary>
    public long LastBarStart => Interlocked.Read(ref _lastBarStart);

    /// <summary>Der gerade vorgezählte Takt ab 1, 0 außerhalb des Vorzählens.</summary>
    public int CountInBar => _countInBar;

    /// <summary>Die Abfolge ohne Wiederholung ist zu Ende, und alle Sounds sind ausgeklungen.</summary>
    public bool IsFinished => _isFinished;

    public void Update(EngineState state) => _state = state;

    /// <summary>Setzt die Wiedergabe beim nächsten Read auf den Anfang zurück und beendet klingende Sounds.</summary>
    public void Rewind()
    {
        _currentStep = -1;
        _currentPattern = -1;
        _isFinished = false;
        _countInBar = 0;
        Interlocked.Exchange(ref _lastBarStart, -1);
        Interlocked.Exchange(ref _rewindPending, 1);
    }

    public int Read(Span<float> buffer)
    {
        var state = _state;
        if (Interlocked.Exchange(ref _rewindPending, 0) == 1)
        {
            ResetTransport(state);
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
        _playingPattern = state.Sequence.Length > 0 ? state.Sequence[0] : state.ActivePattern;
        _countInMeter = state.Meters[_playingPattern];
        _countInStepsLeft = state.CountInBars * _countInMeter.Steps;
        _countInBar = 0;
        _samplePos = 0;
        _nextStepPos = 0;
        _nextStep = 0;
        _currentStep = -1;
        _currentPattern = -1;
        _sequenceIndex = -1;
        _sequenceEnded = false;
        _trailingSamples = 0;
        _isFinished = false;
        Interlocked.Exchange(ref _lastBarStart, -1);
        Array.Clear(_voices);
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
            _playingPattern = sequence[_sequenceIndex];
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
                StartVoice(AccentClick);
                break;
            case Meter.ClickGroup:
                StartVoice(GroupClick);
                break;
            case Meter.ClickBeat:
                StartVoice(BeatClick);
                break;
        }

        _countInStepsLeft--;
        _nextStepPos += StepLength(state, step);
        _nextStep = (step + 1) % _countInMeter.Steps;
    }

    private static EngineTrack CreateClick(float frequency, float level)
    {
        var samples = new float[DrumKit.SampleRate * 3 / 100];
        for (var index = 0; index < samples.Length; index++)
        {
            var time = index / (float)DrumKit.SampleRate;
            samples[index] = level * MathF.Sin(2f * MathF.PI * frequency * time) * MathF.Exp(-time / 0.008f);
        }

        var centered = MathF.Cos(MathF.PI / 4f);
        return new EngineTrack(samples, centered, centered, 1.0);
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
            _currentStep = -1;
            return;
        }

        if (step == 0)
        {
            Interlocked.Exchange(ref _lastBarStart, _samplePos);
        }

        var steps = state.Patterns[_playingPattern];
        for (var track = 0; track < Project.TrackCount; track++)
        {
            if (steps[track][step])
            {
                StartVoice(state.Tracks[track]);
            }
        }

        _currentStep = step;
        _currentPattern = _playingPattern;
        _nextStepPos += StepLength(state, step);
        _nextStep = (step + 1) % state.Meters[_playingPattern].Steps;
    }

    private static double StepLength(EngineState state, int step)
    {
        var sixteenth = 60.0 * DrumKit.SampleRate / state.Bpm / 4.0;
        return step % 2 == 0 ? sixteenth * (1.0 + state.Swing) : sixteenth * (1.0 - state.Swing);
    }

    private void StartVoice(EngineTrack track)
    {
        if (track.Samples.Length == 0)
        {
            return;
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
            Rate = track.Rate,
            GainLeft = track.GainLeft,
            GainRight = track.GainRight,
            StartedAt = _samplePos,
        };
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

            var position = (int)voice.Position;
            if (position >= samples.Length)
            {
                voice.Samples = null;
                continue;
            }

            var fraction = (float)(voice.Position - position);
            var next = position + 1 < samples.Length ? samples[position + 1] : 0f;
            var sample = samples[position] + ((next - samples[position]) * fraction);
            left += sample * voice.GainLeft;
            right += sample * voice.GainRight;
            voice.Position += voice.Rate;
        }

        frame[0] = Math.Clamp(left * volume, -1f, 1f);
        frame[1] = Math.Clamp(right * volume, -1f, 1f);
    }

    private struct Voice
    {
        public float[]? Samples;
        public double Position;
        public double Rate;
        public float GainLeft;
        public float GainRight;
        public long StartedAt;
    }
}
