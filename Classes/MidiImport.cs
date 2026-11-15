using NAudio.Midi;
using System.Text.RegularExpressions;

namespace Drums4me.Classes;

/// <summary>Ein Takt aus einer MIDI-Datei, schon auf das Raster und die Spuren des Kits gebracht.</summary>
internal sealed class MidiBar
{
    /// <summary>Der Takt der Datei, in dem dieser Inhalt zum ersten Mal vorkommt, ab 1.</summary>
    public required int Number { get; init; }

    public required Meter Meter { get; init; }

    public required StepLevel[][] Steps { get; init; }

    /// <summary>So oft kommt genau dieser Takt in der Datei vor.</summary>
    public int Count { get; set; } = 1;

    public int Hits => Steps.Sum(row => row.Count(level => level != StepLevel.Off));
}

/// <summary>Wohin ein Schlagzeugklang der Datei gelangt ist. Ohne Spur wurde er nicht übernommen.</summary>
internal sealed record MidiMapping(string Instrument, string? Track, int Hits);

/// <summary>
/// Liest die Schlagzeugspur (Kanal 10) einer MIDI-Datei als Vorlage für Patterns. drums4me bleibt dabei ein Metronom:
/// Übernommen werden einzelne Takte, auf Sechzehntel gerundet und auf die acht Spuren des Kits verteilt, kein ganzer Song.
/// </summary>
internal sealed partial class MidiImport
{
    private const int DrumChannel = 10;
    private const int MaxGroovePitches = 16;   // mehr verschiedene Klänge hat ein einzelner Groove nicht

    // Ein triolisch gespielter Groove wird zu Swing-Achteln: Dort liegen die Schritte einer Zählzeit bei 0, 1/3, 2/3 und 5/6.
    private const double TripletShare = 2.0;   // so viel häufiger als gerade Zwischenschläge müssen triolische sein

    private MidiImport(string name, int bpm, SwingFeel swing, List<MidiBar> bars, int[] order, List<MidiMapping> mappings)
    {
        Name = name;
        Bpm = bpm;
        Swing = swing;
        Bars = bars;
        Order = order;
        Mappings = mappings;
    }

    /// <summary>Der Dateiname ohne Endung.</summary>
    public string Name { get; }

    public int Bpm { get; }

    public SwingFeel Swing { get; }

    /// <summary>Die verschiedenen Takte mit Schlagzeug, in der Reihenfolge ihres ersten Vorkommens.</summary>
    public IReadOnlyList<MidiBar> Bars { get; }

    /// <summary>Die Takte der Datei vom ersten bis zum letzten mit Schlagzeug: jeweils die Stelle in <see cref="Bars"/>, -1 für einen leeren Takt.</summary>
    public int[] Order { get; }

    public IReadOnlyList<MidiMapping> Mappings { get; }

    /// <summary>So viele Takte passen in ein Projekt.</summary>
    public static int MaxBars => Project.PatternCount;

    /// <summary>Die Takte, die vorgeschlagen werden: alle, wenn sie in ein Projekt passen, sonst die häufigsten.</summary>
    public IReadOnlyList<MidiBar> Suggested =>
        [.. Bars.OrderByDescending(bar => bar.Count).ThenBy(bar => bar.Number).Take(MaxBars).OrderBy(bar => bar.Number)];

    /// <summary>Lässt sich mit dieser Auswahl die Abfolge der Datei nachspielen? Dazu müssen alle ihre Takte dabei sein.</summary>
    public bool CanFollowFile(IReadOnlyList<MidiBar> chosen) =>
        Order.Length > 1 && Order.Length <= Project.MaxSequenceBars && Order.Distinct().Count() > 1 && Bars.All(chosen.Contains);

