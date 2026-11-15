using Drums4me.Classes;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Drums4me.Controls;

/// <summary>
/// Zeichnet das Raster mit Spurnamen, Schritten und den Reglern für Lautstärke, Panorama und Tonhöhe.
/// Klicken setzt einen Schritt oder löscht ihn, Ziehen überträgt das auf weitere Schritte.
/// Ein Rechtsklick wechselt die Stärke eines gesetzten Schritts: normal, betont, leise.
/// Ein Doppelklick auf einen Regler stellt seinen Standardwert wieder her.
/// Mit gedrückter Strg-Taste wirken Volume und Pitch auf alle Spuren gemeinsam, ihre Abstände bleiben dabei erhalten.
/// Klicks auf einen Spurnamen meldet das Raster nur, was daraus folgt, entscheidet das Formular.
/// Unter jedem Spurnamen sitzt ein Schalter, der die Spur stumm schaltet; mit Strg spielt nur diese Spur (Solo).
/// Ein Klick auf die Zahl einer Zählzeit markiert sie, Ziehen auf eine andere Zählzeit kopiert sie dorthin.
/// </summary>
public sealed class StepGrid : Control
{
    private const int FaderCount = 3;
    private const int VolumeFader = 0;
    private const int PanFader = 1;
    private const int PitchFader = 2;
    private static readonly string[] FaderTitles = ["Volume", "Pan", "Pitch"];
    private static readonly string AllTracksHint = Lng.T("Strg + Ziehen: alle Spuren gemeinsam. Doppelklick: Standardwert, mit Strg für alle Spuren.");
    private static readonly string TrackNameHint = Lng.T("Klick: Klang anhören. Rechtsklick: weitere Möglichkeiten.");
    private static readonly string StepHint = Lng.T("Klick: Schritt setzen oder löschen. Ziehen: mehrere Schritte auf einmal. Rechtsklick auf einen gesetzten Schritt: Stärke wechseln (normal, betont, leise).");
    private static readonly string CaptionHint = Lng.T("Das Pattern, das du gerade siehst. Läuft eine Abfolge, steht daneben, der wievielte Takt der Abfolge gerade spielt.");
    private static readonly string SwitchHint = Lng.T("Schaltet die Spur ein oder stumm. Strg + Klick: Nur diese Spur spielt, noch einmal: wieder alle.");
    private static readonly string BeatHint = Lng.T("Klick: Zählzeit markieren. Ziehen auf eine andere Zählzeit: dorthin kopieren. Rechtsklick: kopieren, einfügen, leeren.");
    private static readonly string[] FaderHints =
    [
        Lng.T("Lautstärke der Spur.") + " " + AllTracksHint,
        Lng.T("Panorama der Spur. Doppelklick: Mitte."),
        Lng.T("Tonhöhe der Spur.") + " " + AllTracksHint,
    ];

    // Die Darstellung steht beim Programmstart fest, bevor das erste Raster entsteht.
    private static readonly bool Dark = Application.IsDarkModeEnabled;
    private static readonly Color CellOffA = Dark ? Color.FromArgb(52, 55, 62) : Color.FromArgb(232, 234, 238);
    private static readonly Color CellOffB = Dark ? Color.FromArgb(66, 70, 78) : Color.FromArgb(218, 221, 227);
    private static readonly Color CellOn = Dark ? Color.FromArgb(74, 146, 222) : Color.FromArgb(38, 110, 190);
    private static readonly Color PlayheadOff = Dark ? Color.FromArgb(112, 84, 48) : Color.FromArgb(255, 222, 176);
    private static readonly Color PlayheadOn = Dark ? Color.FromArgb(240, 138, 54) : Color.FromArgb(226, 106, 26);
    private static readonly Color FaderLine = Dark ? Color.FromArgb(96, 100, 108) : Color.FromArgb(200, 203, 209);
    private static readonly Color BarFlashColor = Dark ? Color.FromArgb(122, 74, 26) : Color.FromArgb(255, 176, 96);
    private static readonly Color AccentMark = Color.White;   // das Zeichen „>“ auf betonten Schritten
    private static readonly Color CellMuted = Dark ? Color.FromArgb(104, 108, 116) : Color.FromArgb(172, 177, 186);   // gesetzte Schritte stummer Spuren
    private static readonly Color BeatMark = Dark ? Color.FromArgb(74, 146, 222) : Color.FromArgb(38, 110, 190);       // markierte Zählzeit
    private const float SoftInset = 0.27f;                     // so viel bleibt bei leisen Schritten an jedem Rand frei

