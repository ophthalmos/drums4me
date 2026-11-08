using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Drums4me.Classes;

/// <summary>
/// Ein Schlagzeug-Kit aus einem Unterordner des Kit-Ordners, wahlweise eine Ebene tiefer in einem Sammelordner.
/// Die ersten acht WAV-Dateien (nach Namen sortiert) bilden die Spuren. Führende Ziffern bestimmen nur die
/// Reihenfolge und erscheinen nicht im Namen.
/// </summary>
internal sealed partial class DrumKit
{
    public const int SampleRate = 44100;

    /// <summary>So heißt eine Spur, für die das Kit keine Datei hat.</summary>
    public const string NoTrackName = "–";

    private readonly string[] _files;
    private readonly float[][] _samples = new float[Project.TrackCount][];
    private bool _loaded;

    private DrumKit(string name, string folder, string[] files, bool isOwn, string? collection = null)
    {
        Name = name;
        Folder = folder;
        IsOwn = isOwn;
        Collection = collection;
        _files = files;
        TrackNames = new string[Project.TrackCount];
        for (var track = 0; track < Project.TrackCount; track++)
        {
            TrackNames[track] = track < files.Length ? TrackNameOf(files[track]) : NoTrackName;
            _samples[track] = [];
        }
    }

    /// <summary>Stumme Rückfallebene, falls der Kit-Ordner leer ist.</summary>
    public static DrumKit Empty { get; } = new("(kein Kit)", string.Empty, [], false);

    public string Name { get; }

    /// <summary>Der Ordner mit den Klängen. Leer beim stummen Ersatz-Kit.</summary>
    public string Folder { get; }

    /// <summary>Liegt das Kit im Ordner des Benutzers? Nur dort darf das Programm Dateien umbenennen.</summary>
    public bool IsOwn { get; }

    /// <summary>Der Sammelordner, in dem das Kit liegt, zum Beispiel "hydrogen". Null, wenn es direkt im Kit-Ordner liegt.</summary>
    public string? Collection { get; }

    public string[] TrackNames { get; }

    public float[] SamplesOf(int track) => _samples[track];

    public bool HasTrack(int track) => track >= 0 && track < _files.Length;

    /// <summary>So erscheint das Kit in der Auswahlliste. Projekte merken sich dagegen Name.</summary>
    public override string ToString() => IsOwn ? $"{Name} (eigen)" : Collection is null ? Name : $"{Name} ({Collection})";

