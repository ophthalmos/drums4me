using System.Diagnostics;
using System.Text.Json;

namespace Drums4me.Classes;

/// <summary>Was die Projektübersicht über eine Datei wissen muss.</summary>
internal sealed record ProjectEntry(
    string FilePath, string Name, string Folder, DateTime Modified, int Bpm, string KitName, string[] Meters, bool KitMissing, bool Unreadable)
{
    public string MeterText => string.Join(", ", Meters);
}

/// <summary>Liest alle Projekte eines Ordners samt Unterordnern. Eine defekte Datei verhindert die Übersicht nicht.</summary>
internal static class ProjectIndex
{
    private static readonly EnumerationOptions Options = new() { RecurseSubdirectories = true, IgnoreInaccessible = true };

    public static List<ProjectEntry> Scan(string root, IEnumerable<string> kitNames)
    {
        var known = new HashSet<string>(kitNames, StringComparer.OrdinalIgnoreCase);
        var entries = new List<ProjectEntry>();
        if (!Directory.Exists(root))
        {
            return entries;
        }

        try
        {
            foreach (var file in Directory.EnumerateFiles(root, "*" + ProjectFiles.Extension, Options).Concat(Directory.EnumerateFiles(root, "*" + ProjectFiles.TemplateExtension, Options)))
            {
                entries.Add(Read(root, file, known));
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Etwa ein Netzlaufwerk, das während des Durchlaufs wegfällt. Was bis dahin gefunden wurde, bleibt nutzbar.
            Debug.WriteLine($"Ordner nicht vollständig gelesen: {ex.Message}");
        }
        return entries;
    }

    private static ProjectEntry Read(string root, string file, HashSet<string> known)
    {
        var name = Path.GetFileNameWithoutExtension(file);
        var folder = Path.GetRelativePath(root, Path.GetDirectoryName(file)!);
        folder = folder == "." ? string.Empty : folder;
        var modified = File.GetLastWriteTime(file);
        try
        {
            var project = Project.Load(file);
            var meters = project.Patterns.Where(pattern => pattern.Used)
                .Select(pattern => pattern.MeterName).Distinct().ToArray();
            return new ProjectEntry(file, name, folder, modified, project.Bpm, project.KitName, meters, !known.Contains(project.KitName), false);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or InvalidDataException)
        {
            return new ProjectEntry(file, name, folder, modified, 0, string.Empty, [], false, true);
        }
    }
}