    /// <summary>
    /// Ein neues Projekt aus den gewählten Takten, in der Reihenfolge der Datei. Sind alle Takte der Datei dabei,
    /// spielt die Abfolge sie so ab wie die Datei, leere Takte als stumme.
    /// </summary>
    public Project CreateProject(IReadOnlyList<MidiBar> chosen, string kitName)
    {
        var bars = chosen.OrderBy(bar => bar.Number).Take(MaxBars).ToList();
        var project = Project.CreateEmpty();
        project.Bpm = Bpm;
        project.Swing = Swing;
        project.KitName = kitName;
        for (var index = 0; index < bars.Count; index++)
        {
            var pattern = project.Patterns[index];
            for (var track = 0; track < Project.TrackCount; track++)
            {
                Array.Copy(bars[index].Steps[track], pattern.Steps[track], Project.MaxStepCount);
            }
            pattern.MeterName = bars[index].Meter.Name;
            pattern.Used = true;
        }

        if (CanFollowFile(bars))
        {
            project.Sequence = Project.FormatSequence([.. Order.Select(bar => bar < 0 ? Project.SilentBar : bars.IndexOf(Bars[bar]))]);
            project.SequenceEnabled = true;
            project.SequenceRepeat = true;
        }
        return project;
    }

    /// <exception cref="InvalidDataException">Die Datei enthält nichts, was sich übernehmen lässt. Der Text sagt, warum.</exception>
    public static MidiImport Read(string path, DrumKit kit)
    {
        MidiFile file;
        try
        {
            file = new MidiFile(path, strictChecking: false);
        }
        catch (Exception ex) when (ex is FormatException or InvalidDataException or EndOfStreamException or ArgumentException or IndexOutOfRangeException or NotSupportedException)
        {
            throw new InvalidDataException(Lng.T("Die Datei ist keine gültige MIDI-Datei."), ex);
        }

        var quarter = file.DeltaTicksPerQuarterNote;
        if (quarter <= 0)
        {
            throw new InvalidDataException(Lng.T("Die Datei misst die Zeit in Bildern statt in Noten. So etwas kann drums4me nicht lesen."));
        }

        var all = new List<NoteOnEvent>();
        var signatures = new List<TimeSignatureEvent>();
        TempoEvent? tempo = null;
        for (var track = 0; track < file.Tracks; track++)
        {
            foreach (var midiEvent in file.Events[track])
            {
                switch (midiEvent)
                {
                    case NoteOnEvent { Velocity: > 0 } note:
                        all.Add(note);
                        break;
                    case TimeSignatureEvent signature:
                        signatures.Add(signature);
                        break;
                    case TempoEvent first when tempo is null || first.AbsoluteTime < tempo.AbsoluteTime:
                        tempo = first;
                        break;
                }
            }
        }
        var notes = DrumNotes(all);
        if (notes.Count == 0)
        {
            throw new InvalidDataException(Lng.T("Die Datei enthält kein Schlagzeug auf Kanal 10."));
        }
        notes.Sort((a, b) => a.AbsoluteTime.CompareTo(b.AbsoluteTime));

        var frames = Frames(signatures, quarter, notes[^1].AbsoluteTime);
        if (frames.All(frame => frame.Meter is null))
        {
            throw new InvalidDataException(Lng.T("Die Taktart der Datei kennt drums4me nicht."));
        }

        var swing = IsTriplet(notes, frames, quarter) ? SwingFeel.Eighths : SwingFeel.Straight;
        var mapper = new TrackMapper(kit);
        var levels = new VelocityLevels(notes, mapper);
        var content = new StepLevel[frames.Count][][];
        var frameIndex = 0;
        foreach (var note in notes)
        {
            // Ein Schlag knapp vor dem Taktstrich gehört auf die Eins des nächsten Takts.
            var early = quarter / (swing == SwingFeel.Eighths ? 6 : 8);
            while (frameIndex + 1 < frames.Count && note.AbsoluteTime + early >= frames[frameIndex + 1].Start)
            {
                frameIndex++;
            }

            var frame = frames[frameIndex];
            var track = mapper.TrackOf(note.NoteNumber);
            if (frame.Meter is null || track < 0)
            {
                continue;
            }

            var step = StepOf(Math.Max(0, note.AbsoluteTime - frame.Start), quarter, frame.Meter, swing);
            if (step >= frame.Meter.Steps)
            {
                continue;   // gehört schon zum nächsten Takt, den es in dieser Taktart nicht mehr gibt
            }
            var steps = content[frameIndex] ??= Project.CreateEmptySteps();
            var level = levels.LevelOf(track, note.Velocity);
            if (level > steps[track][step])
            {
                steps[track][step] = level;   // fallen zwei Schläge auf einen Schritt, zählt der kräftigere
            }
        }

        // Gleiche Takte zusammenfassen, leere am Anfang und Ende weglassen
        var bars = new List<MidiBar>();
        var known = new Dictionary<string, int>();
        var order = new List<int>();
        for (var index = 0; index < frames.Count; index++)
        {
            if (content[index] is not { } steps || frames[index].Meter is not { } meter)
            {
                order.Add(-1);
                continue;
            }

            var key = meter.Name + "|" + string.Join('|', steps.Select(row => string.Concat(row.Select(level => (char)('0' + (int)level)))));
            if (known.TryGetValue(key, out var existing))
            {
                bars[existing].Count++;
                order.Add(existing);
            }
            else
            {
                known[key] = bars.Count;
                order.Add(bars.Count);
                bars.Add(new MidiBar { Number = index + 1, Meter = meter, Steps = steps });
            }
        }
        if (bars.Count == 0)
        {
            throw new InvalidDataException(string.Format(Lng.T("Kein Schlagzeugklang der Datei passt zu einer Spur von „{0}“."), kit.Name));
        }

        var from = order.FindIndex(bar => bar >= 0);
        var to = order.FindLastIndex(bar => bar >= 0);
        var bpm = (int)Math.Round(Math.Clamp(tempo?.Tempo ?? 120.0, Project.MinBpm, Project.MaxBpm));
        return new MidiImport(Path.GetFileNameWithoutExtension(path), bpm, swing, bars, [.. order.Skip(from).Take(to - from + 1)], mapper.Report(notes));
    }

