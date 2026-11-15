using Drums4me.Classes;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Drums4me.Forms;

/// <summary>
/// Ermittelt das Tempo auf zwei Wegen: Es hört mit, was Windows gerade abspielt, und nimmt getippte Schläge über die Leertaste an.
/// Getippte Schläge haben Vorrang. Vom Ton wird nichts gespeichert.
/// </summary>
public partial class FrmTempo : Form
{
    private const int WmKeyDown = 0x0100;
    private const long KeyRepeatFlag = 1L << 30;   // gesetzt, wenn eine Taste nur gedrückt gehalten wird
    private const double GoodEnoughSeconds = 20;
    private const double SettledSeconds = 3;   // so lange muss die Erkennung denselben Wert zeigen
    private const int SettledTaps = 4;         // und so viele Schläge braucht ein getipptes Tempo

    // KSDATAFORMAT_SUBTYPE_IEEE_FLOAT: So meldet Windows Gleitkomma-Ton im erweiterten Format.
    private static readonly Guid IeeeFloatSubFormat = new("00000003-0000-0010-8000-00aa00389b71");

    private readonly TapTempo _tap = new();
    private MMDevice? _device;
    private WasapiRecorder? _capture;
    private TempoDetector? _detector;
    private float[] _mono = [];
    private int _channels;
    private int _bytesPerSample;
    private bool _isFloat;
    private double _factor = 1;
    private double? _estimate;
    private bool _sure;
    private int _heardBpm;
    private double _heardSince;
    private string? _captureProblem;

    public FrmTempo()
    {
        InitializeComponent();
        Lng.Apply(this);
    }

    /// <summary>Das ermittelte Tempo im Bereich, den das Programm unterstützt.</summary>
    public int Bpm => (int)Math.Round(Math.Clamp((Source ?? 0) * _factor, Project.MinBpm, Project.MaxBpm));

    private double? Source => _tap.HasTempo ? _tap.Bpm : _estimate;

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

    // ==================== Mithören ====================