    // Die Plakette mit dem Pattern: in beiden Modi dasselbe mittlere Grau, hell genug für den dunklen Grund
    private static readonly Color BadgeFill = Color.FromArgb(92, 97, 106);
    private static readonly Color BadgePosition = Color.FromArgb(255, 240, 170);   // ganz helles Gelb, gut lesbar auf dem Grau

    private string[] _trackNames = new string[Project.TrackCount];
    private TrackSettings[] _tracks = Project.CreateDefaultTracks();
    private StepLevel[][] _steps = Project.CreateEmptySteps();
    private Meter _meter = Meter.FourFour;
    private int _playStep = -1;
    private bool _barFlash;
    private string _patternName = string.Empty;
    private string _sequencePosition = string.Empty;

    private enum DragMode { None, Paint, Fader, Beat }

    private DragMode _dragMode;
    private StepLevel _paintValue;
    private int _dragTrack;
    private int _dragFader;
    private bool _dragAll;
    private readonly float[] _dragStart = new float[Project.TrackCount];

    // Die Schalter sind echte Controls, damit sie aussehen und sich bedienen lassen wie die übrigen im Programm.
    private readonly CheckBox[] _trackSwitches = new CheckBox[Project.TrackCount];
    private bool _syncingSwitches;

    private int _selectedBeat = -1;   // die markierte Zählzeit, -1 ohne Markierung
    private int _dragBeat = -1;       // die Zählzeit, die gerade gezogen wird
    private int _dropBeat = -1;       // die Zählzeit unter der Maus, auf die sie fallen würde

