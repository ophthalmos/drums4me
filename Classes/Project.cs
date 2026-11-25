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

    /// <summary>In der Abfolge ein Takt, in dem das Schlagzeug schweigt. Im Text ist es die 0.</summary>
    public const int SilentBar = -1;

    /// <summary>Länger darf eine Abfolge nicht sein. Das fängt Tippfehler wie „1x9999“ ab.</summary>
    public const int MaxSequenceBars = 999;

    private const int ShortFormFrom = 4;   // ab so vielen gleichen Takten schreibt das Programm „1x8“

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new StepRowConverter(), new SwingFeelConverter() },
    };

    public int Version { get; set; } = 1;
    public int Bpm { get; set; } = 80;
    /// <summary>Triolengefühl bei gerader Schreibweise, siehe <see cref="SwingFeel"/>.</summary>
    public SwingFeel Swing { get; set; }
    public int VolumePercent { get; set; } = 80;
    /// <summary>Das Kit des Projekts. Leer bei einem neuen Projekt: Es bekommt das erste Kit der Auswahlliste.</summary>
    public string KitName { get; set; } = string.Empty;
    public int ActivePattern { get; set; }
    public bool SequenceEnabled { get; set; }

    /// <summary>Die Abfolge in lesbarer Form, zum Beispiel "1 1 1 2" oder "1x8 0x2". Jede Ziffer steht für einen Takt, 0 für einen stummen.</summary>
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
    public StepLevel[][] ActiveSteps => Patterns[ActivePattern].Steps;

    [JsonIgnore]
    public Meter ActiveMeter => Patterns[ActivePattern].Meter;

    /// <summary>Die Pattern-Nummern (ab 0) der eingeschalteten Abfolge, <see cref="SilentBar"/> für stumme Takte, sonst leer.</summary>
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
            if (index != SilentBar)
            {
                PreparePattern(index);
            }
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

    /// <summary>Das Pattern, mit dem die eingeschaltete Abfolge beginnt, stumme Takte übergangen. Null ohne Abfolge oder ohne Pattern darin.</summary>
    [JsonIgnore]
    public int? FirstSequencePattern => ActiveSequence.Cast<int?>().FirstOrDefault(index => index != SilentBar);

    /// <summary>
    /// Liest eine Abfolge. Jede Ziffer ist ein Takt: 1 bis 4 für die Patterns, 0 für einen stummen Takt (<see cref="SilentBar"/>).
    /// „1x8“ steht für acht Takte Pattern 1, statt x gehen auch × und *. Dazwischen sind Leerzeichen, Kommas und
    /// Bindestriche erlaubt, Ziffern dürfen auch direkt aufeinander folgen.
    /// </summary>
    public static bool TryParseSequence(string text, out int[] sequence)
    {
        var bars = new List<int>();
        sequence = [];
        for (var position = 0; position < text.Length; position++)
        {
            var character = text[position];
            if (char.IsWhiteSpace(character) || character is ',' or '-')
            {
                continue;
            }
            if (character < '0' || character >= '1' + PatternCount)
            {
                return false;
            }

            // Hinter der Ziffer kann stehen, wie oft der Takt kommt.
            var count = 1;
            if (position + 1 < text.Length && text[position + 1] is 'x' or 'X' or '×' or '*')
            {
                var digits = 0;
                count = 0;
                for (position += 2; position < text.Length && char.IsAsciiDigit(text[position]) && digits < 4; position++, digits++)
                {
                    count = (count * 10) + (text[position] - '0');
                }
                position--;
                if (digits == 0 || count == 0)
                {
                    return false;
                }
            }

            if (bars.Count + count > MaxSequenceBars)
            {
                return false;
            }
            bars.AddRange(Enumerable.Repeat(character == '0' ? SilentBar : character - '1', count));
        }
        sequence = [.. bars];
        return true;
    }

    /// <summary>
    /// Die einheitliche Schreibweise: Ab vier gleichen Takten in Folge steht die Kurzform „1x8“, kürzere Folgen bleiben
    /// ausgeschrieben, damit das vertraute „1 1 1 2“ so stehen bleibt.
    /// </summary>
    public static string FormatSequence(int[] sequence)
    {
        var parts = new List<string>();
        for (var start = 0; start < sequence.Length;)
        {
            var end = start;
            while (end < sequence.Length && sequence[end] == sequence[start])
            {
                end++;
            }

            var digit = sequence[start] == SilentBar ? "0" : (sequence[start] + 1).ToString();
            if (end - start >= ShortFormFrom)
            {
                parts.Add($"{digit}x{end - start}");
            }
            else
            {
                parts.AddRange(Enumerable.Repeat(digit, end - start));
            }
            start = end;
        }
        return string.Join(' ', parts);
    }

    /// <summary>Neues Projekt mit einem einfachen Metronom-Pattern: Viertel auf der geschlossenen Hi-Hat, Kick auf der Eins.</summary>
    public static Project CreateDefault()
    {
        var project = CreateEmpty();
        const int closedHihat = 5;
        const int kick = 7;
        var steps = project.Patterns[0].Steps;
        for (var step = 0; step < Meter.FourFour.Steps; step += 4)
        {
            steps[closedHihat][step] = StepLevel.Normal;
        }
        steps[kick][0] = StepLevel.Normal;
        return project;
    }

    public static Project Load(string path)
    {
        var project = JsonSerializer.Deserialize<Project>(File.ReadAllText(path), JsonOptions)
            ?? throw new InvalidDataException(Lng.T("Die Datei enthält kein drums4me-Projekt."));
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

    public static StepLevel[][] CreateEmptySteps()
    {
        var steps = new StepLevel[TrackCount][];
        for (var track = 0; track < TrackCount; track++)
        {
            steps[track] = new StepLevel[MaxStepCount];
        }
        return steps;
    }

    /// <summary>Ein Projekt ohne Schläge, wie es nach dem Schließen einer Datei bleibt.</summary>
    public static Project CreateEmpty()
    {
        var project = new Project();
        project.Normalize();
        return project;
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
                tracks[track].Muted = source.Muted;
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
            var used = (sourcePattern?.Used ?? false) || index == ActivePattern || steps.Any(row => row.Any(level => level != StepLevel.Off));
            patterns[index] = new Pattern { Steps = steps, Used = used, MeterName = Meter.Find(sourcePattern?.MeterName).Name };
        }
        Patterns = patterns;
    }
}

