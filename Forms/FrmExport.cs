using Drums4me.Classes;

namespace Drums4me.Forms;

/// <summary>
/// Fragt vor dem Speichern als Audiodatei nach Länge und Format und erklärt, was in die Datei kommt.
/// Das hängt vom Projekt ab: ein Pattern, eine Abfolge mit Wiederholung oder ein Stück mit Ende.
/// Gerechnet und geschrieben wird erst danach im Hauptfenster.
/// </summary>
public partial class FrmExport : Form
{
    private readonly Project _project;

    internal FrmExport(Project project, int rememberedBars, string? rememberedFormat, string suggestedName, string folder)
    {
        InitializeComponent();
        Lng.Apply(this);
        exportFileDialog.Title = Lng.T(exportFileDialog.Title);
        exportFileDialog.FileName = suggestedName;
        exportFileDialog.InitialDirectory = folder;
        _project = project;

        labelHint.Text = Explanation();
        numericBars.Maximum = AudioExport.MaxBars(project);
        numericBars.Value = Math.Clamp(AudioExport.DefaultBars(project, rememberedBars), numericBars.Minimum, numericBars.Maximum);
        radioFlac.Checked = rememberedFormat == AudioExport.FlacExtension;
        radioMp3.Checked = rememberedFormat == AudioExport.Mp3Extension;
        ShowDuration();
    }

    /// <summary>So viele Takte sollen in die Datei.</summary>
    public int Bars => (int)numericBars.Value;

    /// <summary>Das gewählte Format als Dateiendung mit Punkt.</summary>
    public string Format => radioMp3.Checked ? AudioExport.Mp3Extension : radioFlac.Checked ? AudioExport.FlacExtension : AudioExport.WaveExtension;

    /// <summary>Die gewählte Datei, gültig nach „Speichern“.</summary>
    public string FilePath { get; private set; } = string.Empty;

    /// <summary>Was gespeichert wird und wie die Datei endet, passend zum Projekt.</summary>
    private string Explanation()
    {
        var round = _project.ActiveSequence.Length;
        var bars = round == 1 ? Lng.T("1 Takt") : string.Format(Lng.T("{0} Takte"), round);
        var loopEnd = Lng.T("Die Datei endet genau auf der Taktgrenze. Was darüber hinaus klingt, etwa ein Becken, steht schon am Anfang: So klingt sie beim Wiederholen wie das laufende Programm.");
        if (round == 0)
        {
            return string.Format(Lng.T("Gespeichert wird Pattern {0}, so viele Takte hintereinander, wie du angibst."), _project.ActivePattern + 1) + " " + loopEnd;
        }
        return _project.SequenceRepeat
            ? string.Format(Lng.T("Gespeichert wird die Abfolge von Anfang an. Ein Durchgang hat {0}, mit mehr Takten läuft sie mehrmals durch."), bars) + " " + loopEnd
            : string.Format(Lng.T("Gespeichert wird die Abfolge von Anfang an, sie hat {0}. Nach dem letzten Takt klingt das Stück aus. Mit weniger Takten endet es früher."), bars);
    }

    private void NumericBars_ValueChanged(object? sender, EventArgs e) => ShowDuration();

    /// <summary>Die Dauer hilft beim Abschätzen, etwa für eine Übungsdatei von drei Minuten. Zu lange Dateien lassen sich nicht speichern.</summary>
    private void ShowDuration()
    {
        var duration = AudioExport.DurationOf(_project, Bars);
        var tooLong = duration.TotalMinutes > AudioExport.MaxMinutes;
        var clock = $"{(int)duration.TotalMinutes}:{duration.Seconds:00}";
        labelDuration.Text = tooLong
            ? string.Format(Lng.T("Dauer: {0}. Mehr als {1:F0} Minuten gehen nicht."), clock, AudioExport.MaxMinutes)
            : string.Format(Lng.T("Dauer: {0}"), clock);
        labelDuration.ForeColor = tooLong ? Theme.Error : SystemColors.GrayText;
        buttonSave.Enabled = !tooLong;
    }

    private void ButtonSave_Click(object? sender, EventArgs e)
    {
        var format = Format;
        exportFileDialog.DefaultExt = format[1..];
        exportFileDialog.Filter = format switch
        {
            AudioExport.Mp3Extension => Lng.T("MP3-Datei (*.mp3)|*.mp3"),
            AudioExport.FlacExtension => Lng.T("FLAC-Datei (*.flac)|*.flac"),
            _ => Lng.T("WAV-Datei (*.wav)|*.wav"),
        };
        exportFileDialog.FileName = Path.GetFileNameWithoutExtension(exportFileDialog.FileName);
        if (exportFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;   // zurück zu Länge und Format
        }

        // Das Format bestimmt die Wahl hier, auch wenn im Dateidialog eine andere Endung eingetippt wurde.
        FilePath = Path.ChangeExtension(exportFileDialog.FileName, format);
        DialogResult = DialogResult.OK;
    }
}