    /// <summary>
    /// Installierte Kits, danach die eigenen. Projekte finden ihr Kit über den Namen, deshalb gibt es jeden nur einmal:
    /// Ein eigenes Kit ersetzt ein gleichnamiges installiertes, sonst gilt das erste.
    /// </summary>
    public static List<DrumKit> Scan(string installedRoot, string ownRoot)
    {
        var own = ScanFolder(ownRoot, true);
        var kits = ScanFolder(installedRoot, false)
            .Where(kit => !own.Exists(other => string.Equals(other.Name, kit.Name, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        kits.AddRange(own);
        return [.. kits.DistinctBy(kit => kit.Name, StringComparer.OrdinalIgnoreCase)];
    }

    private static List<DrumKit> ScanFolder(string root, bool isOwn)
    {
        var kits = new List<DrumKit>();
        if (!Directory.Exists(root))
        {
            return kits;
        }

        foreach (var folder in SortedFolders(root))
        {
            var files = TrackFiles(folder);
            if (files.Length > 0)
            {
                kits.Add(new DrumKit(CleanName(Path.GetFileName(folder)), folder, files, isOwn));
                continue;
            }

            // Ohne eigene Klänge ist es ein Sammelordner mit weiteren Kits.
            foreach (var kitFolder in SortedFolders(folder))
            {
                files = TrackFiles(kitFolder);
                if (files.Length > 0)
                {
                    kits.Add(new DrumKit(CleanName(Path.GetFileName(kitFolder)), kitFolder, files, isOwn, Path.GetFileName(folder)));
                }
            }
        }
        return kits;
    }

    private static IEnumerable<string> SortedFolders(string root) =>
        Directory.GetDirectories(root).OrderBy(path => path, StringComparer.OrdinalIgnoreCase);

    private static string[] TrackFiles(string folder) => [.. Directory.GetFiles(folder, "*.wav")
        .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
        .Take(Project.TrackCount)];

    /// <summary>Lädt die Samples beim ersten Gebrauch. Liefert die Namen der Dateien, die nicht lesbar waren.</summary>
    public List<string> EnsureLoaded()
    {
        var failed = new List<string>();
        if (_loaded)
        {
            return failed;
        }

        for (var track = 0; track < _files.Length; track++)
        {
            try
            {
                _samples[track] = ReadAsMono(_files[track]);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException
                or FormatException or ArgumentException or COMException or MmException)
            {
                failed.Add(Path.GetFileName(_files[track]));
            }
        }
        _loaded = true;
        return failed;
    }

    private static float[] ReadAsMono(string path)
    {
        using var reader = new AudioFileReader(path);
        ISampleProvider source = reader.WaveFormat.SampleRate == SampleRate
            ? reader
            : new WdlResamplingSampleProvider(reader, SampleRate);

        var channels = source.WaveFormat.Channels;
        var mono = new List<float>(SampleRate);
        var buffer = new float[channels * 4096];
        int read;
        while ((read = source.Read(buffer)) > 0)
        {
            for (var frame = 0; frame + channels <= read; frame += channels)
            {
                var sum = 0f;
                for (var channel = 0; channel < channels; channel++)
                {
                    sum += buffer[frame + channel];
                }
                mono.Add(sum / channels);
            }
        }
        return [.. mono];
    }

    /// <summary>Prüft einen neuen Spurnamen. Liefert den Grund, wenn er nicht geht, sonst null.</summary>
    public string? ValidateTrackName(int track, string name)
    {
        var trimmed = name.Trim();
        if (trimmed.Length == 0)
        {
            return "Gib einen Namen ein.";
        }
        if (trimmed.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || trimmed.EndsWith('.'))
        {
            return "Der Name enthält Zeichen, die in Dateinamen nicht erlaubt sind.";
        }

        var cleaned = Capitalize(trimmed);
        for (var other = 0; other < _files.Length; other++)
        {
            if (other != track && string.Equals(TrackNames[other], cleaned, StringComparison.OrdinalIgnoreCase))
            {
                return "Diesen Namen hat schon eine andere Spur.";
            }
        }
        return null;
    }

    /// <summary>
    /// Benennt die Datei einer Spur um. Die Spur behält dabei ihre Position, denn die Patterns merken sich nur diese:
    /// Eine führende Zahl bleibt erhalten. Würde sich die Reihenfolge trotzdem ändern, bekommen alle Dateien eine Zahl.
    /// </summary>
    public void RenameTrack(int track, string name)
    {
        var folder = Path.GetDirectoryName(_files[track])!;
        var trimmed = name.Trim();
        var targets = _files.Select(Path.GetFileName).ToArray();
        var number = LeadingNumber().Match(Path.GetFileNameWithoutExtension(_files[track])).Value;
        if (number.Length > 0 && char.IsDigit(number[^1]))
        {
            number += " ";
        }
        targets[track] = $"{number}{trimmed}{Path.GetExtension(_files[track])}";

        var keepsOrder = targets.SequenceEqual(targets.OrderBy(file => Path.Combine(folder, file!), StringComparer.OrdinalIgnoreCase));
        if (!keepsOrder || (number.Length == 0 && char.IsDigit(trimmed[0])))
        {
            for (var index = 0; index < targets.Length; index++)
            {
                var plain = index == track ? trimmed : LeadingNumber().Replace(Path.GetFileNameWithoutExtension(_files[index]), string.Empty);
                targets[index] = $"{index + 1} {plain}{Path.GetExtension(_files[index])}";
            }
        }

        for (var index = 0; index < targets.Length; index++)
        {
            var target = Path.Combine(folder, targets[index]!);
            if (target != _files[index])
            {
                File.Move(_files[index], target);
                _files[index] = target;
                TrackNames[index] = TrackNameOf(target);
            }
        }
    }

    private static string TrackNameOf(string file) => CleanName(Path.GetFileNameWithoutExtension(file));

    private static string CleanName(string name)
    {
        var cleaned = LeadingNumber().Replace(name, string.Empty);
        return Capitalize(cleaned.Length > 0 ? cleaned : name);
    }

    /// <summary>Macht Wortanfänge groß und lässt den Rest unverändert, damit Namen wie „RnB“ erhalten bleiben.</summary>
    private static string Capitalize(string name)
    {
        var letters = name.ToCharArray();
        for (var index = 0; index < letters.Length; index++)
        {
            if (index == 0 || char.IsWhiteSpace(letters[index - 1]) || letters[index - 1] == '-')
            {
                letters[index] = char.ToUpperInvariant(letters[index]);
            }
        }
        return new string(letters);
    }

    [GeneratedRegex(@"^\d+[\s._-]*")]
    private static partial Regex LeadingNumber();
}
