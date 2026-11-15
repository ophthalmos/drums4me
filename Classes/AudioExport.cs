using NAudio.Wave;

namespace Drums4me.Classes;

/// <summary>Was beim Speichern einer Audiodatei herauskam.</summary>
/// <param name="IsLoop">Die Datei lässt sich nahtlos wiederholen: Sie endet genau auf der Taktgrenze.</param>
internal sealed record AudioExportResult(int Bars, TimeSpan Duration, bool IsLoop);

/// <summary>
/// Speichert als Audiodatei, was die Wiedergabe spielt, gerechnet ohne Soundkarte und damit schneller als in Echtzeit.
/// Vorzählen und Tempotrainer bleiben außen vor. Zwei Fälle:
/// Eine Abfolge ohne Wiederholung ist ein Stück mit Ende, die Datei klingt nach dem letzten Takt aus.
/// Alles andere ist eine Schleife: Die Datei endet genau auf der Taktgrenze, und was über das Ende hinaus klingt, etwa ein
/// Becken, steht schon am Anfang. So klingt sie beim Wiederholen wie das laufende Programm.
/// </summary>
internal static class AudioExport
{
    public const string WaveExtension = ".wav";
    public const string Mp3Extension = ".mp3";
    public const string FlacExtension = ".flac";

    /// <summary>Länger darf eine Audiodatei nicht werden.</summary>
    public const double MaxMinutes = 60;

    private const int Mp3BitRate = 192_000;
    private const int MaxRounds = 100;

    /// <summary>Wird es eine Schleife? Nur eine Abfolge ohne Wiederholung ist ein Stück mit Ende.</summary>
    public static bool IsLoop(Project project) => project.ActiveSequence.Length == 0 || project.SequenceRepeat;

    /// <summary>So viele Takte gehen höchstens: bei einem Stück seine Länge, bei einer Schleife beliebig viele Durchgänge.</summary>
    public static int MaxBars(Project project) => IsLoop(project) ? Project.MaxSequenceBars : project.ActiveSequence.Length;

    /// <summary>Die naheliegende Länge: mit Abfolge ein Durchgang, sonst die zuletzt gewählte Zahl von Takten.</summary>
    public static int DefaultBars(Project project, int remembered) =>
        project.ActiveSequence.Length > 0 ? project.ActiveSequence.Length : Math.Clamp(remembered, 1, Project.MaxSequenceBars);

    /// <summary>
    /// Die Takte, die in die Datei kommen: ohne Abfolge das aktuelle Pattern, mit Abfolge deren Takte von Anfang an.
    /// Eine Abfolge mit Wiederholung läuft dabei so oft durch, wie es die Länge verlangt.
    /// </summary>
    public static int[] BarsOf(Project project, int bars)
    {
        var sequence = project.ActiveSequence;
        if (sequence.Length == 0)
        {
            return [.. Enumerable.Repeat(project.ActivePattern, bars)];
        }

        var result = new int[Math.Min(bars, MaxBars(project))];
        for (var index = 0; index < result.Length; index++)
        {
            result[index] = sequence[index % sequence.Length];
        }
        return result;
    }

    /// <summary>Wie lang die Takte dauern, ohne den Ausklang eines Stücks. Die Engine misst beim Speichern selbst genau.</summary>
    public static TimeSpan DurationOf(Project project, int bars)
    {
        var sequence = BarsOf(project, bars);
        var meter = project.Patterns[sequence.FirstOrDefault(entry => entry != Project.SilentBar, project.ActivePattern)].Meter;
        var steps = 0;
        foreach (var entry in sequence)
        {
            // Ein stummer Takt ist so lang wie ein Takt des Patterns davor.
            meter = entry == Project.SilentBar ? meter : project.Patterns[entry].Meter;
            steps += meter.Steps;
        }
        return TimeSpan.FromSeconds(steps * 15.0 / project.Bpm);   // ein Schritt ist ein Sechzehntel
    }

