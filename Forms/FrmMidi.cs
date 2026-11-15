using Drums4me.Classes;

namespace Drums4me.Forms;

/// <summary>
/// Zeigt, was eine MIDI-Datei an Schlagzeug enthält, und lässt bis zu vier Takte wählen, die zu den Patterns
/// eines neuen Projekts werden. Die Vorschau zeigt den markierten Takt so, wie er im Raster stehen wird.
/// </summary>
public partial class FrmMidi : Form
{
    private static readonly bool Dark = Application.IsDarkModeEnabled;
    private static readonly Color CellOff = Dark ? Color.FromArgb(58, 61, 68) : Color.FromArgb(226, 229, 234);
    private static readonly Color CellOn = Dark ? Color.FromArgb(74, 146, 222) : Color.FromArgb(38, 110, 190);

    private readonly MidiImport _midi;
    private readonly string[] _trackNames;
    private bool _filling;

    internal FrmMidi(MidiImport midi, DrumKit kit)
    {
        InitializeComponent();
        Lng.Apply(this);
        _midi = midi;
        _trackNames = kit.TrackNames;
        labelInfo.Text = Summary();
        labelMapping.Text = MappingText(kit.Name);

        _filling = true;
        var suggested = midi.Suggested;
        foreach (var bar in midi.Bars)
        {
            listBars.Items.Add(new ListViewItem([string.Format(Lng.T("Takt {0}"), bar.Number), bar.Meter.Name, $"{bar.Count}×"])
            {
                Tag = bar,
                Checked = suggested.Contains(bar),
            });
        }
    }

    /// <summary>
    /// Erst jetzt gelten Häkchen als Eingaben: Beim Aufbau des Fensters meldet die Liste auch die vorgeschlagenen Häkchen,
    /// und die Sperre für ein fünftes nahm dabei schon einmal einen Vorschlag wieder weg.
    /// </summary>
    private void FrmMidi_Shown(object? sender, EventArgs e)
    {
        _filling = false;
        var first = listBars.CheckedItems[0];
        first.Selected = true;
        first.Focused = true;
        first.EnsureVisible();
        ShowChoice();
    }

    /// <summary>Die gewählten Takte in der Reihenfolge der Datei.</summary>
    internal IReadOnlyList<MidiBar> Chosen => [.. listBars.CheckedItems.Cast<ListViewItem>().Select(item => (MidiBar)item.Tag!).OrderBy(bar => bar.Number)];

    private string Summary()
    {
        var count = _midi.Bars.Sum(bar => bar.Count);
        var bars = count == 1 ? Lng.T("1 Takt") : string.Format(Lng.T("{0} Takte"), count);
        var text = string.Format(Lng.T("„{0}“: {1} BPM, {2} mit Schlagzeug, {3} davon verschieden."), _midi.Name, _midi.Bpm, bars, _midi.Bars.Count);
        if (_midi.Swing == SwingFeel.Eighths)
        {
            text += " " + Lng.T("Der Groove ist triolisch gespielt, deshalb wird Swing-Achtel eingeschaltet.");
        }
        return text + " " + Lng.T("Wähle bis zu vier Takte, sie werden zu den Patterns 1 bis 4 eines neuen Projekts.");
    }

    /// <summary>Welcher Klang der Datei auf welcher Spur landet, und was das Kit nicht spielen kann.</summary>
    private string MappingText(string kitName)
    {
        var used = _midi.Mappings.Where(map => map.Track is not null).GroupBy(map => map.Track)
            .Select(track => $"{track.Key}: {string.Join(", ", track.Select(map => map.Instrument))}");
        var text = string.Format(Lng.T("Spuren von „{0}“:"), kitName) + " " + string.Join(" · ", used);
        var dropped = _midi.Mappings.Where(map => map.Track is null).Select(map => map.Instrument).ToList();
        if (dropped.Count > 0)
        {
            text += "\n" + Lng.T("Nicht übernommen, weil das Kit dafür keine Spur hat:") + " " + string.Join(", ", dropped);
        }
        return text;
    }

