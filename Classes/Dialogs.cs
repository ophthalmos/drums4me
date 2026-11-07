using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Drums4me.Classes;

/// <summary>Alle Meldungen des Programms. Sie erscheinen ausschließlich als TaskDialog.</summary>
internal static class Dialogs
{
    private const string NAudioUrl = "https://naudio.github.io/NAudio/";
    private const string LicenseUrl = "https://www.gnu.org/licenses/gpl-3.0.html";

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
                + "Strg+O: Karte Datei, Esc: zurück zur Karte Start\n"
                + "Strg+F: Projekte suchen\n"
                + "Strg+S: Speichern\n"
                + "Strg+Umschalt+S: Speichern unter\n"
                + "Strg+Z: Rückgängig\n"
                + "Strg+Y: Wiederholen\n"
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
