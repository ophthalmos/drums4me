using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    /// <summary>Lädt die letzte Datei beim Programmstart. Vorhandene Einstellungsdateien kennen den Wert noch nicht, deshalb ist er voreingestellt.</summary>
    public bool OpenLastFile { get; set; } = true;

    /// <summary>Der selbst gewählte Ordner für Projekte. Leer bedeutet den Standardordner unter „Dokumente“.</summary>
    public string? ProjectFolder { get; set; }

    /// <summary>Das Ausgabegerät für die Wiedergabe. Leer bedeutet den Windows-Standard.</summary>
    public string? AudioDeviceId { get; set; }

    /// <summary>Der Name dazu, damit die Auswahl ihn auch zeigen kann, wenn das Gerät gerade nicht angeschlossen ist.</summary>
    public string? AudioDeviceName { get; set; }

    /// <summary>Kits, die nicht in der Auswahl auf der Karte „Start“ erscheinen sollen. Projekte mit einem davon klingen trotzdem richtig.</summary>
    public List<string>? HiddenKits { get; set; }

    /// <summary>Die Spaltenbreiten der Projektliste in Pixeln.</summary>
    public Dictionary<string, int>? ColumnWidths { get; set; }

    /// <summary>Größe und Lage des Hauptfensters beim letzten Beenden. Fehlt der Wert, startet es mittig in Standardgröße.</summary>
    public WindowPlacement? Window { get; set; }

    /// <summary>Lässt das Raster zu jedem neuen Takt kurz aufleuchten.</summary>
    public bool FlashOnBar { get; set; }

    /// <summary>Hell, dunkel oder wie Windows. Gilt ab dem nächsten Programmstart, denn sie muss vor dem ersten Fenster feststehen.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter<SystemColorMode>))]
    public SystemColorMode ColorMode { get; set; } = SystemColorMode.System;

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

/// <summary>Die Lage gilt für das nicht maximierte Fenster, damit es nach dem Verkleinern wieder dort steht.</summary>
internal sealed record WindowPlacement(int X, int Y, int Width, int Height, bool Maximized);
