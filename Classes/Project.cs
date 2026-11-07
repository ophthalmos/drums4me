using System.Text.Json;
using System.Text.Json.Serialization;

namespace Drums4me.Classes;

/// <summary>Ein drums4me-Projekt: Tempo, Kit, Spur-Einstellungen und Patterns. Wird nur im UI-Thread benutzt.</summary>
internal sealed class Project
{
    public const int TrackCount = 8;

    /// <summary>So viele Schritte hält jede Spur vor, auch wenn die Taktart weniger zeigt. So geht beim Wechsel der Taktart nichts verloren.</summary>
    public const int MaxStepCount = 24;

    public const int PatternCount = 4;
    public const int MinBpm = 20;
    public const int MaxBpm = 300;
    public const int MaxCountInBars = 8;
    public const int MaxTrainerStepBpm = 20;
    public const int MaxTrainerEveryBars = 64;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new StepRowConverter() },
    };

    public int Version { get; set; } = 1;
    public int Bpm { get; set; } = 80;
    public int SwingPercent { get; set; }
    public int VolumePercent { get; set; } = 80;
    public string KitName { get; set; } = "Kit 1";
    public int ActivePattern { get; set; }
    public bool SequenceEnabled { get; set; }

    /// <summary>Die Abfolge in lesbarer Form, zum Beispiel "1 1 1 2". Jede Ziffer steht für einen Takt.</summary>
    public string Sequence { get; set; } = string.Empty;

    /// <summary>Beginnt die Abfolge am Ende von vorn? Sonst stoppt die Wiedergabe nach dem letzten Takt.</summary>
    public bool SequenceRepeat { get; set; } = true;

    /// <summary>So viele Takte zählt ein Metronom vor, bevor die Wiedergabe beginnt. 0 bedeutet kein Vorzählen.</summary>
    public int CountInBars { get; set; }

    /// <summary>Der Tempotrainer erhöht beim Spielen das Tempo schrittweise. Bpm bleibt dabei unverändert und ist der Startwert.</summary>
    public bool TrainerEnabled { get; set; }

    /// <summary>Um so viele BPM steigt das Tempo je Schritt.</summary>
    public int TrainerStepBpm { get; set; } = 2;

    /// <summary>Nach so vielen Takten steigt das Tempo.</summary>
    public int TrainerEveryBars { get; set; } = 4;

    /// <summary>Bei diesem Tempo bleibt der Trainer stehen.</summary>
    public int TrainerTargetBpm { get; set; } = 120;

    public TrackSettings[] Tracks { get; set; } = [];
    public Pattern[] Patterns { get; set; } = [];

    [JsonIgnore]
    public bool[][] ActiveSteps => Patterns[ActivePattern].Steps;

    [JsonIgnore]
    public Meter ActiveMeter => Patterns[ActivePattern].Meter;

    /// <summary>Die Pattern-Nummern (ab 0) der eingeschalteten Abfolge, sonst leer.</summary>
    [JsonIgnore]
    public int[] ActiveSequence => SequenceEnabled && TryParseSequence(Sequence, out var sequence) ? sequence : [];

    /// <summary>Wechselt das Pattern. Beim ersten Aufruf übernimmt es den Inhalt des bisher aktiven Patterns.</summary>
    public void ActivatePattern(int index)
    {
        PreparePattern(index);
        ActivePattern = index;
    }

    /// <summary>Damit keine Takte unerwartet stumm bleiben, gilt auch die Abfolge als erster Aufruf ihrer Patterns.</summary>
    public void PrepareSequencePatterns()
    {
        foreach (var index in ActiveSequence)
        {
            PreparePattern(index);
        }
    }

    private void PreparePattern(int index)
    {
        var pattern = Patterns[index];
        if (pattern.Used)
        {
            return;
        }

        for (var track = 0; track < TrackCount; track++)
        {
            Array.Copy(ActiveSteps[track], pattern.Steps[track], MaxStepCount);
        }
        pattern.MeterName = Patterns[ActivePattern].MeterName;
        pattern.Used = true;
    }

    /// <summary>Liest eine Abfolge. Erlaubt sind die Ziffern der Patterns, dazwischen Leerzeichen, Kommas und Bindestriche.</summary>
    public static bool TryParseSequence(string text, out int[] sequence)
    {
        var patterns = new List<int>();
        foreach (var character in text)
        {
            if (character >= '1' && character < '1' + PatternCount)
            {
                patterns.Add(character - '1');
            }
            else if (!char.IsWhiteSpace(character) && character != ',' && character != '-')
            {
                sequence = [];
                return false;
            }
        }
        sequence = [.. patterns];
        return true;
    }

    public static string FormatSequence(int[] sequence) => string.Join(' ', sequence.Select(index => index + 1));

    /// <summary>Neues Projekt mit einem einfachen Metronom-Pattern: Viertel auf der geschlossenen Hi-Hat, Kick auf der Eins.</summary>
    public static Project CreateDefault()
    {
        var project = new Project();
        project.Normalize();
        const int closedHihat = 5;
        const int kick = 7;
        var steps = project.Patterns[0].Steps;
        for (var step = 0; step < Meter.FourFour.Steps; step += 4)
        {
            steps[closedHihat][step] = true;
        }
        steps[kick][0] = true;
        return project;
    }

    public static Project Load(string path)
    {
        var project = JsonSerializer.Deserialize<Project>(File.ReadAllText(path), JsonOptions)
            ?? throw new InvalidDataException("Die Datei enthält kein drums4me-Projekt.");
        project.Normalize();
        return project;
    }

    public void Save(string path) => File.WriteAllText(path, JsonSerializer.Serialize(this, JsonOptions));

    /// <summary>Eine unabhängige Kopie dieses Stands.</summary>
    public Project Snapshot()
    {
        var copy = JsonSerializer.Deserialize<Project>(JsonSerializer.Serialize(this, JsonOptions), JsonOptions)!;
        copy.Normalize();
        return copy;
    }

    public static bool[][] CreateEmptySteps()
    {
        var steps = new bool[TrackCount][];
        for (var track = 0; track < TrackCount; track++)
        {
            steps[track] = new bool[MaxStepCount];
        }
        return steps;
    }

    public static TrackSettings[] CreateDefaultTracks()
    {
        var tracks = new TrackSettings[TrackCount];
        for (var track = 0; track < TrackCount; track++)
        {
            tracks[track] = new TrackSettings();
        }
        return tracks;
    }

    /// <summary>Bringt alle Werte in den gültigen Bereich, damit auch unvollständige oder von Hand geänderte Dateien sicher laufen.</summary>
    private void Normalize()
    {
        Bpm = Math.Clamp(Bpm, MinBpm, MaxBpm);
        CountInBars = Math.Clamp(CountInBars, 0, MaxCountInBars);
        TrainerStepBpm = Math.Clamp(TrainerStepBpm, 1, MaxTrainerStepBpm);
        TrainerEveryBars = Math.Clamp(TrainerEveryBars, 1, MaxTrainerEveryBars);
        TrainerTargetBpm = Math.Clamp(TrainerTargetBpm, MinBpm, MaxBpm);
        SwingPercent = Math.Clamp(SwingPercent, 0, 100);
        VolumePercent = Math.Clamp(VolumePercent, 0, 100);
        ActivePattern = Math.Clamp(ActivePattern, 0, PatternCount - 1);
        KitName ??= string.Empty;
        Sequence = TryParseSequence(Sequence ?? string.Empty, out var sequence) ? FormatSequence(sequence) : string.Empty;

        var tracks = CreateDefaultTracks();
        for (var track = 0; track < TrackCount && track < (Tracks?.Length ?? 0); track++)
        {
            var source = Tracks![track];
            if (source is not null)
            {
                tracks[track].Volume = Math.Clamp(source.Volume, 0f, 1f);
                tracks[track].Pan = Math.Clamp(source.Pan, -1f, 1f);
                tracks[track].Pitch = Math.Clamp(source.Pitch, TrackSettings.MinPitch, TrackSettings.MaxPitch);
            }
        }
        Tracks = tracks;

        var patterns = new Pattern[PatternCount];
        for (var index = 0; index < PatternCount; index++)
        {
            var sourcePattern = index < (Patterns?.Length ?? 0) ? Patterns![index] : null;
            var source = sourcePattern?.Steps;
            var steps = CreateEmptySteps();
            for (var track = 0; track < TrackCount && track < (source?.Length ?? 0); track++)
            {
                var row = source![track];
                if (row is not null)
                {
                    Array.Copy(row, steps[track], Math.Min(row.Length, MaxStepCount));
                }
            }
            // Dateien ohne diese Angabe: Ein Pattern mit Inhalt oder das aktive Pattern gilt als schon benutzt.
            var used = (sourcePattern?.Used ?? false) || index == ActivePattern || steps.Any(row => row.Contains(true));
            patterns[index] = new Pattern { Steps = steps, Used = used, MeterName = Meter.Find(sourcePattern?.MeterName).Name };
        }
        Patterns = patterns;
    }
}

