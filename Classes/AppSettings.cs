using System.Diagnostics;
using System.Text.Json;

namespace Drums4me.Classes;

/// <summary>Einstellungen des Programms, getrennt von den Projektdateien. Fehler beim Lesen oder Schreiben sind unkritisch.</summary>
internal sealed class AppSettings
{
    public const string AppName = "drums4me";

    private static readonly string SettingsFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName, "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    /// <summary>Die zuletzt geöffnete oder gespeicherte Projektdatei.</summary>
    public string? LastFile { get; set; }

    /// <summary>Der selbst gewählte Ordner für Projekte. Leer bedeutet den Standardordner unter „Dokumente“.</summary>
    public string? ProjectFolder { get; set; }

    /// <summary>Lässt das Raster zu jedem neuen Takt kurz aufleuchten.</summary>
    public bool FlashOnBar { get; set; }

    public static AppSettings Load()
    {
        try
        {
            return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsFile)) ?? new AppSettings();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            return new AppSettings();
        }
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFile)!);
            File.WriteAllText(SettingsFile, JsonSerializer.Serialize(this, JsonOptions));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Das Projekt selbst ist davon nicht betroffen, beim nächsten Start fehlt nur der Vorschlag.
            Debug.WriteLine($"Einstellungen nicht gespeichert: {ex.Message}");
        }
    }
}