    /// <summary>
    /// Schreibt <paramref name="bars"/> Takte in die Datei, das Format ergibt sich aus der Endung.
    /// Welche Takte das sind, sagt <see cref="BarsOf"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Die Datei würde zu lang.</exception>
    public static AudioExportResult Save(Project project, DrumKit kit, int bars, string path)
    {
        var sequence = BarsOf(project, bars);
        var isLoop = IsLoop(project);
        var seconds = DurationOf(project, bars).TotalSeconds;
        if (seconds > MaxMinutes * 60)
        {
            throw new InvalidOperationException(string.Format(Lng.T("Die Audiodatei wäre {0:F0} Minuten lang. Mehr als {1:F0} Minuten gehen nicht."), seconds / 60, MaxMinutes));
        }

        var state = EngineState.From(project, kit) with { CountInBars = 0, TrainerStepBpm = 0, RepeatSequence = false };

        // Eine Schleife läuft vorab so oft durch, bis auch der längste Klang aus einem früheren Durchgang verklungen ist.
        // Gespeichert wird der letzte Durchgang: Er enthält am Anfang, was aus dem vorigen noch nachklingt.
        var rounds = 1;
        if (isLoop)
        {
            var longest = state.Tracks.Max(track => track.Samples.Length) / (double)DrumKit.SampleRate;
            rounds = Math.Min(MaxRounds, 1 + (int)Math.Ceiling(longest / seconds));
        }

        var all = new int[sequence.Length * rounds];
        for (var index = 0; index < all.Length; index++)
        {
            all[index] = sequence[index % sequence.Length];
        }

        var engine = new SequencerEngine(state with { Sequence = all });
        var source = new RenderSource(engine, isLoop, ((rounds - 1) * sequence.Length) + 1);
        switch (Path.GetExtension(path).ToLowerInvariant())
        {
            case Mp3Extension:
                MediaFoundationEncoder.EncodeToMp3(source.ToWaveProvider16(), path, Mp3BitRate);
                break;
            case FlacExtension:
                MediaFoundationEncoder.EncodeToFlac(source.ToWaveProvider16(), path);
                break;
            default:
                WaveFileWriter.CreateWaveFile16(path, source);
                break;
        }
        return new AudioExportResult(sequence.Length, TimeSpan.FromSeconds(source.Frames / (double)DrumKit.SampleRate), isLoop);
    }

    /// <summary>Liefert genau den Ausschnitt aus dem Lauf der Engine, der in die Datei gehört, und endet dann.</summary>
    private sealed class RenderSource(SequencerEngine engine, bool isLoop, int firstBar) : ISampleProvider
    {
        // Kürzer als jeder Takt, damit in einem Block höchstens ein Taktanfang liegt.
        private const int BlockFrames = 256;

        private readonly float[] _block = new float[BlockFrames * 2];
        private int _next;         // nächster ungelesener Frame im Block
        private int _end;          // Ende des gültigen Bereichs im Block
        private long _position;    // so viele Frames hat die Engine schon geliefert
        private long _start = -1;  // Schleife: Anfang des letzten Durchgangs
        private bool _finished;

        public WaveFormat WaveFormat => engine.WaveFormat;

        /// <summary>So viele Frames sind in die Datei gegangen.</summary>
        public long Frames { get; private set; }

        public int Read(Span<float> buffer)
        {
            var written = 0;
            while (written + 1 < buffer.Length)
            {
                if (_next == _end && !FillBlock())
                {
                    break;
                }

                var frames = Math.Min((buffer.Length - written) / 2, _end - _next);
                _block.AsSpan(_next * 2, frames * 2).CopyTo(buffer[written..]);
                _next += frames;
                written += frames * 2;
            }
            return written;
        }

        /// <summary>Holt den nächsten Block mit Ton für die Datei. Liefert false, wenn nichts mehr kommt.</summary>
        private bool FillBlock()
        {
            while (!_finished)
            {
                engine.Read(_block);
                var from = _position;
                _position += BlockFrames;
                var low = from;
                var high = _position;
                if (isLoop)
                {
                    if (_start < 0)
                    {
                        if (engine.BarsPlayed < firstBar)
                        {
                            continue;   // noch ein Durchgang zum Einschwingen
                        }
                        _start = engine.CurrentBarStart;
                    }

                    low = Math.Max(low, _start);
                    if (engine.EndPosition is >= 0 and var end)
                    {
                        high = Math.Min(high, end);
                        _finished = true;
                    }
                }
                else if (engine.IsFinished)
                {
                    _finished = true;
                }

                if (high > low)
                {
                    _next = (int)(low - from);
                    _end = (int)(high - from);
                    Frames += high - low;
                    return true;
                }
            }
            return false;
        }
    }
}