    /// <summary>Mehr als vier Takte passen nicht in ein Projekt, ein fünftes Häkchen wird deshalb nicht gesetzt.</summary>
    private void ListBars_ItemCheck(object? sender, ItemCheckEventArgs e)
    {
        if (!_filling && e.NewValue == CheckState.Checked && listBars.CheckedItems.Count >= MidiImport.MaxBars)
        {
            e.NewValue = CheckState.Unchecked;
            labelChoice.Text = Lng.T("Mehr als vier Takte passen nicht in ein Projekt. Nimm zuerst bei einem anderen das Häkchen weg.");
            labelChoice.ForeColor = Theme.Error;
        }
    }

    private void ListBars_ItemChecked(object? sender, ItemCheckedEventArgs e)
    {
        if (!_filling)
        {
            ShowChoice();
        }
    }

    private void ListBars_SelectedIndexChanged(object? sender, EventArgs e) => panelPreview.Invalidate();

    private void ShowChoice()
    {
        var chosen = Chosen;
        var covered = chosen.Sum(bar => bar.Count);
        var text = string.Format(Lng.T("Gewählt: {0} von höchstens {1}."), chosen.Count, MidiImport.MaxBars);
        if (chosen.Count > 0 && _midi.CanFollowFile(chosen))
        {
            text += " " + Lng.T("Die Abfolge spielt die Takte so wie die Datei.");
        }
        else if (chosen.Count > 0 && _midi.Bars.Count > chosen.Count)
        {
            text += " " + string.Format(Lng.T("Sie decken {0} der {1} Takte mit Schlagzeug ab."), covered, _midi.Bars.Sum(bar => bar.Count));
        }
        labelChoice.Text = text;
        labelChoice.ForeColor = SystemColors.ControlText;
        buttonImport.Enabled = chosen.Count > 0;
    }

    /// <summary>Der markierte Takt als kleines Raster: je Spur eine Zeile, leise Schläge klein, betonte mit Akzentzeichen.</summary>
    private void PanelPreview_Paint(object? sender, PaintEventArgs e)
    {
        if (listBars.SelectedItems.Count == 0 || listBars.SelectedItems[0].Tag is not MidiBar bar)
        {
            return;
        }

        var graphics = e.Graphics;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        var nameWidth = LogicalToDeviceUnits(92);
        var beatGap = LogicalToDeviceUnits(5);
        var rowHeight = panelPreview.ClientSize.Height / (float)Project.TrackCount;
        var stepWidth = (panelPreview.ClientSize.Width - nameWidth - ((bar.Meter.BeatCount - 1) * beatGap)) / (float)bar.Meter.Steps;
        using var off = new SolidBrush(CellOff);
        using var on = new SolidBrush(CellOn);
        using var mark = new Pen(Color.White, LogicalToDeviceUnits(3) / 2f);
        for (var track = 0; track < Project.TrackCount; track++)
        {
            var top = track * rowHeight;
            var used = bar.Steps[track].Any(level => level != StepLevel.Off);
            TextRenderer.DrawText(graphics, _trackNames[track], Font, new Rectangle(0, (int)top, nameWidth, (int)rowHeight),
                used ? ForeColor : SystemColors.GrayText, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            for (var step = 0; step < bar.Meter.Steps; step++)
            {
                var cell = new RectangleF(nameWidth + (step * stepWidth) + (bar.Meter.BeatOf[step] * beatGap) + 1, top + 1, stepWidth - 2, rowHeight - 2);
                var level = bar.Steps[track][step];
                graphics.FillRectangle(level is StepLevel.Normal or StepLevel.Accent ? on : off, cell);
                if (level == StepLevel.Soft)
                {
                    graphics.FillRectangle(on, RectangleF.Inflate(cell, -cell.Width * 0.27f, -cell.Height * 0.27f));
                }
                else if (level == StepLevel.Accent)
                {
                    var size = Math.Min(cell.Width, cell.Height) * 0.3f;
                    var x = cell.Left + ((cell.Width - size) / 2f);
                    var y = cell.Top + (cell.Height / 2f);
                    graphics.DrawLines(mark, [new PointF(x, y - size), new PointF(x + size, y), new PointF(x, y + size)]);
                }
            }
        }
    }
}
