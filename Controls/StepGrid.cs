using Drums4me.Classes;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Drums4me.Controls;

/// <summary>
/// Zeichnet das Raster mit Spurnamen, Schritten und den Reglern für Lautstärke, Panorama und Tonhöhe.
/// Klicken schaltet einen Schritt um, Ziehen überträgt diesen Zustand auf weitere Schritte.
/// Ein Doppelklick auf einen Regler stellt seinen Standardwert wieder her.
/// </summary>
public sealed class StepGrid : Control
{
    private const int FaderCount = 3;
    private const int VolumeFader = 0;
    private const int PanFader = 1;
    private const int PitchFader = 2;
    private static readonly string[] FaderTitles = ["Volume", "Pan", "Pitch"];

    private static readonly Color CellOffA = Color.FromArgb(232, 234, 238);
    private static readonly Color CellOffB = Color.FromArgb(218, 221, 227);
    private static readonly Color CellOn = Color.FromArgb(38, 110, 190);
    private static readonly Color PlayheadOff = Color.FromArgb(255, 222, 176);
    private static readonly Color PlayheadOn = Color.FromArgb(226, 106, 26);
    private static readonly Color FaderLine = Color.FromArgb(200, 203, 209);
    private static readonly Color BarFlashColor = Color.FromArgb(255, 176, 96);

    private string[] _trackNames = new string[Project.TrackCount];
    private TrackSettings[] _tracks = Project.CreateDefaultTracks();
    private bool[][] _steps = Project.CreateEmptySteps();
    private Meter _meter = Meter.FourFour;
    private int _playStep = -1;
    private bool _barFlash;

    private enum DragMode { None, Paint, Fader }

    private DragMode _dragMode;
    private bool _paintValue;
    private int _dragTrack;
    private int _dragFader;