internal sealed class TrackSettings
{
    public const int MinPitch = -12;
    public const int MaxPitch = 12;
    public const float DefaultVolume = 0.8f;

    public float Volume { get; set; } = DefaultVolume;

    /// <summary>Panorama von -1 (links) bis 1 (rechts).</summary>
    public float Pan { get; set; }

    /// <summary>Tonhöhe in Halbtönen.</summary>
    public int Pitch { get; set; }
}

internal sealed class Pattern
{
    /// <summary>Wurde das Pattern schon einmal aufgerufen? Ein bewusst geleertes Pattern bleibt dadurch leer.</summary>
    public bool Used { get; set; }

    /// <summary>Die Taktart dieses Patterns. Dateien ohne diese Angabe sind im 4/4-Takt.</summary>
    public string MeterName { get; set; } = Meter.FourFour.Name;

    [JsonIgnore]
    public Meter Meter => Meter.Find(MeterName);

    public bool[][] Steps { get; set; } = [];
}

/// <summary>Speichert eine Spur lesbar als Zeichenkette, zum Beispiel "x---x---x---x---".</summary>
internal sealed class StepRowConverter : JsonConverter<bool[]>
{
    public override bool[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var text = reader.GetString() ?? string.Empty;
        var row = new bool[text.Length];
        for (var index = 0; index < text.Length; index++)
        {
            row[index] = text[index] == 'x';
        }
        return row;
    }

    public override void Write(Utf8JsonWriter writer, bool[] value, JsonSerializerOptions options)
    {
        var text = new char[value.Length];
        for (var index = 0; index < value.Length; index++)
        {
            text[index] = value[index] ? 'x' : '-';
        }
        writer.WriteStringValue(new string(text));
    }
}