    /// <summary>
    /// Schlagzeug liegt im MIDI-Standard auf Kanal 10. Einzelne Grooves aus Musikprogrammen stehen aber oft auf Kanal 1:
    /// Hat die Datei nur einen Kanal und darauf wenige verschiedene Klänge aus dem Bereich des Schlagzeugs, gilt er als Schlagzeug.
    /// Ein Klavierstück auf einem Kanal fällt durch, denn es benutzt viel mehr verschiedene Töne.
    /// </summary>
    private static List<NoteOnEvent> DrumNotes(List<NoteOnEvent> all)
    {
        var drums = all.Where(note => note.Channel == DrumChannel).ToList();
        if (drums.Count > 0)
        {
            return drums;
        }

        var pitches = all.Select(note => note.NoteNumber).Distinct().ToList();
        var oneChannel = all.Select(note => note.Channel).Distinct().Count() == 1;
        return oneChannel && pitches.Count <= MaxGroovePitches && pitches.All(TrackMapper.IsDrumNote) ? all : [];
    }

    /// <summary>Ein Takt der Datei: sein Anfang in Ticks und seine Taktart. Ohne Taktart kennt drums4me sie nicht.</summary>
    private sealed record Frame(long Start, Meter? Meter);

    /// <summary>Teilt die Datei nach ihren Taktartwechseln in Takte. Ohne Angabe gilt wie im MIDI-Standard 4/4.</summary>
    private static List<Frame> Frames(List<TimeSignatureEvent> signatures, int quarter, long lastNote)
    {
        signatures.Sort((a, b) => a.AbsoluteTime.CompareTo(b.AbsoluteTime));
        var frames = new List<Frame>();
        var numerator = 4;
        var denominator = 4;
        var next = 0;
        long start = 0;
        while (start <= lastNote)
        {
            while (next < signatures.Count && signatures[next].AbsoluteTime <= start)
            {
                numerator = Math.Max(1, signatures[next].Numerator);
                denominator = 1 << Math.Clamp(signatures[next].Denominator, 0, 6);
                next++;
            }

            var length = Math.Max(1L, (long)numerator * quarter * 4 / denominator);
            var end = start + length;
            if (next < signatures.Count && signatures[next].AbsoluteTime < end)
            {
                end = signatures[next].AbsoluteTime;   // ein Wechsel mitten im Takt beginnt einen neuen
            }
            frames.Add(new Frame(start, MeterOf(numerator, denominator)));
            start = end;
        }
        return frames;
    }

