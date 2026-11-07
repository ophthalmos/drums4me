using Drums4me.Classes;
using System.Diagnostics;

namespace Drums4me.Forms;

/// <summary>Fenster zum Eintippen des Tempos über die Leertaste.</summary>
public partial class FrmTap : Form
{
    private const int WmKeyDown = 0x0100;
    private const long KeyRepeatFlag = 1L << 30;   // gesetzt, wenn eine Taste nur gedrückt gehalten wird

    private readonly TapTempo _tempo = new();

    public FrmTap()
    {
        InitializeComponent();
        ShowTempo();
    }

    /// <summary>Das ermittelte Tempo im Bereich, den das Programm unterstützt.</summary>
    public int Bpm => (int)Math.Round(Math.Clamp(_tempo.Bpm, Project.MinBpm, Project.MaxBpm));

    private static double Now => Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // Die Leertaste zählt nur als Schlag und darf keine Schaltfläche auslösen.
        if (keyData == Keys.Space)
        {
            if (msg.Msg == WmKeyDown && (msg.LParam.ToInt64() & KeyRepeatFlag) == 0)
            {
                RegisterTap();
            }
            return true;
        }

        // Enter übernimmt immer, auch wenn der Fokus auf einer anderen Schaltfläche liegt.
        if (keyData == Keys.Enter)
        {
            buttonApply.PerformClick();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void RegisterTap()
    {
        _tempo.Tap(Now);
        ShowTempo();

        labelBpm.BackColor = SystemColors.Highlight;
        labelBpm.ForeColor = SystemColors.HighlightText;
        flashTimer.Stop();
        flashTimer.Start();
    }

    private void ShowTempo()
    {
        labelBpm.Text = _tempo.HasTempo ? $"{Bpm} BPM" : "– BPM";
        buttonApply.Enabled = _tempo.HasTempo;
        labelStatus.Text = _tempo.TapCount switch
        {
            0 => "Noch kein Schlag",
            1 => "1 Schlag: Tippe im Takt weiter.",
            _ => $"{_tempo.TapCount} Schläge",
        };
    }

    private void FlashTimer_Tick(object? sender, EventArgs e)
    {
        flashTimer.Stop();
        labelBpm.BackColor = SystemColors.Control;
        labelBpm.ForeColor = SystemColors.ControlText;
    }

    private void PauseTimer_Tick(object? sender, EventArgs e)
    {
        if (_tempo.IsExpired(Now))
        {
            labelStatus.Text = "Pause erkannt: Der nächste Schlag beginnt neu.";
        }
    }

    private void ButtonReset_Click(object? sender, EventArgs e)
    {
        _tempo.Reset();
        ShowTempo();
    }
}