/// <summary>
/// Swing, wie ihn Notensatzprogramme kennen: Die Noten stehen gerade im Raster, gespielt werden zwei gleich notierte
/// im Verhältnis 2:1, also wie die erste und die dritte Note einer Triole.
/// </summary>
internal enum SwingFeel
{
    /// <summary>Gerader Rhythmus: Alle Schritte sind gleich lang.</summary>
    Straight,

    /// <summary>Die zweite Achtel jeder Zählzeit kommt später. Wirkt in Taktarten, die in Vierteln zählen.</summary>
    Eighths,

    /// <summary>Jede zweite Sechzehntel kommt später.</summary>
    Sixteenths,
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

    /// <summary>Eine stumme Spur behält Schritte und Einstellungen, sie klingt nur nicht.</summary>
    public bool Muted { get; set; }
}

internal sealed class Pattern
{
    /// <summary>Wurde das Pattern schon einmal aufgerufen? Ein bewusst geleertes Pattern bleibt dadurch leer.</summary>
    public bool Used { get; set; }

    /// <summary>Die Taktart dieses Patterns. Dateien ohne diese Angabe sind im 4/4-Takt.</summary>
    public string MeterName { get; set; } = Meter.FourFour.Name;

    [JsonIgnore]
    public Meter Meter => Meter.Find(MeterName);

    public StepLevel[][] Steps { get; set; } = [];
}

/// <summary>Wie kräftig ein Schritt angeschlagen wird. Off ist ein leerer Schritt.</summary>
internal enum StepLevel : byte
{
    Off,

    /// <summary>Leiser Schlag, am Schlagzeug eine Ghost Note.</summary>
    Soft,

    Normal,

    /// <summary>Betonter Schlag, ein Akzent.</summary>
    Accent,
}

/// <summary>
/// Speichert eine Spur lesbar als Zeichenkette, zum Beispiel "X-o-x-o-": X betont, x normal, o leise, - leer.
/// Dateien aus der Zeit vor den Akzenten kennen nur x und -, sie bleiben gültig.
/// </summary>
internal sealed class StepRowConverter : JsonConverter<StepLevel[]>
{
    private const string Signs = "-oxX";   // in der Reihenfolge von StepLevel

    public override StepLevel[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var text = reader.GetString() ?? string.Empty;
        var row = new StepLevel[text.Length];
        for (var index = 0; index < text.Length; index++)
        {
            row[index] = (StepLevel)Math.Max(0, Signs.IndexOf(text[index]));   // Unbekanntes gilt als leer
        }
        return row;
    }

    public override void Write(Utf8JsonWriter writer, StepLevel[] value, JsonSerializerOptions options)
    {
        var text = new char[value.Length];
        for (var index = 0; index < value.Length; index++)
        {
            text[index] = Signs[(int)value[index]];
        }
        writer.WriteStringValue(new string(text));
    }
}

/// <summary>Speichert den Swing als Wort. Unbekanntes, etwa aus von Hand geänderten Dateien, ergibt den geraden Rhythmus.</summary>
internal sealed class SwingFeelConverter : JsonConverter<SwingFeel>
{
    public override SwingFeel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            reader.Skip();
            return SwingFeel.Straight;
        }
        return Enum.TryParse<SwingFeel>(reader.GetString(), ignoreCase: true, out var swing) && Enum.IsDefined(swing) ? swing : SwingFeel.Straight;
    }

    public override void Write(Utf8JsonWriter writer, SwingFeel value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}