    /// <summary>Die Taktart mit diesem Namen. 2/2 und 4/4 sind im Raster dasselbe, bei 5/4 und 7/8 gilt die erste Gliederung.</summary>
    private static Meter? MeterOf(int numerator, int denominator)
    {
        if (denominator == 2)
        {
            numerator *= 2;
            denominator = 4;
        }
        var name = $"{numerator}/{denominator}";
        return Meter.All.FirstOrDefault(meter => meter.Name == name || meter.Name.StartsWith(name + " ", StringComparison.Ordinal));
    }

    /// <summary>
    /// Triolisch ist ein Groove, wenn deutlich mehr Schläge auf dem zweiten und dritten Drittel der Viertel liegen
    /// als auf ihren geraden Teilen (Viertel, Hälfte, drei Viertel). Gezählt wird nur in Taktarten, die in Vierteln zählen.
    /// </summary>
    private static bool IsTriplet(List<NoteOnEvent> notes, List<Frame> frames, int quarter)
    {
        var triplet = 0;
        var straight = 0;
        var frameIndex = 0;
        foreach (var note in notes)
        {
            while (frameIndex + 1 < frames.Count && note.AbsoluteTime >= frames[frameIndex + 1].Start)
            {
                frameIndex++;
            }
            if (frames[frameIndex].Meter is not { } meter || meter.StepFactor(SwingFeel.Eighths, 0) == 1.0)
            {
                continue;
            }

            var part = (note.AbsoluteTime - frames[frameIndex].Start) % quarter / (double)quarter;
            const double Near = 1.0 / 24.0;
            if (Math.Abs(part - (1.0 / 3.0)) < Near || Math.Abs(part - (2.0 / 3.0)) < Near)
            {
                triplet++;
            }
            else if (Math.Abs(part - 0.25) < Near || Math.Abs(part - 0.5) < Near || Math.Abs(part - 0.75) < Near)
            {
                straight++;
            }
        }
        return triplet > TripletShare * straight && triplet > 0;
    }

    private static int StepOf(long ticks, int quarter, Meter meter, SwingFeel swing)
    {
        if (swing == SwingFeel.Eighths && meter.StepFactor(SwingFeel.Eighths, 0) != 1.0)
        {
            // Drittel der Viertel liegen bei Swing-Achteln genau auf den ersten drei Schritten der Zählzeit.
            var third = (int)Math.Round(ticks * 3.0 / quarter);
            return (third / 3 * 4) + (third % 3);
        }
        return (int)Math.Round(ticks * 4.0 / quarter);
    }

    /// <summary>
    /// Aus der Anschlagstärke werden drei Stufen, gemessen am üblichen Anschlag derselben Spur: Viele Dateien spielen
    /// alles gleich laut, andere die Hi-Hat grundsätzlich leiser als die Snare. Beides soll normal bleiben.
    /// </summary>
    private sealed class VelocityLevels
    {
        private readonly int[] _usual = new int[Project.TrackCount];

