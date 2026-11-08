using Drums4me.Classes;
using System.ComponentModel;
using System.Diagnostics;

namespace Drums4me.Forms;

/// <summary>
/// Legt fest, welche Kits in der Auswahl auf der Karte „Start“ erscheinen. Die Reihenfolge ist die der Auswahl:
/// nach Sammlung, die eigenen Kits am Ende. Mindestens ein Kit muss angehakt bleiben.
/// </summary>
public partial class FrmKits : Form
{
    public FrmKits()
    {
        InitializeComponent();
    }

    /// <summary>Der Ordner für eigene Kits, für die Anleitung hinter dem Link.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal string OwnKitFolder { get; set; } = string.Empty;

    /// <summary>Die Namen der Kits ohne Haken.</summary>
    internal List<string> HiddenKits => [.. listKits.Items.Cast<ListViewItem>().Where(item => !item.Checked).Select(item => ((DrumKit)item.Tag!).Name)];

    /// <summary>
    /// Zeigt die Kits in der Reihenfolge der Auswahlliste, die Klänge daneben als Entscheidungshilfe.
    /// Die Sammlung steht in einer eigenen Spalte: Gruppenüberschriften der ListView wären im Dunkelmodus kaum lesbar.
    /// Ein Kit liegt bei den installierten oder bei den eigenen Kits. Den Ordner der markierten Zeile nennt der Ordnerknopf.
    /// Die Zeilen haben keinen eigenen Tooltip-Text: So zeigt die Liste bei zu schmaler Spalte den ganzen Inhalt der Zelle.
    /// </summary>
    internal void Fill(IReadOnlyList<DrumKit> kits, IEnumerable<string> hidden)
    {
        var hiddenNames = new HashSet<string>(hidden, StringComparer.OrdinalIgnoreCase);
        listKits.BeginUpdate();
        foreach (var kit in kits)
        {
            var item = new ListViewItem(kit.Name) { Tag = kit, Checked = !hiddenNames.Contains(kit.Name) };
            item.SubItems.Add(kit.IsOwn ? "eigen" : kit.Collection ?? string.Empty);
            item.SubItems.Add(string.Join(", ", kit.TrackNames.Where(name => name != DrumKit.NoTrackName)));
            listKits.Items.Add(item);
        }
        if (listKits.Items.Count > 0)
        {
            listKits.Items[0].Selected = true;
            listKits.Items[0].Focused = true;
        }
        listKits.EndUpdate();
        UpdateCount();
        UpdateFolderButton();
    }

    private void ListKits_SelectedIndexChanged(object? sender, EventArgs e) => UpdateFolderButton();

    private const string Ellipsis = "…";

    /// <summary>Der Knopf nennt den Ordner, den er öffnet. Ohne markierte Zeile gibt es nichts zu öffnen.</summary>
    private void UpdateFolderButton()
    {
        var folder = SelectedFolder;
        buttonOpenFolder.Enabled = folder is { Length: > 0 };
        buttonOpenFolder.Text = folder is { Length: > 0 } ? FitPathEnd(folder) : "Kein Kit markiert";

        // Ist der Pfad vorn gekürzt, steht er hier vollständig.
        toolTip.SetToolTip(buttonOpenFolder, folder is { Length: > 0 } ? folder : "Markiere ein Kit, um seinen Ordner zu öffnen.");
    }

    private void ButtonOpenFolder_SizeChanged(object? sender, EventArgs e) => UpdateFolderButton();

    /// <summary>
    /// Passt der Pfad nicht auf den Knopf, fällt vorn genau so viel weg wie nötig: Das Ende mit Sammlung und Kit
    /// sagt mehr als das Laufwerk. Gesucht wird der längste Rest, der samt Auslassungszeichen noch passt.
    /// </summary>
    private string FitPathEnd(string path)
    {
        // Geschützte Leerzeichen: Bei knappem Platz bricht der Knopf sonst an einem Leerzeichen um, und der Rest
        // des Namens verschwindet in einer unsichtbaren zweiten Zeile.
        path = path.Replace(' ', '\u00A0');
        var room = buttonOpenFolder.ClientSize.Width - buttonOpenFolder.Padding.Horizontal - (buttonOpenFolder.Image?.Width ?? 0) - LogicalToDeviceUnits(10);
        bool Fits(string text) => TextRenderer.MeasureText(text, buttonOpenFolder.Font).Width <= room;
        if (Fits(path))
        {
            return path;
        }

        // Je weiter vorn der Schnitt, desto länger der Text: kleinster Schnittpunkt, der noch passt
        var low = 1;
        var high = path.Length;
        while (low < high)
        {
            var middle = (low + high) / 2;
            if (Fits(Ellipsis + path[middle..]))
            {
                high = middle;
            }
            else
            {
                low = middle + 1;
            }
        }
        return Ellipsis + path[low..];
    }

    private string? SelectedFolder => listKits.SelectedItems.Count > 0 ? ((DrumKit)listKits.SelectedItems[0].Tag!).Folder : null;

    private void ButtonOpenFolder_Click(object? sender, EventArgs e)
    {
        if (SelectedFolder is not { Length: > 0 } folder)
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(folder) { UseShellExecute = true });
        }
        catch (Win32Exception ex)
        {
            Dialogs.Error(this, "Der Ordner ließ sich nicht öffnen.", ex.Message);
        }
    }

    private void LinkKitGuide_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) => Dialogs.ShowKitGuide(this, OwnKitFolder);

    private void ListKits_ItemChecked(object? sender, ItemCheckedEventArgs e) => UpdateCount();

    private void ButtonAll_Click(object? sender, EventArgs e) => SetAll(true);

    private void ButtonNone_Click(object? sender, EventArgs e) => SetAll(false);

    private void SetAll(bool check)
    {
        listKits.BeginUpdate();
        foreach (ListViewItem item in listKits.Items)
        {
            item.Checked = check;
        }
        listKits.EndUpdate();
        UpdateCount();
        listKits.Focus();
    }

    /// <summary>Ohne ein einziges Kit gäbe es nichts zu spielen, deshalb geht OK erst mit mindestens einem Haken.</summary>
    private void UpdateCount()
    {
        var total = listKits.Items.Count;
        var chosen = listKits.CheckedItems.Count;
        labelCount.Text = chosen == 0 ? "Hake mindestens ein Kit an." : $"{chosen} von {total} ausgewählt";
        buttonOk.Enabled = chosen > 0;
    }
}
