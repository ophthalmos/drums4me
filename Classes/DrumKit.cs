using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Drums4me.Classes;

/// <summary>
/// Ein Schlagzeug-Kit aus einem Unterordner des Kit-Ordners. Die ersten acht WAV-Dateien (nach Namen sortiert)
/// bilden die Spuren. Führende Ziffern bestimmen nur die Reihenfolge und erscheinen nicht im Namen.
/// </summary>
internal sealed partial class DrumKit
{
    public const int SampleRate = 44100;

    private readonly string[] _files;
    private readonly float[][] _samples = new float[Project.TrackCount][];
    private bool _loaded;

    private DrumKit(string name, string[] files)
    {
        Name = name;
        _files = files;
        TrackNames = new string[Project.TrackCount];
        for (var track = 0; track < Project.TrackCount; track++)
        {
            TrackNames[track] = track < files.Length ? CleanName(Path.GetFileNameWithoutExtension(files[track])) : "–";
            _samples[track] = [];
        }
    }

    /// <summary>Stumme Rückfallebene, falls der Kit-Ordner leer ist.</summary>
    public static DrumKit Empty { get; } = new("(kein Kit)", []);

    public string Name { get; }

    public string[] TrackNames { get; }

    public float[] SamplesOf(int track) => _samples[track];

    public override string ToString() => Name;

    /// <summary>Liest mehrere Kit-Ordner. Ein gleichnamiges Kit aus einem späteren Ordner ersetzt das frühere.</summary>
    public static List<DrumKit> ScanFolders(params string[] roots)
    {
        var kits = new List<DrumKit>();
        foreach (var root in roots)
        {
            foreach (var kit in ScanFolder(root))
            {
                var existing = kits.FindIndex(other => string.Equals(other.Name, kit.Name, StringComparison.OrdinalIgnoreCase));
                if (existing >= 0)
                {
                    kits[existing] = kit;
                }
                else
                {
                    kits.Add(kit);
                }
            }
        }
        return kits;
    }

    public static List<DrumKit> ScanFolder(string root)
    {
        var kits = new List<DrumKit>();
        if (!Directory.Exists(root))
        {
            return kits;
        }

        foreach (var folder in Directory.GetDirectories(root).OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            var files = Directory.GetFiles(folder, "*.wav")
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .Take(Project.TrackCount)
                .ToArray();
            if (files.Length > 0)
            {
                kits.Add(new DrumKit(CleanName(Path.GetFileName(folder)), files));
            }
        }
        return kits;
    }

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

    private static string CleanName(string name)
    {
        var cleaned = LeadingNumber().Replace(name, string.Empty);
        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(cleaned.Length > 0 ? cleaned : name);
    }

    [GeneratedRegex(@"^\d+[\s._-]*")]
    private static partial Regex LeadingNumber();
}