        public VelocityLevels(List<NoteOnEvent> notes, TrackMapper mapper)
        {
            var byTrack = notes.Select(note => (Track: mapper.TrackOf(note.NoteNumber), note.Velocity)).Where(note => note.Track >= 0).ToLookup(note => note.Track, note => note.Velocity);
            foreach (var track in byTrack)
            {
                var sorted = track.Order().ToArray();
                _usual[track.Key] = sorted[sorted.Length / 2];
            }
        }

        public StepLevel LevelOf(int track, int velocity)
        {
            var usual = _usual[track];
            return velocity < usual * 0.6 ? StepLevel.Soft
                : velocity > usual * 1.25 && velocity >= usual + 15 ? StepLevel.Accent
                : StepLevel.Normal;
        }
    }

    /// <summary>Ordnet die Schlagzeugklänge des MIDI-Standards (General MIDI) den Spuren des Kits zu, abgelesen an den Spurnamen.</summary>
    private sealed partial class TrackMapper
    {
        private const int FirstNote = 27;

        private static readonly string[] Names =
        [
            "High Q", "Slap", "Scratch Push", "Scratch Pull", "Sticks", "Square Click", "Metronome Click", "Metronome Bell",
            "Acoustic Bass Drum", "Bass Drum 1", "Side Stick", "Acoustic Snare", "Hand Clap", "Electric Snare", "Low Floor Tom", "Closed Hi-Hat",
            "High Floor Tom", "Pedal Hi-Hat", "Low Tom", "Open Hi-Hat", "Low-Mid Tom", "Hi-Mid Tom", "Crash Cymbal 1", "High Tom",
            "Ride Cymbal 1", "Chinese Cymbal", "Ride Bell", "Tambourine", "Splash Cymbal", "Cowbell", "Crash Cymbal 2", "Vibraslap",
            "Ride Cymbal 2", "Hi Bongo", "Low Bongo", "Mute Hi Conga", "Open Hi Conga", "Low Conga", "High Timbale", "Low Timbale",
            "High Agogo", "Low Agogo", "Cabasa", "Maracas", "Short Whistle", "Long Whistle", "Short Guiro", "Long Guiro",
            "Claves", "Hi Wood Block", "Low Wood Block", "Mute Cuica", "Open Cuica", "Mute Triangle", "Open Triangle", "Shaker",
        ];

        private readonly DrumKit _kit;
        private readonly int[] _toms;
        private readonly Dictionary<int, int> _tracks = [];

        public TrackMapper(DrumKit kit)
        {
            _kit = kit;

            // Die Toms des Kits von hoch nach tief: zuerst die ohne „Floor“ oder „Low“ im Namen, sonst in der Reihenfolge der Spuren
            _toms = [.. Tracks(TomWord()).OrderBy(track => LowWord().IsMatch(kit.TrackNames[track]) ? 1 : 0)];
        }

        public static bool IsDrumNote(int note) => note >= FirstNote && note < FirstNote + Names.Length;

        public static string NameOf(int note) => IsDrumNote(note) ? Names[note - FirstNote] : string.Format(Lng.T("Klang {0}"), note);

        public int TrackOf(int note)
        {
            if (!_tracks.TryGetValue(note, out var track))
            {
                _tracks[note] = track = Find(note);
            }
            return track;
        }

        /// <summary>Je Klang der Datei: wohin er ging und wie oft er vorkommt, die häufigsten zuerst.</summary>
        public List<MidiMapping> Report(List<NoteOnEvent> notes) =>
            [.. notes.GroupBy(note => note.NoteNumber).OrderByDescending(group => group.Count())
                .Select(group => new MidiMapping(NameOf(group.Key), TrackOf(group.Key) is >= 0 and var track ? _kit.TrackNames[track] : null, group.Count()))];