    public StepGrid()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        for (var track = 0; track < Project.TrackCount; track++)
        {
            var trackSwitch = new CheckBox
            {
                Appearance = Appearance.ToggleSwitch,
                VisualStylesMode = VisualStylesMode.Net11,
                AccessibleDescription = SwitchHint,
                Checked = true,
                TabIndex = track,
                Tag = track,
                Visible = false,
            };
            trackSwitch.CheckedChanged += TrackSwitch_CheckedChanged;
            _trackSwitches[track] = trackSwitch;
            Controls.Add(trackSwitch);
        }
    }

    /// <summary>Wird ausgelöst, wenn der Benutzer einen Schritt oder Regler geändert hat.</summary>
    public event EventHandler? Edited;

    /// <summary>Wird am Ende eines Zugs mit der Maus ausgelöst. Ein ganzer Zug zählt als eine Änderung.</summary>
    public event EventHandler? EditCompleted;

    /// <summary>Links beim Drücken, rechts beim Loslassen der Maustaste, passend zum üblichen Kontextmenü.</summary>
    public event EventHandler<TrackNameEventArgs>? TrackNameClicked;

    /// <summary>Rechtsklick auf einen leeren Schritt. Dort gibt es nichts zu wechseln, das Formular erklärt es.</summary>
    public event EventHandler<MouseEventArgs>? EmptyStepRightClicked;

    /// <summary>Rechtsklick auf die Zahl einer Zählzeit. Das Menü dazu zeigt das Formular.</summary>
    public event EventHandler<BeatEventArgs>? BeatMenuRequested;

    /// <summary>Eine Zählzeit wurde auf eine andere gezogen und dorthin kopiert.</summary>
    public event EventHandler<BeatEventArgs>? BeatCopied;

    /// <summary>Läuft gerade ein Zug mit der Maus?</summary>
    internal bool IsEditing => _dragMode != DragMode.None;

    /// <summary>Die markierte Zählzeit ab 0, -1 ohne Markierung.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal int SelectedBeat
    {
        get => _selectedBeat;
        set
        {
            if (_selectedBeat != value)
            {
                _selectedBeat = value;
                Invalidate();
            }
        }
    }

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
                ColorSwitches();
                Invalidate();
            }
        }
    }

    /// <summary>
    /// Oben links über den Spurnamen: welches Pattern das Raster zeigt, etwa „Pattern 2“.
    /// Läuft eine Abfolge, steht daneben die Stelle darin, etwa „4/5“ für den vierten von fünf Takten.
    /// </summary>
    internal void SetPatternCaption(string patternName, string sequencePosition)
    {
        if (patternName != _patternName || sequencePosition != _sequencePosition)
        {
            _patternName = patternName;
            _sequencePosition = sequencePosition;
            Invalidate(CaptionArea);
        }
    }

    internal void Bind(string[] trackNames, TrackSettings[] tracks, StepLevel[][] steps, Meter meter)
    {
        _trackNames = trackNames;
        _tracks = tracks;
        _steps = steps;
        _meter = meter;
        if (_selectedBeat >= meter.BeatCount)
        {
            _selectedBeat = -1;
        }
        PlaceSwitches();
        SyncSwitches();
        Invalidate();
    }

    // ==================== Aufteilung ====================

    private int Padding8 => LogicalToDeviceUnits(8);
    private int NameWidth => LogicalToDeviceUnits(104);
    private int HeaderHeight => LogicalToDeviceUnits(26);
    private int GroupGap => LogicalToDeviceUnits(7);
    private int FaderGap => LogicalToDeviceUnits(7);
    private int SlideWidth => LogicalToDeviceUnits(52);
    private int KnobRoom => LogicalToDeviceUnits(5);    // so weit ragt der Knopf an beiden Enden über die Schiene
    private int ValueGap => LogicalToDeviceUnits(4);    // zwischen Knopf am Anschlag und Wert bleibt so noch Luft

    // Die Wertanzeige steht direkt hinter ihrem Regler und ist je Spalte nur so breit wie der längste Wert:
    // „100“ bei Volume, „R100“ bei Pan, „+12“ bei Pitch. Vorher waren alle drei gleich breit und die Werte rechtsbündig,
    // dadurch standen sie näher am nächsten Regler als am eigenen.
    private static readonly int[] ValueWidths = [24, 31, 26];
    private const int SwitchWidth = 40;
    private const int SwitchDrawnWidth = 35;   // so breit zeichnet sich der Schalter links in seinem Feld; danach richtet er sich rechtsbündig am Raster aus

    private int ValueWidth(int fader) => LogicalToDeviceUnits(ValueWidths[fader]);

    private int FaderWidth(int fader) => (2 * KnobRoom) + SlideWidth + ValueGap + ValueWidth(fader);

    /// <summary>Die Breite der ersten <paramref name="count"/> Reglerspalten samt ihrem Abstand davor.</summary>
    private int FadersWidth(int count)
    {
        var width = 0;
        for (var fader = 0; fader < count; fader++)
        {
            width += FaderGap + FaderWidth(fader);
        }
        return width;
    }

    private float RowHeight => Math.Max(1f, (ClientSize.Height - HeaderHeight - Padding8) / (float)Project.TrackCount);

    private int GridLeft => Padding8 + NameWidth;

    private int GridRight => ClientSize.Width - Padding8 - FadersWidth(FaderCount);

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
        var left = GridRight + FadersWidth(fader) + FaderGap;
        var top = (int)(HeaderHeight + (track * RowHeight));
        return new Rectangle(left, top, FaderWidth(fader), (int)RowHeight);
    }

    private Rectangle FaderSlide(int track, int fader)
    {
        var column = FaderColumn(track, fader);
        return new Rectangle(column.Left + KnobRoom, column.Top, SlideWidth, column.Height);
    }

    /// <summary>Die Ecke über den Spurnamen, nach rechts bis zur Mitte des ersten Schritts, wo die „1“ steht.</summary>
    private Rectangle CaptionArea => new(0, 0, (int)StepRect(0, 0).Left + ((int)StepWidth / 2) - LogicalToDeviceUnits(6), HeaderHeight);

    private int SwitchHeight => LogicalToDeviceUnits(20);
    private int SwitchGap => LogicalToDeviceUnits(2);

    /// <summary>Name und Schalter stehen als Block übereinander in der Mitte der Zeile. Hier beginnt der Name.</summary>
    private int NameTop(int track) => (int)(HeaderHeight + (track * RowHeight) + ((RowHeight - Font.Height - SwitchGap - SwitchHeight) / 2f));

    private bool HasSound(int track) => !string.IsNullOrEmpty(_trackNames[track]) && _trackNames[track] != DrumKit.NoTrackName;

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
        else if (Dark)
        {
            graphics.Clear(SystemColors.Control);   // Window wäre im Dunkelmodus heller als die Zellen
        }
        DrawHeader(graphics);

        for (var track = 0; track < Project.TrackCount; track++)
        {
            var nameRect = new Rectangle(Padding8, NameTop(track), NameWidth - Padding8, Font.Height);
            TextRenderer.DrawText(graphics, _trackNames[track] ?? string.Empty, Font, nameRect, _tracks[track].Muted ? SystemColors.GrayText : ForeColor,
                TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis);

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
        if (_patternName.Length > 0)
        {
            const TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.NoClipping;
            using var bold = new Font(Font, FontStyle.Bold);
            using var semibold = new Font("Segoe UI Semibold", Font.SizeInPoints);   // für die Stelle in der Abfolge

            // Eine Plakette vom linken Rand bis direkt vor die erste Rasterspalte. Der Name bleibt bündig mit den
            // Spurnamen darunter, die Stelle in der Abfolge steht rechts darin. Auch beim Taktblitz gut lesbar.
            var inset = LogicalToDeviceUnits(5);
            var badgeLeft = Padding8 - inset;
            var badgeRight = (int)StepRect(0, 0).Left;
            var badge = new RectangleF(badgeLeft, LogicalToDeviceUnits(3), badgeRight - badgeLeft - 4, HeaderHeight - LogicalToDeviceUnits(6));
            var radius = LogicalToDeviceUnits(4);
            using (var fill = new SolidBrush(BadgeFill))
            {
                graphics.FillRoundedRectangle(fill, badge, new SizeF(radius, radius));
            }

            // Passt eine lange Abfolge wie „12/16“ nicht neben „Pattern 4“, steht dort kurz „P4“.
            var innerRight = badgeRight - inset;
            var positionWidth = _sequencePosition.Length > 0 ? TextRenderer.MeasureText(graphics, _sequencePosition, semibold, Size.Empty, flags).Width : 0;
            var room = innerRight - Padding8 - (positionWidth > 0 ? positionWidth + LogicalToDeviceUnits(6) : 0);
            var name = _patternName;
            if (TextRenderer.MeasureText(graphics, name, bold, Size.Empty, flags).Width > room)
            {
                name = name.Replace("Pattern ", "P");
            }

            TextRenderer.DrawText(graphics, name, bold, new Rectangle(Padding8, 0, innerRight - Padding8, HeaderHeight), Color.White, BadgeFill, flags);
            if (positionWidth > 0)
            {
                TextRenderer.DrawText(graphics, _sequencePosition, semibold, new Rectangle(innerRight - positionWidth, 0, positionWidth, HeaderHeight), BadgePosition, BadgeFill, flags);
            }
        }

        // Jede Zählzeit trägt in der Kopfzeile ein Band in der Farbe ihrer leeren Schritte. Es zeigt auch ohne
        // Markierung, dass es dort etwas zum Anfassen gibt: Klick markiert, Ziehen kopiert.
        var bandRadius = LogicalToDeviceUnits(4);
        for (var beat = 0; beat < _meter.BeatCount; beat++)
        {
            using var band = new SolidBrush(_meter.GroupOf[_meter.StepsOfBeat(beat).Start] % 2 == 0 ? CellOffA : CellOffB);
            graphics.FillRoundedRectangle(band, BeatBand(beat), new SizeF(bandRadius, bandRadius));
        }
        DrawBeatMark(graphics, _selectedBeat, target: false);
        DrawBeatMark(graphics, _dropBeat, target: true);
        for (var step = 0; step < _meter.Steps; step++)
        {
            var cell = StepRect(0, step);
            var rect = new Rectangle((int)cell.Left, 0, (int)cell.Width, HeaderHeight);
            var isBeat = _meter.IsBeatStart[step];
            var marked = _meter.BeatOf[step] == _selectedBeat;
            TextRenderer.DrawText(graphics, isBeat ? (_meter.BeatOf[step] + 1).ToString() : "·", Font, rect,
                marked ? Color.White : isBeat ? ForeColor : SystemColors.GrayText, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        for (var fader = 0; fader < FaderCount; fader++)
        {
            var column = FaderColumn(0, fader);
            var rect = new Rectangle(column.Left, 0, column.Width, HeaderHeight);
            TextRenderer.DrawText(graphics, FaderTitles[fader], Font, rect, SystemColors.GrayText, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }
    }

    /// <summary>
    /// Die markierte Zählzeit trägt in der Kopfzeile ihr Band in kräftiger Farbe und einen Rahmen um ihre Spalten.
    /// Das Ziel beim Ziehen zeigt nur einen gestrichelten Rahmen: Dorthin würde kopiert.
    /// </summary>
    private void DrawBeatMark(Graphics graphics, int beat, bool target)
    {
        if (beat < 0 || beat >= _meter.BeatCount)
        {
            return;
        }

        var span = BeatSpan(beat);
        var radius = LogicalToDeviceUnits(4);
        if (!target)
        {
            using var fill = new SolidBrush(BeatMark);
            graphics.FillRoundedRectangle(fill, BeatBand(beat), new SizeF(radius, radius));
        }
        using var pen = new Pen(BeatMark, LogicalToDeviceUnits(2)) { DashStyle = target ? DashStyle.Dash : DashStyle.Solid };
        var frame = new RectangleF(span.Left, HeaderHeight, span.Width, Project.TrackCount * RowHeight);
        graphics.DrawRoundedRectangle(pen, frame, new SizeF(radius, radius));
    }

    private void DrawStep(Graphics graphics, int track, int step)
    {
        var level = _steps[track][step];
        var playing = step == _playStep;
        var empty = playing ? PlayheadOff : _meter.GroupOf[step] % 2 == 0 ? CellOffA : CellOffB;
        var set = _tracks[track].Muted ? CellMuted : playing ? PlayheadOn : CellOn;
        var cell = StepRect(track, step);
        var radius = LogicalToDeviceUnits(3);

        // Die Stärke zeigt sich an der Form, nicht an der Farbe: Ein leiser Schlag füllt nur die Mitte der Zelle,
        // ein normaler die ganze, ein betonter trägt dazu das Akzentzeichen aus der Notenschrift.
        using var brush = new SolidBrush(level is StepLevel.Normal or StepLevel.Accent ? set : empty);
        graphics.FillRoundedRectangle(brush, cell, new SizeF(radius, radius));
        if (level == StepLevel.Soft)
        {
            var core = RectangleF.Inflate(cell, -cell.Width * SoftInset, -cell.Height * SoftInset);
            using var coreBrush = new SolidBrush(set);
            graphics.FillRoundedRectangle(coreBrush, core, new SizeF(radius, radius));
        }
        else if (level == StepLevel.Accent)
        {
            var height = Math.Min(cell.Height * 0.44f, LogicalToDeviceUnits(10));
            var width = height * 1.1f;
            var left = cell.Left + ((cell.Width - width) / 2f);
            var middle = cell.Top + (cell.Height / 2f);
            using var pen = new Pen(AccentMark, LogicalToDeviceUnits(2)) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
            var smoothing = graphics.SmoothingMode;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.DrawLines(pen, [new PointF(left, middle - (height / 2f)), new PointF(left + width, middle), new PointF(left, middle + (height / 2f))]);
            graphics.SmoothingMode = smoothing;
        }
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
        var valueRect = new Rectangle(column.Right - ValueWidth(fader), column.Top, ValueWidth(fader), column.Height);
        TextRenderer.DrawText(graphics, FaderText(track, fader), Font, valueRect, SystemColors.GrayText,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
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
                return pan == 0 ? Lng.T("M") : pan < 0 ? $"L{-pan}" : $"R{pan}";
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

    /// <summary>Die Spalten einer Zählzeit in der Kopfzeile, mit etwas Rand um die äußeren Schritte.</summary>
    private RectangleF BeatSpan(int beat)
    {
        var (start, count) = _meter.StepsOfBeat(beat);
        var margin = LogicalToDeviceUnits(3);
        var left = StepRect(0, start).Left - margin;
        return new RectangleF(left, 0, StepRect(0, start + count - 1).Right + margin - left, HeaderHeight);
    }

    /// <summary>Das Band einer Zählzeit in der Kopfzeile, so breit wie ihre Schritte darunter.</summary>
    private RectangleF BeatBand(int beat)
    {
        var (start, count) = _meter.StepsOfBeat(beat);
        var left = StepRect(0, start).Left;
        return new RectangleF(left, LogicalToDeviceUnits(3), StepRect(0, start + count - 1).Right - left, HeaderHeight - LogicalToDeviceUnits(6));
    }

    /// <param name="headerOnly">Zum Anfassen zählt nur die Kopfzeile, zum Ablegen die ganze Höhe der Spalten.</param>
    private bool HitBeat(Point point, bool headerOnly, out int beat)
    {
        beat = -1;
        if (point.Y < 0 || (headerOnly && point.Y >= HeaderHeight))
        {
            return false;
        }

        for (var candidate = 0; candidate < _meter.BeatCount; candidate++)
        {
            var span = BeatSpan(candidate);
            if (point.X >= span.Left && point.X < span.Right)
            {
                beat = candidate;
                return true;
            }
        }
        return false;
    }

    private bool HitName(Point point, out int track)
    {
        track = (int)((point.Y - HeaderHeight) / RowHeight);
        return point.Y >= HeaderHeight && track < Project.TrackCount
            && point.X >= Padding8 && point.X < GridLeft
            && HasSound(track);
    }

    private void SetStep(int track, int step, StepLevel value)
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
        if (_dragAll)
        {
            // Vom Stand zu Beginn des Zugs aus gerechnet: Wer am Rand anstößt, findet beim Zurückziehen seinen Abstand wieder.
            var change = value - _dragStart[_dragTrack];
            for (var track = 0; track < Project.TrackCount; track++)
            {
                SetFaderValue(track, _dragFader, Math.Clamp(_dragStart[track] + change, 0f, 1f));
                Invalidate(FaderColumn(track, _dragFader));
            }
        }
        else
        {
            SetFaderValue(_dragTrack, _dragFader, value);
            Invalidate(FaderColumn(_dragTrack, _dragFader));
        }
        Edited?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Was an dieser Stelle möglich ist, für die Statuszeile. Während eines Zugs gibt es nichts zu erklären.</summary>
    internal string? HintAt(Point point)
    {
        if (_dragMode != DragMode.None)
        {
            return null;
        }
        if (HitName(point, out _))
        {
            return TrackNameHint;
        }
        if (HitBeat(point, headerOnly: true, out _))
        {
            return BeatHint;
        }
        if (_patternName.Length > 0 && CaptionArea.Contains(point))
        {
            return CaptionHint;
        }

        for (var fader = 0; fader < FaderCount; fader++)
        {
            var column = FaderColumn(0, fader);
            if (point.X >= column.Left && point.X < column.Right)
            {
                return FaderHints[fader];
            }
        }
        return HitStep(point, out _, out _) ? StepHint : null;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Focus();   // beendet eine laufende Eingabe im Abfolge-Feld
        if (e.Button == MouseButtons.Right && HitStep(e.Location, out var changed, out var changedStep))
        {
            if (_steps[changed][changedStep] == StepLevel.Off)
            {
                EmptyStepRightClicked?.Invoke(this, e);
                return;
            }

            // Reihum, beginnend mit dem häufigsten Wunsch: aus einem normalen Schlag einen betonten machen.
            var next = _steps[changed][changedStep] switch
            {
                StepLevel.Normal => StepLevel.Accent,
                StepLevel.Accent => StepLevel.Soft,
                StepLevel.Soft => StepLevel.Normal,
                _ => StepLevel.Off,
            };
            SetStep(changed, changedStep, next);
            return;
        }
        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        if (HitBeat(e.Location, headerOnly: true, out var beat))
        {
            _dragMode = DragMode.Beat;
            _dragBeat = beat;
            _dropBeat = -1;
            return;
        }

        if (HitStep(e.Location, out var track, out var step))
        {
            _dragMode = DragMode.Paint;
            _paintValue = _steps[track][step] == StepLevel.Off ? StepLevel.Normal : StepLevel.Off;
            SetStep(track, step, _paintValue);
        }
        else if (HitFader(e.Location, out track, out var fader))
        {
            var allTracks = fader != PanFader && (ModifierKeys & Keys.Control) != 0;
            if (e.Clicks == 2)
            {
                for (var other = 0; other < Project.TrackCount; other++)
                {
                    if (allTracks || other == track)
                    {
                        ResetFader(other, fader);
                        Invalidate(FaderColumn(other, fader));
                    }
                }
                Edited?.Invoke(this, EventArgs.Empty);
                return;
            }

            _dragMode = DragMode.Fader;
            _dragTrack = track;
            _dragFader = fader;
            _dragAll = allTracks;
            for (var other = 0; other < Project.TrackCount; other++)
            {
                _dragStart[other] = GetFaderValue(other, fader);
            }
            DragFaderTo(e.X);
        }
        else if (HitName(e.Location, out track))
        {
            TrackNameClicked?.Invoke(this, new TrackNameEventArgs(track, e.Button, e.Location));
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
        else if (_dragMode == DragMode.Beat)
        {
            var drop = HitBeat(e.Location, headerOnly: false, out var over) && over != _dragBeat ? over : -1;
            if (drop != _dropBeat)
            {
                _dropBeat = drop;
                Invalidate();
            }
        }

        var clickable = _dragMode == DragMode.None && (HitName(e.Location, out _) || HitBeat(e.Location, headerOnly: true, out _));
        var cursor = clickable || _dropBeat >= 0 ? Cursors.Hand : Cursors.Default;
        if (Cursor != cursor)
        {
            Cursor = cursor;
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (_dragMode == DragMode.Beat)
        {
            DropBeat(e.Location);
        }
        EndEdit();
        if (e.Button == MouseButtons.Right && HitName(e.Location, out var track))
        {
            TrackNameClicked?.Invoke(this, new TrackNameEventArgs(track, e.Button, e.Location));
        }
        else if (e.Button == MouseButtons.Right && HitBeat(e.Location, headerOnly: true, out var beat))
        {
            SelectedBeat = beat;
            BeatMenuRequested?.Invoke(this, new BeatEventArgs(beat, beat, e.Location));
        }
    }

    protected override void OnMouseCaptureChanged(EventArgs e)
    {
        base.OnMouseCaptureChanged(e);
        EndEdit();   // etwa, wenn ein anderes Fenster die Maus übernimmt
    }

    // ==================== Spurschalter ====================

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        PlaceSwitches();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        PlaceSwitches();
    }

    private void PlaceSwitches()
    {
        for (var track = 0; track < Project.TrackCount; track++)
        {
            var trackSwitch = _trackSwitches[track];
            trackSwitch.Bounds = new Rectangle(GridLeft - Padding8 - LogicalToDeviceUnits(SwitchDrawnWidth), NameTop(track) + Font.Height + SwitchGap, LogicalToDeviceUnits(SwitchWidth), SwitchHeight);
            trackSwitch.Visible = HasSound(track);   // wo das Kit keinen Klang hat, gibt es nichts zu schalten
            trackSwitch.AccessibleName = _trackNames[track];
        }
        ColorSwitches();
    }

    /// <summary>Die Schalter liegen auf dem Raster und tragen deshalb dessen Farbe, auch während des Taktblitzes.</summary>
    private void ColorSwitches()
    {
        var color = _barFlash ? BarFlashColor : Dark ? SystemColors.Control : BackColor;
        foreach (var trackSwitch in _trackSwitches)
        {
            if (trackSwitch.BackColor != color)
            {
                trackSwitch.BackColor = color;
            }
        }
    }

    private void SyncSwitches()
    {
        _syncingSwitches = true;
        for (var track = 0; track < Project.TrackCount; track++)
        {
            _trackSwitches[track].Checked = !_tracks[track].Muted;
        }
        _syncingSwitches = false;
    }

    private void TrackSwitch_CheckedChanged(object? sender, EventArgs e)
    {
        if (_syncingSwitches || sender is not CheckBox { Tag: int track } trackSwitch)
        {
            return;
        }

        if ((ModifierKeys & Keys.Control) != 0)
        {
            // Solo: Spielt noch eine andere Spur, bleibt nur diese. War sie schon die einzige, spielen wieder alle.
            var othersPlay = false;
            for (var other = 0; other < Project.TrackCount; other++)
            {
                othersPlay |= other != track && HasSound(other) && !_tracks[other].Muted;
            }
            for (var other = 0; other < Project.TrackCount; other++)
            {
                _tracks[other].Muted = othersPlay && other != track;
            }
            SyncSwitches();
        }
        else
        {
            _tracks[track].Muted = !trackSwitch.Checked;
        }
        Invalidate();
        Edited?.Invoke(this, EventArgs.Empty);
    }

    private void EndEdit()
    {
        if (_dragBeat >= 0 || _dropBeat >= 0)
        {
            // ein abgebrochener Zug an einer Zählzeit, etwa weil ein anderes Fenster die Maus übernahm
            _dragBeat = -1;
            _dropBeat = -1;
            _dragMode = DragMode.None;
            Invalidate();
        }
        if (_dragMode != DragMode.None)
        {
            _dragMode = DragMode.None;
            EditCompleted?.Invoke(this, EventArgs.Empty);
        }
    }

    // ==================== Zählzeiten ====================

    /// <summary>
    /// Ende eines Zugs an einer Zählzeit: Über einer anderen losgelassen wird dorthin kopiert. Wieder auf der eigenen
    /// Zahl losgelassen war es ein Klick, der die Zählzeit markiert oder die Markierung aufhebt.
    /// </summary>
    private void DropBeat(Point location)
    {
        var source = _dragBeat;
        var target = _dropBeat;
        _dragBeat = -1;
        _dropBeat = -1;
        if (target >= 0)
        {
            PasteBeat(target, CopyBeat(source));
            _selectedBeat = target;
            BeatCopied?.Invoke(this, new BeatEventArgs(source, target, location));
        }
        else
        {
            if (HitBeat(location, headerOnly: true, out var released) && released == source)
            {
                _selectedBeat = _selectedBeat == source ? -1 : source;
            }
            _dragMode = DragMode.None;   // nichts geändert, also auch kein Eintrag im Verlauf
        }
        Invalidate();
    }

    /// <summary>Die Schritte einer Zählzeit, je Spur eine Zeile.</summary>
    internal StepLevel[][] CopyBeat(int beat)
    {
        var (start, count) = _meter.StepsOfBeat(beat);
        var copy = new StepLevel[Project.TrackCount][];
        for (var track = 0; track < Project.TrackCount; track++)
        {
            copy[track] = _steps[track].AsSpan(start, count).ToArray();
        }
        return copy;
    }

    /// <summary>Setzt die Schritte einer Zählzeit. Ist die Quelle kürzer, bleibt der Rest leer, ist sie länger, fällt ihr Ende weg.</summary>
    internal void PasteBeat(int beat, StepLevel[][] source)
    {
        if (WriteBeat(beat, source))
        {
            BeatsChanged();
        }
    }

    /// <summary>Überträgt eine Zählzeit auf alle anderen des Patterns.</summary>
    internal void FillBeats(int beat)
    {
        var source = CopyBeat(beat);
        var changed = false;
        for (var other = 0; other < _meter.BeatCount; other++)
        {
            changed |= other != beat && WriteBeat(other, source);
        }
        if (changed)
        {
            BeatsChanged();
        }
    }

    internal void ClearBeat(int beat) => PasteBeat(beat, Project.CreateEmptySteps());

    private bool WriteBeat(int beat, StepLevel[][] source)
    {
        var (start, count) = _meter.StepsOfBeat(beat);
        var changed = false;
        for (var track = 0; track < Project.TrackCount; track++)
        {
            for (var index = 0; index < count; index++)
            {
                var level = index < source[track].Length ? source[track][index] : StepLevel.Off;
                changed |= _steps[track][start + index] != level;
                _steps[track][start + index] = level;
            }
        }
        return changed;
    }

    private void BeatsChanged()
    {
        Invalidate();
        Edited?.Invoke(this, EventArgs.Empty);
    }
}

public sealed class TrackNameEventArgs(int track, MouseButtons button, Point location) : EventArgs
{
    public int Track { get; } = track;

    public MouseButtons Button { get; } = button;

    /// <summary>Die Mausposition im Raster.</summary>
    public Point Location { get; } = location;
}

/// <summary>Eine Zählzeit ab 0. Beim Kopieren durch Ziehen ist <see cref="Target"/> die Zählzeit, auf die sie fiel.</summary>
public sealed class BeatEventArgs(int beat, int target, Point location) : EventArgs
{
    public int Beat { get; } = beat;

    public int Target { get; } = target;

    /// <summary>Die Mausposition im Raster.</summary>
    public Point Location { get; } = location;
}
