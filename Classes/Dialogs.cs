using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Drums4me.Classes;

/// <summary>Alle Meldungen des Programms. Sie erscheinen ausschließlich als TaskDialog.</summary>
internal static class Dialogs
{
    private const string NAudioUrl = "https://naudio.github.io/NAudio/";
    private const string LicenseUrl = "https://www.gnu.org/licenses/gpl-3.0.html";
    private const string HydrogenKitsUrl = "https://github.com/hydrogen-music/hydrogen/tree/main/data/drumkits";

    public static void Error(IWin32Window owner, string heading, string text = "") => Show(owner, TaskDialogIcon.Error, heading, text);

    public static void Warning(IWin32Window owner, string heading, string text = "") => Show(owner, TaskDialogIcon.Warning, heading, text);

    public static void Info(IWin32Window owner, string heading, string text = "") => Show(owner, TaskDialogIcon.Information, heading, text);

    /// <summary>Fragt nach dem Speichern. Liefert Yes (speichern), No (verwerfen) oder Cancel (abbrechen).</summary>
    public static DialogResult AskSaveChanges(IWin32Window owner, string projectName, IReadOnlyList<string> changes)
    {
        var save = new TaskDialogButton("Speichern");
        var discard = new TaskDialogButton("Nicht speichern");
        var page = new TaskDialogPage
        {
            Caption = AppSettings.AppName,
            Heading = "Möchtest du die Änderungen speichern?",
            Text = ProjectChanges.Summarize(projectName, changes),
            AllowCancel = true,
            Buttons = { save, discard, TaskDialogButton.Cancel },
            DefaultButton = save,
        };

        var result = TaskDialog.ShowDialog(owner, page);
        if (result == save)
        {
            return DialogResult.Yes;
        }
        return result == discard ? DialogResult.No : DialogResult.Cancel;
    }

    /// <summary>Fragt, bevor ein Projekt in den Papierkorb wandert. Liefert true, wenn es weg soll.</summary>
    public static bool AskMoveToRecycleBin(IWin32Window owner, string projectName, bool isOpen)
    {
        var delete = new TaskDialogButton("In den Papierkorb");
        var page = new TaskDialogPage
        {
            Caption = AppSettings.AppName,
            Heading = $"„{projectName}“ in den Papierkorb verschieben?",
            Text = "Von dort kannst du das Projekt wiederherstellen."
                + (isOpen ? "\n\nEs ist gerade geöffnet und bleibt es, bis du es schließt. Speichern legt dann eine neue Datei an." : string.Empty),
            AllowCancel = true,
            Buttons = { delete, TaskDialogButton.Cancel },
            DefaultButton = delete,
        };
        return TaskDialog.ShowDialog(owner, page) == delete;
    }

    /// <summary>
    /// Die Anleitung für eigene Kits, ganz ohne Aufklappen. Fehlen alle Kits, steht das in der Überschrift.
    /// „Ordner öffnen“ lässt den Dialog offen, damit man beim Kopieren weiterlesen kann.
    /// </summary>
    public static void ShowKitGuide(IWin32Window owner, string kitFolder, bool noKitsFound = false)
    {
        var openFolder = new TaskDialogButton("Ordner öffnen") { AllowCloseDialog = false };
        openFolder.Click += (_, _) =>
        {
            try
            {
                Directory.CreateDirectory(kitFolder);
                Process.Start(new ProcessStartInfo(kitFolder) { UseShellExecute = true });
            }
            catch (Exception ex) when (ex is Win32Exception or IOException or UnauthorizedAccessException)
            {
                Debug.WriteLine($"Kit-Ordner nicht geöffnet: {ex.Message}");
            }
        };

        var page = new TaskDialogPage
        {
            Caption = AppSettings.AppName,
            Heading = noKitsFound ? "Es wurden keine Kits gefunden." : "So legst du eigene Kits an",
            Text = (noKitsFound ? "Installiere ein Kit-Paket oder lege ein eigenes Kit an.\n\n" : string.Empty)
                + "Ein Kit ist ein Ordner mit bis zu acht WAV-Dateien, eine Datei je Spur. Lege ihn im Ordner\n"
                + "für eigene Kits an:\n"
                + $"{kitFolder}\n"
                + "Klicke danach auf der Karte Einstellungen auf „Kits neu einlesen“.\n\n"
                + "Reihenfolge und Namen\n"
                + "‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾\n"
                + "Die Spuren folgen dem Alphabet der Dateinamen. Mit einer Zahl am Anfang legst du die\n"
                + "Reihenfolge fest, etwa „1 Crash.wav“.Die Zahl wird nicht angezeigt, der Rest des Namens wird\n"
                + "zum Spurnamen. Dasselbe gilt für die Ordnernamen der Kits.\n"
                + "Bei eigenen Kits kannst du eine Spur später noch umbenennen (Rechtsklick auf Namen im Raster).\n\n"
                + "Empfohlene Belegung\n"
                + "‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾\n"
                + "Damit ein Pattern nach einem Kit-Wechsel passt, sind alle mitgelieferten Kits so aufgebaut:\n"
                + "1 Becken, 2 bis 4 Toms oder Percussion, 5 offene Hi-Hat, 6 geschlossene Hi-Hat, 7 Snare, 8 Kick.\n\n"
                + "Klänge aufbereiten\n"
                + "‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾\n"
                + "Abtastrate und Bittiefe sind beliebig, drums4me rechnet selbst um und mischt Stereo zu Mono.\n"
                + "Schneide Stille am Anfang ab, sonst kommt der Schlag zu spät. Kürze lange Ausklänge von Becken\n"
                + "und offener Hi-Hat, denn drums4me stoppt eine klingende Hi-Hat nicht.\n\n"
                + "Mehrere Kits ordnen\n"
                + "‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾\n"
                + "Ein Ordner ohne eigene WAV-Dateien, aber mit Kit-Ordnern, ergibt eine sog. Kits-Sammlung.",
            Icon = TaskDialogIcon.None,
            EnableLinks = true,
            AllowCancel = true,
            Buttons = { openFolder, TaskDialogButton.Close },
            DefaultButton = TaskDialogButton.Close,
            Footnote = new TaskDialogFootnote
            {
                Text = $"Freie Klänge gibt es zum Beispiel bei <a href=\"{HydrogenKitsUrl}\">Hydrogen</a>. "
                    + $"Achte vor einer Weitergabe auf die Lizenz.",
            },
        };
        page.LinkClicked += Page_LinkClicked;
        TaskDialog.ShowDialog(owner, page);
    }