        private int Find(int note) => note switch
        {
            35 or 36 => First(KickWord()),
            38 or 40 => First(SnareWord()),
            37 => First(RimWord(), SnareWord()),
            39 => First(ClapWord(), SnareWord()),
            42 => Hat(HatRole.Closing),
            44 => First(PedalWord()) is >= 0 and var pedal ? pedal : Hat(HatRole.Closing),
            46 => Hat(HatRole.Open) is >= 0 and var open ? open : Hat(HatRole.Closing),
            48 or 50 => Tom(0),
            45 or 47 => Tom(1),
            41 or 43 => Tom(2),
            49 or 52 or 55 or 57 => First(CrashWord()),

            // Ohne Ride-Spur hält die geschlossene Hi-Hat den Puls, sonst fehlte dem Groove das Wichtigste.
            51 or 53 or 59 => First(RideWord()) is >= 0 and var ride ? ride : Hat(HatRole.Closing),
            _ => ByName(note),
        };

        private int Hat(HatRole role) => Enumerable.Range(0, Project.TrackCount).Where(track => _kit.HatRoleOf(track) == role).DefaultIfEmpty(-1).First();

        /// <summary>Hohe, mittlere und tiefe Toms verteilen sich auf so viele Tom-Spuren, wie das Kit hat.</summary>
        private int Tom(int height) => _toms.Length == 0 ? -1 : _toms[(int)Math.Round(height * (_toms.Length - 1) / 2.0)];

        /// <summary>Percussion gelangt nur auf eine Spur, die so heißt wie der Klang, etwa „Cowbell“ oder „Conga“.</summary>
        private int ByName(int note)
        {
            foreach (var word in NameOf(note).Split(' ', '-'))
            {
                if (word.Length < 4 || GenericWord().IsMatch(word))
                {
                    continue;
                }
                var stem = word.Length > 5 ? word[..5] : word;   // der Wortanfang genügt: „Tambo“ trifft „Tambourine“ wie „Tambourin“
                var track = Enumerable.Range(0, Project.TrackCount).Where(candidate => _kit.HasTrack(candidate)
                    && _kit.TrackNames[candidate].Contains(stem, StringComparison.OrdinalIgnoreCase)).DefaultIfEmpty(-1).First();
                if (track >= 0)
                {
                    return track;
                }
            }
            return -1;
        }

        private int First(params Regex[] words)
        {
            foreach (var word in words)
            {
                var tracks = Tracks(word);
                if (tracks.Length > 0)
                {
                    return tracks[0];
                }
            }
            return -1;
        }

        private int[] Tracks(Regex word) => [.. Enumerable.Range(0, Project.TrackCount).Where(track => _kit.HasTrack(track) && word.IsMatch(_kit.TrackNames[track]))];

        [GeneratedRegex(@"kick|bass ?drum|\bbd\b", RegexOptions.IgnoreCase)]
        private static partial Regex KickWord();

        [GeneratedRegex(@"snare|\bsd\b", RegexOptions.IgnoreCase)]
        private static partial Regex SnareWord();

        [GeneratedRegex(@"rim|stick", RegexOptions.IgnoreCase)]
        private static partial Regex RimWord();

        [GeneratedRegex(@"clap", RegexOptions.IgnoreCase)]
        private static partial Regex ClapWord();

        [GeneratedRegex(@"pedal", RegexOptions.IgnoreCase)]
        private static partial Regex PedalWord();

        [GeneratedRegex(@"tom", RegexOptions.IgnoreCase)]
        private static partial Regex TomWord();

        [GeneratedRegex(@"floor|low|\blo\b|stand", RegexOptions.IgnoreCase)]
        private static partial Regex LowWord();

        [GeneratedRegex(@"crash|cymbal|becken|splash|china", RegexOptions.IgnoreCase)]
        private static partial Regex CrashWord();

        [GeneratedRegex(@"ride", RegexOptions.IgnoreCase)]
        private static partial Regex RideWord();

        [GeneratedRegex(@"^(high|mute|open|short|long|wood|block)$", RegexOptions.IgnoreCase)]
        private static partial Regex GenericWord();
    }
}