    public StepGrid()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
    }

    /// <summary>Wird ausgelöst, wenn der Benutzer einen Schritt oder Regler geändert hat.</summary>
    public event EventHandler? Edited;

    /// <summary>Wird am Ende eines Zugs mit der Maus ausgelöst. Ein ganzer Zug zählt als eine Änderung.</summary>
    public event EventHandler? EditCompleted;

    /// <summary>Läuft gerade ein Zug mit der Maus?</summary>
    internal bool IsEditing => _dragMode != DragMode.None;

    /// <summary>Der gerade spielende Schritt, -1 wenn nichts läuft.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal int PlayStep
    {
        get => _playStep;
        set
        {
            if (_playStep != value)
            {
                InvalidateStepColumn(_playStep);
                _playStep = value;
                InvalidateStepColumn(_playStep);
            }
        }
    }

    /// <summary>Färbt den Hintergrund ein, solange der Blitz beim Taktwechsel leuchtet.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal bool BarFlash
    {
        get => _barFlash;
        set
        {
            if (_barFlash != value)
            {
                _barFlash = value;
                Invalidate();
            }
        }
    }

    internal void Bind(string[] trackNames, TrackSettings[] tracks, bool[][] steps, Meter meter)
    {
        _trackNames = trackNames;
        _tracks = tracks;
        _steps = steps;
        _meter = meter;
        Invalidate();
    }

    // ==================== Aufteilung ====================

    private int Padding8 => LogicalToDeviceUnits(8);
    private int NameWidth => LogicalToDeviceUnits(104);
    private int HeaderHeight => LogicalToDeviceUnits(26);
    private int GroupGap => LogicalToDeviceUnits(7);
    private int FaderWidth => LogicalToDeviceUnits(92);
    private int FaderGap => LogicalToDeviceUnits(8);
    private int ValueWidth => LogicalToDeviceUnits(30);

    private float RowHeight => Math.Max(1f, (ClientSize.Height - HeaderHeight - Padding8) / (float)Project.TrackCount);

    private int GridLeft => Padding8 + NameWidth;

    private int GridRight => ClientSize.Width - Padding8 - (FaderCount * (FaderWidth + FaderGap));

    private float StepWidth => Math.Max(1f, (GridRight - GridLeft - ((_meter.BeatCount - 1f) * GroupGap)) / _meter.Steps);

    private RectangleF StepRect(int track, int step)
    {
        var inset = LogicalToDeviceUnits(2);
        var left = GridLeft + (step * StepWidth) + (_meter.BeatOf[step] * GroupGap);
        var top = HeaderHeight + (track * RowHeight);
        return new RectangleF(left + inset, top + inset, StepWidth - (2 * inset), RowHeight - (2 * inset));
    }

    private Rectangle FaderColumn(int track, int fader)
    {
        var left = GridRight + FaderGap + (fader * (FaderWidth + FaderGap));
        var top = (int)(HeaderHeight + (track * RowHeight));
        return new Rectangle(left, top, FaderWidth, (int)RowHeight);
    }

    private Rectangle FaderSlide(int track, int fader)
    {
        var column = FaderColumn(track, fader);
        var knob = LogicalToDeviceUnits(5);
        return new Rectangle(column.Left + knob, column.Top, column.Width - ValueWidth - (2 * knob), column.Height);
    }

    private void InvalidateStepColumn(int step)
    {
        if (step >= 0 && step < _meter.Steps)
        {
            var rect = StepRect(0, step);
            Invalidate(new Rectangle((int)rect.Left - 2, HeaderHeight, (int)rect.Width + 4, ClientSize.Height - HeaderHeight));
        }
    }

    // ==================== Zeichnen ====================

    protected override void OnPaint(PaintEventArgs e)
    {
        var graphics = e.Graphics;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        if (_barFlash)
        {
            graphics.Clear(BarFlashColor);
        }
        DrawHeader(graphics);

        for (var track = 0; track < Project.TrackCount; track++)
        {
            var nameRect = new Rectangle(Padding8, (int)(HeaderHeight + (track * RowHeight)), NameWidth - Padding8, (int)RowHeight);
            TextRenderer.DrawText(graphics, _trackNames[track] ?? string.Empty, Font, nameRect, ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            for (var step = 0; step < _meter.Steps; step++)
            {
                DrawStep(graphics, track, step);
            }
            for (var fader = 0; fader < FaderCount; fader++)
            {
                DrawFader(graphics, track, fader);
            }
        }
    }

    private void DrawHeader(Graphics graphics)
    {
        for (var step = 0; step < _meter.Steps; step++)
        {
            var cell = StepRect(0, step);
            var rect = new Rectangle((int)cell.Left, 0, (int)cell.Width, HeaderHeight);
            var isBeat = _meter.IsBeatStart[step];
            TextRenderer.DrawText(graphics, isBeat ? (_meter.BeatOf[step] + 1).ToString() : "·", Font, rect,
                isBeat ? ForeColor : SystemColors.GrayText, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        for (var fader = 0; fader < FaderCount; fader++)
        {
            var column = FaderColumn(0, fader);
            var rect = new Rectangle(column.Left, 0, column.Width, HeaderHeight);
            TextRenderer.DrawText(graphics, FaderTitles[fader], Font, rect, SystemColors.GrayText, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }
    }

    private void DrawStep(Graphics graphics, int track, int step)
    {
        var on = _steps[track][step];
        Color color;
        if (step == _playStep)
        {
            color = on ? PlayheadOn : PlayheadOff;
        }
        else if (on)
        {
            color = CellOn;
        }
        else
        {
            color = _meter.GroupOf[step] % 2 == 0 ? CellOffA : CellOffB;
        }

        using var brush = new SolidBrush(color);
        var radius = LogicalToDeviceUnits(3);
        graphics.FillRoundedRectangle(brush, StepRect(track, step), new SizeF(radius, radius));
    }

    private void DrawFader(Graphics graphics, int track, int fader)
    {
        var slide = FaderSlide(track, fader);
        var centerY = slide.Top + (slide.Height / 2);
        var lineHeight = LogicalToDeviceUnits(3);
        var knobWidth = LogicalToDeviceUnits(8);
        var knobHeight = Math.Min(LogicalToDeviceUnits(16), slide.Height - 4);
        var knobX = slide.Left + (GetFaderValue(track, fader) * slide.Width);

        using (var line = new SolidBrush(FaderLine))
        {
            graphics.FillRectangle(line, slide.Left, centerY - (lineHeight / 2f), slide.Width, lineHeight);
        }
        using (var knob = new SolidBrush(CellOn))
        {
            graphics.FillRoundedRectangle(knob, new RectangleF(knobX - (knobWidth / 2f), centerY - (knobHeight / 2f), knobWidth, knobHeight),
                new SizeF(2f, 2f));
        }

        var column = FaderColumn(track, fader);
        var valueRect = new Rectangle(column.Right - ValueWidth, column.Top, ValueWidth, column.Height);
        TextRenderer.DrawText(graphics, FaderText(track, fader), Font, valueRect, SystemColors.GrayText,
            TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }

    // ==================== Reglerwerte ====================

    private float GetFaderValue(int track, int fader)
    {
        var settings = _tracks[track];
        return fader switch
        {
            VolumeFader => settings.Volume,
            PanFader => (settings.Pan + 1f) / 2f,
            _ => (settings.Pitch - TrackSettings.MinPitch) / (float)(TrackSettings.MaxPitch - TrackSettings.MinPitch),
        };
    }

    private void SetFaderValue(int track, int fader, float value)
    {
        var settings = _tracks[track];
        switch (fader)
        {
            case VolumeFader:
                settings.Volume = value;
                break;
            case PanFader:
                var pan = (value * 2f) - 1f;
                settings.Pan = Math.Abs(pan) < 0.05f ? 0f : pan;   // rastet in der Mitte ein
                break;
            case PitchFader:
                settings.Pitch = (int)MathF.Round(TrackSettings.MinPitch + (value * (TrackSettings.MaxPitch - TrackSettings.MinPitch)));
                break;
        }
    }

    private void ResetFader(int track, int fader)
    {
        var settings = _tracks[track];
        switch (fader)
        {
            case VolumeFader:
                settings.Volume = TrackSettings.DefaultVolume;
                break;
            case PanFader:
                settings.Pan = 0f;
                break;
            case PitchFader:
                settings.Pitch = 0;
                break;
        }
    }

    private string FaderText(int track, int fader)
    {
        var settings = _tracks[track];
        switch (fader)
        {
            case VolumeFader:
                return ((int)MathF.Round(settings.Volume * 100f)).ToString();
            case PanFader:
                var pan = (int)MathF.Round(settings.Pan * 100f);
                return pan == 0 ? "M" : pan < 0 ? $"L{-pan}" : $"R{pan}";
            default:
                return settings.Pitch > 0 ? $"+{settings.Pitch}" : settings.Pitch.ToString();
        }
    }

    // ==================== Maus ====================

    private bool HitStep(Point point, out int track, out int step)
    {
        track = (int)((point.Y - HeaderHeight) / RowHeight);
        step = -1;
        if (point.Y < HeaderHeight || track >= Project.TrackCount)
        {
            return false;
        }

        for (var candidate = 0; candidate < _meter.Steps; candidate++)
        {
            if (StepRect(track, candidate).Contains(point))
            {
                step = candidate;
                return true;
            }
        }
        return false;
    }

    private bool HitFader(Point point, out int track, out int fader)
    {
        track = (int)((point.Y - HeaderHeight) / RowHeight);
        fader = -1;
        if (point.Y < HeaderHeight || track >= Project.TrackCount)
        {
            return false;
        }

        for (var candidate = 0; candidate < FaderCount; candidate++)
        {
            if (FaderColumn(track, candidate).Contains(point))
            {
                fader = candidate;
                return true;
            }
        }
        return false;
    }

    private void SetStep(int track, int step, bool value)
    {
        if (_steps[track][step] != value)
        {
            _steps[track][step] = value;
            Invalidate(Rectangle.Ceiling(StepRect(track, step)));
            Edited?.Invoke(this, EventArgs.Empty);
        }
    }

    private void DragFaderTo(int x)
    {
        var slide = FaderSlide(_dragTrack, _dragFader);
        var value = Math.Clamp((x - slide.Left) / (float)Math.Max(1, slide.Width), 0f, 1f);
        SetFaderValue(_dragTrack, _dragFader, value);
        Invalidate(FaderColumn(_dragTrack, _dragFader));
        Edited?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Focus();   // beendet eine laufende Eingabe im Abfolge-Feld
        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        if (HitStep(e.Location, out var track, out var step))
        {
            _dragMode = DragMode.Paint;
            _paintValue = !_steps[track][step];
            SetStep(track, step, _paintValue);
        }
        else if (HitFader(e.Location, out track, out var fader))
        {
            if (e.Clicks == 2)
            {
                ResetFader(track, fader);
                Invalidate(FaderColumn(track, fader));
                Edited?.Invoke(this, EventArgs.Empty);
                return;
            }

            _dragMode = DragMode.Fader;
            _dragTrack = track;
            _dragFader = fader;
            DragFaderTo(e.X);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_dragMode == DragMode.Paint && HitStep(e.Location, out var track, out var step))
        {
            SetStep(track, step, _paintValue);
        }
        else if (_dragMode == DragMode.Fader)
        {
            DragFaderTo(e.X);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        EndEdit();
    }

    protected override void OnMouseCaptureChanged(EventArgs e)
    {
        base.OnMouseCaptureChanged(e);
        EndEdit();   // etwa, wenn ein anderes Fenster die Maus übernimmt
    }

    private void EndEdit()
    {
        if (_dragMode != DragMode.None)
        {
            _dragMode = DragMode.None;
            EditCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
