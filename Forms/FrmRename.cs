using Drums4me.Classes;

namespace Drums4me.Forms;

/// <summary>Fragt nach einem neuen Spurnamen. Die Prüfung liefert den Grund, wenn ein Name nicht geht.</summary>
public partial class FrmRename : Form
{
    private readonly Func<string, string?> _validate;

    public FrmRename(string currentName, Func<string, string?> validate)
    {
        InitializeComponent();
        Lng.Apply(this);
        _validate = validate;
        textBoxName.Text = currentName;
        textBoxName.SelectAll();
    }

    public string NewName => textBoxName.Text.Trim();

    private void TextBoxName_TextChanged(object? sender, EventArgs e)
    {
        var problem = _validate(textBoxName.Text);
        labelHint.Text = problem ?? Lng.T("Auch die Datei im Kit-Ordner bekommt diesen Namen.");
        labelHint.ForeColor = problem is null ? SystemColors.GrayText : Classes.Theme.Error;
        buttonOk.Enabled = problem is null;
    }
}
