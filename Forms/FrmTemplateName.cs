using Drums4me.Classes;

namespace Drums4me.Forms;

/// <summary>
/// Fragt nach dem Namen einer eigenen Vorlage. Gibt es den Namen schon, sagt der Hinweis, dass Speichern die alte
/// Vorlage ersetzt; eine zusätzliche Rückfrage braucht es dann nicht.
/// </summary>
public partial class FrmTemplateName : Form
{
    private readonly string _folder;

    internal FrmTemplateName(string suggestedName, string folder)
    {
        InitializeComponent();
        Lng.Apply(this);
        _folder = folder;
        textBoxName.Text = suggestedName;
        textBoxName.SelectAll();
    }

    /// <summary>Der Name ohne Dateiendung, gültig nach „Speichern“.</summary>
    internal string TemplateName => textBoxName.Text.Trim();

    private void TextBoxName_TextChanged(object? sender, EventArgs e)
    {
        var name = TemplateName;
        var problem = name.Length == 0 ? Lng.T("Gib einen Namen ein.")
            : name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || name.EndsWith('.') ? Lng.T("Der Name enthält Zeichen, die in Dateinamen nicht erlaubt sind.")
            : null;
        var path = Path.Combine(_folder, name + ProjectFiles.TemplateExtension);
        var exists = problem is null && File.Exists(path);
        if (exists && !CanReplace(path))
        {
            problem = Lng.T("Diese Vorlage lässt sich nicht ersetzen, etwa weil sie mit einem Paket installiert wurde. Wähle einen anderen Namen.");
        }
        labelHint.Text = problem ?? (exists
            ? Lng.T("Eine Vorlage mit diesem Namen gibt es schon. Speichern ersetzt sie.")
            : Lng.T("Sie erscheint danach unter „Vorlage öffnen“, das Projekt selbst bleibt, wie es ist."));
        labelHint.ForeColor = problem is null ? SystemColors.GrayText : Theme.Error;
        buttonSave.Enabled = problem is null;
    }

    /// <summary>Installierte Vorlagen und die anderer Benutzer gehören nicht dir, schreiben darfst du sie nicht.</summary>
    private static bool CanReplace(string path)
    {
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (IOException)
        {
            return true;   // etwa gerade geöffnet: Das meldet erst das Speichern
        }
    }
}