    private void FrmTempo_Shown(object? sender, EventArgs e)
    {
        try
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
            _capture = new WasapiRecorderBuilder().WithDevice(_device).WithSharedMode().WithLoopbackCapture().Build();
            var format = _capture.WaveFormat;
            _channels = format.Channels;
            _bytesPerSample = format.BitsPerSample / 8;
            _isFloat = format.Encoding == WaveFormatEncoding.IeeeFloat
                || (format is WaveFormatExtensible extensible && extensible.SubFormat == IeeeFloatSubFormat);
            if (_bytesPerSample is not (2 or 3 or 4) || (_isFloat && _bytesPerSample != 4))
            {
                throw new NotSupportedException(string.Format(Lng.T("Das Tonformat mit {0} Bit wird nicht unterstützt."), format.BitsPerSample));
            }

            _detector = new TempoDetector(format.SampleRate);
            _capture.DataAvailable += Capture_DataAvailable;
            _capture.RecordingStopped += Capture_RecordingStopped;
            _capture.StartRecording();
        }
        catch (Exception ex) when (ex is COMException or InvalidOperationException or ArgumentException or NotSupportedException)
        {
            // Das Tippen geht auch ohne Mithören, deshalb bleibt das Fenster offen.
            StopCapture();
            _captureProblem = ex.Message;
        }
        ShowState();
    }

    private void FrmTempo_FormClosed(object? sender, FormClosedEventArgs e)
    {
        updateTimer.Stop();
        StopCapture();
    }

    private void StopCapture()
    {
        if (_capture is { } capture)
        {
            _capture = null;
            capture.DataAvailable -= Capture_DataAvailable;
            capture.RecordingStopped -= Capture_RecordingStopped;
            capture.StopRecording();
            capture.Dispose();
        }
        _device?.Dispose();
        _device = null;
    }

    /// <summary>Läuft im Aufnahme-Thread. Der Ton wird zu Mono gemischt und nur ausgewertet.</summary>
    private void Capture_DataAvailable(ReadOnlySpan<byte> buffer, AudioClientBufferFlags flags, long devicePosition, long qpcPosition)
    {
        var frames = buffer.Length / (_channels * _bytesPerSample);
        if (_mono.Length < frames)
        {
            _mono = new float[frames];
        }

        var silent = (flags & AudioClientBufferFlags.Silent) != 0;
        for (var frame = 0; frame < frames; frame++)
        {
            var sum = 0f;
            for (var channel = 0; channel < _channels && !silent; channel++)
            {
                var at = ((frame * _channels) + channel) * _bytesPerSample;
                sum += _bytesPerSample switch
                {
                    4 when _isFloat => BitConverter.ToSingle(buffer.Slice(at, 4)),
                    4 => BitConverter.ToInt32(buffer.Slice(at, 4)) / 2147483648f,
                    3 => ((buffer[at] << 8) | (buffer[at + 1] << 16) | (buffer[at + 2] << 24)) / 2147483648f,
                    _ => BitConverter.ToInt16(buffer.Slice(at, 2)) / 32768f,
                };
            }
            _mono[frame] = sum / _channels;
        }
        _detector?.Add(_mono.AsSpan(0, frames));
    }

    private void Capture_RecordingStopped(object? sender, StoppedEventArgs e)
    {
        if (e.Exception is not null && IsHandleCreated)
        {
            BeginInvoke(() =>
            {
                _captureProblem = e.Exception.Message;
                ShowState();
            });
        }
    }

    private void UpdateTimer_Tick(object? sender, EventArgs e)
    {
        // Solange getippt wird, zählt die Erkennung nicht und braucht auch nicht zu rechnen.
        if (!_tap.HasTempo && _detector?.Estimate() is { } estimate)
        {
            _estimate = estimate.Bpm;
            _sure = estimate.IsSure;

            // Seit wann zeigt die Erkennung denselben Wert? Daran hängt, ob das Ergebnis als gefestigt gilt.
            var rounded = (int)Math.Round(estimate.Bpm);
            if (rounded != _heardBpm)
            {
                _heardBpm = rounded;
                _heardSince = Now;
            }
        }
        ShowState();
    }

    /// <summary>
    /// Steht das Ergebnis? Beim Mithören muss der Rhythmus eindeutig sein und der Wert sich einige Sekunden gehalten haben,
    /// beim Tippen braucht es ein paar Schläge. Bis dahin erscheint die Zahl grau.
    /// </summary>
    private bool IsSettled => _tap.HasTempo
        ? _tap.TapCount >= SettledTaps
        : _estimate is not null && _sure && Now - _heardSince >= SettledSeconds;

    // ==================== Tippen ====================

    private void RegisterTap()
    {
        // Wechselt die Quelle des Tempos, gilt eine Korrektur auf halb oder doppelt nicht mehr.
        var hadTempo = _tap.HasTempo;
        _tap.Tap(Now);
        if (_tap.HasTempo != hadTempo)
        {
            _factor = 1;
        }
        ShowState();

        labelBpm.BackColor = SystemColors.Highlight;
        labelBpm.ForeColor = SystemColors.HighlightText;
        flashTimer.Stop();
        flashTimer.Start();
    }

    private void FlashTimer_Tick(object? sender, EventArgs e)
    {
        flashTimer.Stop();
        labelBpm.BackColor = SystemColors.Control;
        ShowState();   // setzt auch die Schriftfarbe wieder passend zum Ergebnis
    }

    // ==================== Anzeige und Bedienung ====================

    private void ShowState()
    {
        var hasResult = Source is not null;
        var hadResult = buttonApply.Enabled;
        labelBpm.Text = hasResult ? $"{Bpm} BPM" : "– BPM";
        if (!flashTimer.Enabled)
        {
            labelBpm.ForeColor = hasResult && IsSettled ? SystemColors.ControlText : SystemColors.GrayText;
        }
        labelStatus.Text = StatusText();
        buttonApply.Enabled = hasResult;

        // Halb und Doppelt nur, solange das Ergebnis im Bereich des Programms bleibt. Sonst stünde dort ein abgeschnittener Wert.
        // Die Knöpfe nennen ihr Ergebnis, damit man die Alternative sieht, ohne sie erst auszuprobieren.
        var shown = (Source ?? 0) * _factor;
        buttonHalf.Enabled = hasResult && shown / 2 >= Project.MinBpm;
        buttonDouble.Enabled = hasResult && shown * 2 <= Project.MaxBpm;
        buttonHalf.Text = buttonHalf.Enabled ? string.Format(Lng.T("Halb ({0})"), (int)Math.Round(shown / 2)) : Lng.T("Halb");
        buttonDouble.Text = buttonDouble.Enabled ? string.Format(Lng.T("Doppelt ({0})"), (int)Math.Round(shown * 2)) : Lng.T("Doppelt");

        // Der Fokus zeigt die Hauptaktion: ohne Ergebnis „Abbrechen“ (das regelt die Tabulatorfolge), mit Ergebnis „Übernehmen“.
        if (hasResult && !hadResult && ActiveControl == buttonCancel)
        {
            buttonApply.Focus();
        }
    }

    private string StatusText()
    {
        if (_tap.TapCount > 0)
        {
            return _tap.IsExpired(Now) ? Lng.T("Pause erkannt: Der nächste Schlag beginnt neu.")
                : _tap.TapCount == 1 ? Lng.T("1 Schlag: Tippe im Takt weiter.")
                : string.Format(Lng.T("{0} Schläge: Das getippte Tempo gilt."), _tap.TapCount);
        }

        if (_captureProblem is not null)
        {
            return string.Format(Lng.T("Tippe mit der Leertaste, mithören geht nicht: {0}"), _captureProblem);
        }

        var heard = _detector?.HeardSeconds ?? 0;
        return heard <= 0 ? Lng.T("Warte auf Ton oder auf deinen ersten Schlag.")
            : _estimate is null ? string.Format(Lng.T("Höre zu: {0:F0} s. Das erste Ergebnis kommt gleich."), heard)
            : !_sure ? string.Format(Lng.T("Höre zu: {0:F0} s. Der Rhythmus ist nicht eindeutig, prüfe das Ergebnis."), heard)
            : heard < GoodEnoughSeconds ? string.Format(Lng.T("Höre zu: {0:F0} s. Das Ergebnis wird noch genauer."), heard)
            : string.Format(Lng.T("Höre zu: {0:F0} s. Stimmt nur das halbe oder doppelte Tempo? Dann korrigiere es."), heard);
    }

    private void ButtonHalf_Click(object? sender, EventArgs e)
    {
        _factor /= 2;
        ShowState();
    }

    private void ButtonDouble_Click(object? sender, EventArgs e)
    {
        _factor *= 2;
        ShowState();
    }

    private void ButtonReset_Click(object? sender, EventArgs e)
    {
        _tap.Reset();
        _detector?.Reset();
        _estimate = null;
        _sure = false;
        _heardBpm = 0;
        _factor = 1;
        ShowState();
    }
}