    public static void ShowAbout(Form owner)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);
        var copyright = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
        var page = new TaskDialogPage
        {
            Caption = $"Über {AppSettings.AppName}",
            Heading = $"{AppSettings.AppName} {version}",
            Text = $"Step-Sequencer und Metronom\n\n{copyright}\n\n"
                + $"Freie Software unter der <a href=\"{LicenseUrl}\">GNU General Public License, v3</a>.\n"
                + "Du darfst das Programm unter deren Bedingungen weitergeben und ändern. Es kommt ohne jede Gewährleistung.\n\n"
                + $"Verwendet <a href=\"{NAudioUrl}\">NAudio</a> von Mark Heath et al. (MIT-Lizenz).",
            Icon = owner.Icon is { } icon ? new TaskDialogIcon(icon) : TaskDialogIcon.Information,
            EnableLinks = true,
            AllowCancel = true,
            Buttons = { TaskDialogButton.OK },
            Footnote = "              Die Kits gehören nicht zum Programm.\n              Die Rechte liegen bei ihren Urhebern.",
        };
        page.LinkClicked += Page_LinkClicked;
        TaskDialog.ShowDialog(owner, page);
    }

    public static void ShowShortcuts(Form owner)
    {
        var page = new TaskDialogPage
        {
            Caption = AppSettings.AppName,
            Heading = "Tastenkombinationen",
            Text = "Wiedergabe und Tempo\n"
                + "Leertaste: Start und Stopp\n"
                + "Plus, Minus: Tempo um 1 BPM\n"
                + "Strg+Plus, Strg+Minus: Tempo um 5 BPM\n"
                + "Alt+Plus, Alt+Minus: Lautstärke um 5 %\n\n"
                + "Befehle\n"
                + "Strg+N: Neu\n"
                + "Strg+O oder Alt+D: Karte Datei, Esc: zurück zur Karte Start\n"
                + "Strg+F: Projekte suchen\n"
                + "In der Projektliste: F2 umbenennen, Entf in den Papierkorb, Alt+Eingabe Eigenschaften,\n"
                + "Strg+E im Editor bearbeiten, Strg+Umschalt+C Dateipfad kopieren\n"
                + "Strg+S: Speichern\n"
                + "Strg+Umschalt+S: Speichern unter\n"
                + "Strg+Z: Rückgängig\n"
                + "Strg+Y: Wiederholen\n"
                + "Strg+Entf: Pattern leeren\n"
                + "Strg+T: Tempo ermitteln (tippen oder mithören)\n"
                + "Strg+Komma: Karte Einstellungen\n"
                + "F1: diese Übersicht\n\n"
                + "Maus im Raster\n"
                + "Klick auf einen Spurnamen: Klang anhören\n"
                + "Rechtsklick auf einen Spurnamen: weitere Möglichkeiten\n"
                + "Strg + Ziehen an Volume oder Pitch: alle Spuren gemeinsam\n"
                + "Doppelklick auf einen Regler: Standardwert\n"
                + "Strg + Doppelklick auf Volume oder Pitch: Standardwert für alle Spuren",
            Icon = owner.Icon is { } icon ? new TaskDialogIcon(icon) : TaskDialogIcon.Information,
            AllowCancel = true,
            Buttons = { TaskDialogButton.OK },
            Footnote = "Beim Schreiben im Abfolge-Feld und in der Suche gehören Leertaste, Plus und Minus dem Text.",
        };
        TaskDialog.ShowDialog(owner, page);
    }

    private static void Show(IWin32Window owner, TaskDialogIcon icon, string heading, string text)
    {
        var page = new TaskDialogPage
        {
            Caption = AppSettings.AppName,
            Heading = heading,
            Text = text,
            Icon = icon,
            AllowCancel = true,
            Buttons = { TaskDialogButton.OK },
        };
        TaskDialog.ShowDialog(owner, page);
    }

    private static void Page_LinkClicked(object? sender, TaskDialogLinkClickedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo(e.LinkHref) { UseShellExecute = true });
        }
        catch (Exception ex) when (ex is Win32Exception or InvalidOperationException)
        {
            Debug.WriteLine($"Link nicht geöffnet: {ex.Message}");
        }
    }
}
