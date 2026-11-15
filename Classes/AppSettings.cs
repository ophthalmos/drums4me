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

    /// <summary>So viele Takte kommen ohne Abfolge in eine Audiodatei.</summary>
    public int ExportBars { get; set; } = 4;

    /// <summary>Das zuletzt gewählte Format für Audiodateien als Endung, etwa ".wav". Ohne Wert gilt WAV.</summary>
    public string? ExportFormat { get; set; }

    /// <summary>Der Ordner, in den zuletzt eine Audiodatei gespeichert wurde. Ohne Wert der Projektordner.</summary>
    public string? ExportFolder { get; set; }

    /// <summary>Der Ordner, aus dem zuletzt eine MIDI-Datei importiert wurde.</summary>
    public string? MidiFolder { get; set; }

    /// <summary>Lässt das Raster zu jedem neuen Takt kurz aufleuchten.</summary>
    public bool FlashOnBar { get; set; }

    /// <summary>Solange die Wiedergabe läuft, schaltet Windows den Bildschirm nicht ab.</summary>
    public bool KeepScreenOn { get; set; } = true;

    /// <summary>Lautstärke der Klicks beim Vorzählen in Prozent, bezogen auf die Gesamtlautstärke des Projekts.</summary>
    public int CountInVolume { get; set; } = 100;

    /// <summary>Das Fenster bleibt über anderen Programmen, etwa über Noten.</summary>
    public bool AlwaysOnTop { get; set; }

    /// <summary>Mit diesem Puffer läuft die Tonausgabe, wenn nichts anderes eingestellt ist.</summary>
    public const int DefaultOutputLatencyMs = 40;

    /// <summary>Puffer der Tonausgabe in Millisekunden. Größer hilft gegen Knackser, kleiner lässt den Ton schneller folgen.</summary>
    public int OutputLatencyMs { get; set; } = DefaultOutputLatencyMs;

    /// <summary>Hell, dunkel oder wie Windows. Gilt ab dem nächsten Programmstart, denn sie muss vor dem ersten Fenster feststehen.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter<SystemColorMode>))]
    public SystemColorMode ColorMode { get; set; } = SystemColorMode.System;

    /// <summary>Die Programmsprache, "de" oder "en". Ohne Vorgabe Deutsch nur bei deutschem Windows. Gilt ab dem nächsten Start.</summary>
    public string Language { get; set; } = Lng.DefaultCultureCode;

    /// <summary>Die zuletzt übernommene Sprachwahl des Setups, damit sie nur einmal pro Installation gilt (siehe ApplyInstallerDefaults).</summary>
    public string? InstallerLanguage { get; set; }

    public static AppSettings Load()
    {
        AppSettings settings;
        try
        {
            settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsFile)) ?? new AppSettings();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            settings = new AppSettings();
        }
        return ApplyInstallerDefaults(settings);
    }

    /// <summary>
    /// Übernimmt die Sprache, die im Setup gewählt wurde (Datei setup.default neben der EXE, Zeile „language=de“), genau einmal
    /// pro Installation: Erst wenn eine neue Installation einen anderen Wert hinterlegt, überschreibt er die Einstellung.
    /// Eine spätere Umstellung in den Einstellungen bleibt bis dahin erhalten. Gespeichert wird beim nächsten Save.
    /// </summary>
    private static AppSettings ApplyInstallerDefaults(AppSettings settings)
    {
        try
        {
            var marker = Path.Combine(AppContext.BaseDirectory, "setup.default");
            if (!File.Exists(marker))
            {
                return settings;
            }

            foreach (var line in File.ReadAllLines(marker))
            {
                var separator = line.IndexOf('=');
                if (separator <= 0)
                {
                    continue;
                }

                var key = line[..separator].Trim();
                var value = line[(separator + 1)..].Trim();
                if (key == "language" && (value is "de" or "en") && value != settings.InstallerLanguage)
                {
                    settings.InstallerLanguage = value;
                    settings.Language = value;
                }
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Ohne lesbare Vorgabe bleibt die bisherige Sprache.
        }
        return settings;
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
