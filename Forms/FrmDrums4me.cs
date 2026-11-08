using Drums4me.Classes;
using Drums4me.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Drums4me.Forms;

public partial class FrmDrums4me : Form, IMessageFilter
{
    private const int OutputLatencyMs = 40;
    private const int MaxRestartTries = 5;
    private const int MaxDeviceLosses = 3;
    private const double DeviceLossWindowSeconds = 30;
    private const int BarFlashMs = 120;
    private const int TempoStep = 1;
    private const int TempoBigStep = 5;
    private const int VolumeStep = 5;
    private const int TrainerDefaultRange = 20;
    private const string RenameCaption = "&Umbenennen…";
    private const string RenameLockedCaption = "Umbenennen (nur bei eigenen Kits)";
    private const string CountInCaption = "Vorzählen";
    private const string SequenceError = "Abfolge: Erlaubt sind nur die Ziffern 1 bis 4.";
    private const int WmMouseMove = 0x0200;
    private const int WmMouseLeave = 0x02A3;

    private static readonly string DefaultProjectFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppSettings.AppName);
    private static readonly string KitFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppSettings.AppName, "Kits");

    // Hierhin installieren die Inhaltspakete. Für normale Benutzer sind die Dateien schreibgeschützt.
    private static readonly string SharedFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppSettings.AppName);
    private static readonly string SharedKitFolder = Path.Combine(SharedFolder, "Kits");
    private static readonly string SharedDemoFolder = Path.Combine(SharedFolder, "Demos");

    private readonly SequencerEngine _engine;
    private readonly AppSettings _settings = AppSettings.Load();
    private List<DrumKit> _kits = [];
    private DrumKit _kit = DrumKit.Empty;
    private Project _project = Project.CreateDefault();
    private WasapiPlayer? _output;
    private string? _filePath;
    private bool _dirty;
    private Project _savedProject;
    private readonly UndoHistory _history;
    private bool _updatingUi;
    private bool _sequenceActive;
    private const int MaxStatusPathLength = 60;
    private const int RibbonPagePadding = 4;
    private const string AllFilterItems = "Alle";
    private const string PlaySymbol = "▶";
    private const string StopSymbol = "■";

    private string _projectFolder = DefaultProjectFolder;
    private string? _otherFolder;
    private bool _fillingFilters;
    private readonly string? _startFile;
    private string? _pendingFile;
    private long _flashedBar = -1;
    private long _flashEnd;
    private int _trainerBpm;
    private WasapiPlayer? _previewOutput;
    private MMDevice? _outputDevice;
    private MMDevice? _previewDevice;
    private int _menuTrack;
    private long _playStart;
    private int _restartTries;
    private int _deviceLosses;
    private long _firstDeviceLoss;
    private string? _deviceLossMessage;
    private Size _defaultSize;
    private string? _hoverHint;
    private string? _statusError;
    private string? _notice;

    public FrmDrums4me()
    {
        InitializeComponent();
        _savedProject = _project.Snapshot();
        _history = new UndoHistory(_project);
        _engine = new SequencerEngine(EngineState.From(_project, _kit));
    }

    /// <summary>Startet mit der angegebenen Datei statt mit der zuletzt benutzten.</summary>
    public FrmDrums4me(string? startFile) : this()
    {
        _startFile = startFile;
    }

    // ==================== Start und Ende ====================

    private void FrmDrums4me_Load(object? sender, EventArgs e)
    {
        Application.AddMessageFilter(this);
        _defaultSize = Size;
        RestoreWindow();
        if (Application.IsDarkModeEnabled)
        {
            // Die Trennlinien der Statuszeile wären auf Dunkel grell weiß.
            statusProjects.BorderSides = ToolStripStatusLabelBorderSides.None;
            statusTrainer.BorderSides = ToolStripStatusLabelBorderSides.None;
            statusBars.BorderSides = ToolStripStatusLabelBorderSides.None;
            statusTime.BorderSides = ToolStripStatusLabelBorderSides.None;

            // Die Trenner innerhalb eines Bereichs sollen auch auf Dunkel schwächer sein als die Bereichsgrenzen.
            lineFlash.BackColor = Color.FromArgb(56, 56, 56);
            lineKit.BackColor = lineFlash.BackColor;
            lineCommands.BackColor = lineFlash.BackColor;
            lineWindowCommands.BackColor = lineFlash.BackColor;
        }
        Directory.CreateDirectory(KitFolder);
        ApplyProjectFolder();
        checkBoxFlash.Checked = _settings.FlashOnBar;
        checkBoxOpenLast.Checked = _settings.OpenLastFile;
        FillAudioDevices();
        projectBrowser.SetColumnWidths(_settings.ColumnWidths);
        comboMeter.Items.AddRange([.. Meter.All]);
        ShowColorMode();
        buttonOwnKits.AccessibleDescription = $"Öffnet den Ordner für eigene Kits: {KitFolder}";
        buttonInstalledKits.AccessibleDescription = $"Öffnet den Ordner der installierten Kits: {SharedKitFolder}";

        ActiveControl = buttonPlay;
        ScanKits();
        ShowKitCount();
        var opened = _startFile is not null
            ? LoadProjectFile(_startFile)
            : _settings.OpenLastFile && _settings.LastFile is { } lastFile && File.Exists(lastFile) && LoadProjectFile(lastFile);
        if (!opened)
        {
            ApplyProject();
        }

        if (_kits.Count == 0)
        {
            ShowNoKitsMessage();
        }
        UpdateRibbonHeight();
        PlaceFileCommands();
    }

    /// <summary>
    /// Die Befehlsspalte der Karte „Datei“ beginnt direkt unter den Reitern, Band und Projektliste liegen rechts davon.
    /// Wie hoch die Reiter sind, steht erst zur Laufzeit fest. Danach hält der Anker die Spalte in Form.
    /// </summary>
    private void PlaceFileCommands()
    {
        var top = ribbonTabs.Top + ribbonTabs.GetTabRect(0).Bottom;
        panelFileCommands.SetBounds(0, top, panelFileCommands.Width, statusStrip.Top - top);
    }

    private void FrmDrums4me_Resize(object? sender, EventArgs e) => UpdateRibbonHeight();

    /// <summary>Reicht die Breite nicht, brechen die Gruppen der Karte um. Das Band wächst dann mit, damit alles erreichbar bleibt.</summary>
    private void UpdateRibbonHeight()
    {
        if (WindowState == FormWindowState.Minimized || ribbonTabs.SelectedTab?.Controls[0] is not FlowLayoutPanel flow)
        {
            return;
        }

        // Auf der Karte „Datei“ hält ein Innenabstand die Gruppen rechts der Befehlsspalte.
        var page = ribbonTabs.DisplayRectangle;
        var width = page.Width - ribbonTabs.SelectedTab.Padding.Horizontal;
        var height = (ribbonTabs.Height - page.Height) + flow.GetPreferredSize(new Size(width, 0)).Height + RibbonPagePadding;
        if (ribbonTabs.Height != height)
        {
            ribbonTabs.Height = height;
        }
    }

    private void FrmDrums4me_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!ConfirmDiscard())
        {
            e.Cancel = true;
            return;
        }
        StopPlayback();
        StopPreview();
        Application.RemoveMessageFilter(this);
        _settings.ColumnWidths = projectBrowser.GetColumnWidths();
        var bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;
        _settings.Window = new WindowPlacement(bounds.X, bounds.Y, bounds.Width, bounds.Height, WindowState == FormWindowState.Maximized);
        _settings.Save();
    }

    /// <summary>
    /// Setzt Größe und Lage vom letzten Mal. Wurde inzwischen ein Bildschirm abgebaut oder umgestellt,
    /// muss die Titelleiste noch sichtbar sein, sonst bleibt es bei der Standardlage.
    /// </summary>
    private void RestoreWindow()
    {
        if (_settings.Window is not { } window)
        {
            return;
        }

        var bounds = new Rectangle(window.X, window.Y, Math.Max(window.Width, MinimumSize.Width), Math.Max(window.Height, MinimumSize.Height));
        var titleBar = new Rectangle(bounds.X, bounds.Y, bounds.Width, SystemInformation.CaptionHeight);
        if (!Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(titleBar)))
        {
            return;
        }

        StartPosition = FormStartPosition.Manual;
        Bounds = bounds;
        if (window.Maximized)
        {
            WindowState = FormWindowState.Maximized;
        }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // Im Abfolge-Feld nimmt Strg+Z die Texteingabe zurück, nicht die letzte Projektänderung.
        if (keyData is (Keys.Control | Keys.Z) or (Keys.Control | Keys.Y) && IsTyping)
        {
            return false;
        }

        if (keyData == Keys.Space && !IsTyping)
        {
            TogglePlayback();
            return true;
        }

        // Eine offene Auswahlliste schließt Esc selbst, das soll die Karte nicht wechseln.
        if (keyData == Keys.Escape && ribbonTabs.SelectedTab != tabPageStart && ActiveControl is not ComboBox { DroppedDown: true } && !projectBrowser.IsEditingName)
        {
            ribbonTabs.SelectedTab = tabPageStart;
            return true;
        }

        if (RunShortcut(keyData))
        {
            return true;
        }

        // Plus und Minus brauchen die anderen Elemente nicht, sie wirken überall außer in den Textfeldern.
        var tempoChange = keyData switch
        {
            Keys.Oemplus or Keys.Add => TempoStep,
            Keys.OemMinus or Keys.Subtract => -TempoStep,
            (Keys.Control | Keys.Oemplus) or (Keys.Control | Keys.Add) => TempoBigStep,
            (Keys.Control | Keys.OemMinus) or (Keys.Control | Keys.Subtract) => -TempoBigStep,
            _ => 0,
        };
        if (tempoChange != 0 && !IsTyping && CanEditProject)
        {
            numericTempo.Value = Math.Clamp(numericTempo.Value + tempoChange, numericTempo.Minimum, numericTempo.Maximum);
            return true;
        }

        var volumeChange = keyData switch
        {
            (Keys.Alt | Keys.Oemplus) or (Keys.Alt | Keys.Add) => VolumeStep,
            (Keys.Alt | Keys.OemMinus) or (Keys.Alt | Keys.Subtract) => -VolumeStep,
            _ => 0,
        };
        if (volumeChange != 0 && CanEditProject)
        {
            trackBarVolume.Value = Math.Clamp(trackBarVolume.Value + volumeChange, trackBarVolume.Minimum, trackBarVolume.Maximum);
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>Die Knöpfe liegen teils auf gerade verborgenen Karten, deshalb rufen die Tasten direkt die Befehle auf.</summary>
    private bool RunShortcut(Keys keyData)
    {
        Action? command = keyData switch
        {
            Keys.Control | Keys.N => () => ButtonNew_Click(this, EventArgs.Empty),
            Keys.Control | Keys.O => () => ribbonTabs.SelectedTab = projectBrowser.Visible ? tabPageStart : tabPageFile,
            Keys.Control | Keys.F => FocusSearch,
            Keys.Control | Keys.S => () => SaveProject(),
            Keys.Control | Keys.Shift | Keys.S => () => SaveProjectAs(),
            Keys.Control | Keys.Z => () => ButtonUndo_Click(this, EventArgs.Empty),
            Keys.Control | Keys.Y => () => ButtonRedo_Click(this, EventArgs.Empty),
            Keys.Control | Keys.T when !projectBrowser.Visible => () => ButtonTap_Click(this, EventArgs.Empty),

            // In Textfeldern löscht Strg+Entf das nächste Wort, das soll so bleiben.
            Keys.Control | Keys.Delete when CanEditProject && !IsTyping => () => ButtonClearPattern_Click(this, EventArgs.Empty),
            Keys.Control | Keys.Oemcomma => () => ribbonTabs.SelectedTab = tabPageSettings,
            Keys.F1 => () => Dialogs.ShowShortcuts(this),
            _ => null,
        };
        command?.Invoke();
        return command is not null;
    }

    /// <summary>Auf der Karte „Datei“ ist das Projekt verborgen. Änderungen daran blieben unbemerkt.</summary>
    private bool CanEditProject => !projectBrowser.Visible;

    /// <summary>Im Abfolge-Feld, in der Suche und beim Umbenennen in der Projektliste gehören die Tasten dem Text.</summary>
    private bool IsTyping => ActiveControl is TextBox || projectBrowser.IsEditingName;

    // ==================== Projektübersicht ====================

    /// <summary>Der andere Ordner gilt nur für die Übersicht und bis zum Programmende. Der Speicherort in den Einstellungen bleibt.</summary>
    private void ButtonOtherFolder_Click(object? sender, EventArgs e)
    {
        otherFolderDialog.InitialDirectory = BrowserFolder;
        if (otherFolderDialog.ShowDialog(this) == DialogResult.OK)
        {
            var chosen = otherFolderDialog.SelectedPath;
            _otherFolder = string.Equals(chosen, _projectFolder, StringComparison.OrdinalIgnoreCase) ? null : chosen;
            UseWaitCursor = true;
            try
            {
                RefreshProjectBrowser();
            }
            finally
            {
                UseWaitCursor = false;
            }
            FocusProjectList();
        }
    }
    /// <summary>Ein langer Pfad würde die Anzahl aus der Statuszeile drängen. Dann steht nur der Ordnername da, der ganze Pfad im Tooltip.</summary>
    private void ProjectBrowser_CountChanged(object? sender, EventArgs e)
    {
        var folder = BrowserFolder;
        var shown = folder.Length <= MaxStatusPathLength ? folder : $"…\\{Path.GetFileName(folder.TrimEnd('\\'))}";
        statusProjects.Text = $"{projectBrowser.CountText} in {shown}";
        statusProjects.ToolTipText = folder;
    }

    private void ProjectBrowser_ProjectChosen(object? sender, string path)
    {
        // Das geladene, unveränderte Projekt muss nicht neu geladen werden.
        if (!_dirty && string.Equals(path, _filePath, StringComparison.OrdinalIgnoreCase))
        {
            ShowProjectBrowser(false);
        }
        else if (ConfirmDiscard())
        {
            LoadProjectFile(path);
        }
    }

    /// <summary>Die Karte „Datei“ zeigt statt des Rasters die Projektliste. Bearbeiten ist währenddessen gesperrt, man sieht ja nichts davon.</summary>
    private void RibbonTabs_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateRibbonHeight();
        var show = ribbonTabs.SelectedTab == tabPageFile;
        if (show != projectBrowser.Visible)
        {
            SuspendLayout();
            stepGrid.Visible = !show;
            projectBrowser.Visible = show;
            panelFileCommands.Visible = show;
            statusProjects.Visible = show;
            ResumeLayout();
            UpdateUndoMenu();
            if (show)
            {
                RefreshProjectBrowser();
            }
        }

        if (show)
        {
            FocusProjectList();
        }
        else if (ribbonTabs.SelectedTab == tabPageStart)
        {
            buttonPlay.Focus();
        }
    }

    /// <summary>Wechselt von jeder Karte zur Suche. Der vorhandene Suchtext ist markiert und wird beim Tippen ersetzt.</summary>
    private void FocusSearch()
    {
        ribbonTabs.SelectedTab = tabPageFile;
        textBoxSearch.Focus();
        textBoxSearch.SelectAll();
    }

    /// <summary>Die markierte Zeile ist das aktuelle oder letzte Projekt. Enter öffnet es, Pfeiltasten und Tippen wählen ein anderes. Ohne Projekte bleibt nur die Suche.</summary>
    private void ProjectBrowser_RenameRequested(object? sender, ProjectRename e)
    {
        if (!ProjectFiles.TryGetRenameTarget(e.FilePath, e.NewName, out var target, out var problem))
        {
            Dialogs.Warning(this, "Das Projekt wurde nicht umbenannt.", problem);
            return;
        }
        if (target == e.FilePath)
        {
            return;
        }

        try
        {
            File.Move(e.FilePath, target);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Dialogs.Error(this, "Das Projekt wurde nicht umbenannt.", ex.Message);
            return;
        }

        if (IsSameFile(_filePath, e.FilePath))
        {
            _filePath = target;
            UpdateTitle();
        }
        if (IsSameFile(_settings.LastFile, e.FilePath))
        {
            RememberFile(target);
        }
        RefreshProjectBrowser(target);
        FocusProjectList();
    }

    private void ProjectBrowser_DuplicateRequested(object? sender, string path)
    {
        var target = ProjectFiles.DuplicateTarget(path);
        try
        {
            File.Copy(path, target);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Dialogs.Error(this, "Das Projekt wurde nicht kopiert.", ex.Message);
            return;
        }

        RefreshProjectBrowser(target);
        FocusProjectList();
        ShowNotice($"„{Path.GetFileNameWithoutExtension(target)}“ angelegt. Mit F2 gibst du ihm einen Namen.");
    }

    /// <summary>Ein gelöschtes offenes Projekt bleibt offen, nur ohne Datei. Schließen fragt dann, ob du es speichern willst.</summary>
    private void ProjectBrowser_DeleteRequested(object? sender, string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        var isOpen = IsSameFile(_filePath, path);
        if (!Dialogs.AskMoveToRecycleBin(this, name, isOpen))
        {
            return;
        }

        var neighbor = projectBrowser.NeighborOf(path);
        try
        {
            ProjectFiles.MoveToRecycleBin(this, path);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (IOException ex)
        {
            Dialogs.Error(this, "Das Projekt wurde nicht gelöscht.", ex.Message);
            return;
        }

        if (isOpen)
        {
            _filePath = null;
            _savedProject = Project.CreateEmpty();
            _dirty = CurrentChanges().Count > 0;
            UpdateTitle();
        }
        if (IsSameFile(_settings.LastFile, path))
        {
            RememberFile(null);
        }
        RefreshProjectBrowser(neighbor);
        FocusProjectList();
        ShowNotice($"„{name}“ liegt jetzt im Papierkorb.");
    }

    private void ProjectBrowser_NoticeRequested(object? sender, string text) => ShowNotice(text);

    private static bool IsSameFile(string? a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    private void FocusProjectList()
    {
        if (!projectBrowser.FocusList())
        {
            textBoxSearch.Focus();
        }
    }

    private void ShowProjectBrowser(bool show) => ribbonTabs.SelectedTab = show ? tabPageFile : tabPageStart;

    private string BrowserFolder => _otherFolder ?? _projectFolder;

    /// <param name="select">Die Zeile, die markiert sein soll. Ohne Angabe ist es das offene oder zuletzt benutzte Projekt.</param>
    private void RefreshProjectBrowser(string? select = null)
    {
        projectBrowser.SetEntries(ProjectIndex.Scan(BrowserFolder, _kits.Select(kit => kit.Name)), select ?? _filePath ?? _settings.LastFile);
        _fillingFilters = true;
        FillFilter(comboFilterKit, projectBrowser.KitNames);
        FillFilter(comboFilterMeter, projectBrowser.MeterNames);
        _fillingFilters = false;
        BrowserFilter_Changed(this, EventArgs.Empty);
    }

    /// <summary>Die gewählten Filter bleiben stehen, solange es den Wert noch gibt.</summary>
    private static void FillFilter(ComboBox combo, IEnumerable<string> values)
    {
        var selected = combo.SelectedItem as string;
        combo.Items.Clear();
        combo.Items.Add(AllFilterItems);
        foreach (var value in values)
        {
            combo.Items.Add(value);
        }
        combo.SelectedItem = selected is not null && combo.Items.Contains(selected) ? selected : AllFilterItems;
    }

    private void BrowserFilter_Changed(object? sender, EventArgs e)
    {
        if (!_fillingFilters)
        {
            projectBrowser.SetFilter(textBoxSearch.Text, FilterValue(comboFilterKit), FilterValue(comboFilterMeter));
        }
    }

    private static string? FilterValue(ComboBox combo) => combo.SelectedItem is string value && value != AllFilterItems ? value : null;

    private void TextBoxSearch_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            projectBrowser.OpenSelected();
        }
        else if (e.KeyCode == Keys.Down)
        {
            e.Handled = true;
            projectBrowser.FocusList();
        }
    }

    // ==================== Wiedergabe ====================

    private void ButtonPlay_Click(object? sender, EventArgs e) => TogglePlayback();

    private void TogglePlayback()
    {
        // Wer selbst startet oder stoppt, übernimmt; ein anstehender Neustart nach einem Geräteausfall entfällt dann.
        restartTimer.Stop();
        if (_output is null)
        {
            StartPlayback();
        }
        else
        {
            StopPlayback();
        }
    }

    /// <summary>Spielt über das gewählte Gerät. Ist es nicht angeschlossen, über den Windows-Standard, damit das Üben nicht daran scheitert.</summary>
    private WasapiPlayer CreateOutput(out MMDevice? device)
    {
        device = AudioDevices.Open(_settings.AudioDeviceId);
        if (device is null && !string.IsNullOrEmpty(_settings.AudioDeviceId))
        {
            ShowNotice($"„{_settings.AudioDeviceName}“ ist nicht angeschlossen, gespielt wird über den Windows-Standard.");
        }

        var builder = new WasapiPlayerBuilder();
        if (device is not null)
        {
            builder = builder.WithDevice(device);
        }
        return builder
            .WithSharedMode()
            .WithEventSync()
            .WithLatency(OutputLatencyMs)
            .WithMmcssThreadPriority("Pro Audio")
            .Build();
    }

    /// <param name="reportErrors">Bei false meldet der Aufrufer einen Fehler selbst. Liefert, ob die Wiedergabe läuft.</param>
    private bool StartPlayback(bool reportErrors = true)
    {
        StopPreview();
        WasapiPlayer? output = null;
        MMDevice? device = null;
        try
        {
            output = CreateOutput(out device);
            output.PlaybackStopped += Output_PlaybackStopped;
            _engine.Rewind();

            // Schon beim Vorzählen ist das Pattern zu sehen, mit dem die Abfolge beginnt.
            if (_sequenceActive && _project.ActiveSequence is [var first, ..] && first != _project.ActivePattern)
            {
                ShowPattern(first);
            }
            _flashedBar = -1;
            output.Init(_engine.ToWaveProvider());
            output.Play();
            _output = output;
            _outputDevice = device;
            buttonPlay.Text = StopSymbol;
            buttonPlay.AccessibleName = "Stopp";
            toolTip.SetToolTip(buttonPlay, "Stopp (Leertaste)");
            _trainerBpm = _project.Bpm;
            _playStart = Stopwatch.GetTimestamp();
            playheadTimer.Start();
            UpdatePatternButtons();
            UpdateTrainerStatus();
            UpdatePracticeStatus();
        }
        catch (Exception ex) when (ex is COMException or InvalidOperationException or ArgumentException or NotSupportedException)
        {
            output?.Dispose();
            device?.Dispose();
            if (reportErrors)
            {
                Dialogs.Error(this, "Die Audioausgabe konnte nicht gestartet werden.", ex.Message);
            }
            return false;
        }
        return true;
    }

    private void StopPlayback()
    {
        if (_output is null)
        {
            return;
        }

        _output.PlaybackStopped -= Output_PlaybackStopped;
        _output.Dispose();
        _output = null;
        _outputDevice?.Dispose();
        _outputDevice = null;
        playheadTimer.Stop();
        stepGrid.PlayStep = -1;
        stepGrid.BarFlash = false;
        ShowCountIn(0);
        buttonPlay.Text = PlaySymbol;
        buttonPlay.AccessibleName = "Start";
        toolTip.SetToolTip(buttonPlay, "Start (Leertaste)");
        UpdatePatternButtons();
        UpdatePatternCaption();
        UpdateTrainerStatus();
    }

    // ==================== Probehören ====================

    private void StepGrid_TrackNameClicked(object? sender, Drums4me.Controls.TrackNameEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            PreviewTrack(e.Track);
        }
        else if (e.Button == MouseButtons.Right)
        {
            _menuTrack = e.Track;
            renameTrackToolStripMenuItem.Enabled = _kit.IsOwn;
            renameTrackToolStripMenuItem.Text = _kit.IsOwn ? RenameCaption : RenameLockedCaption;
            trackNameMenu.Show(stepGrid, e.Location);
        }
    }

    private void PreviewTrackToolStripMenuItem_Click(object? sender, EventArgs e) => PreviewTrack(_menuTrack);

    /// <summary>Spielt den Klang einer Spur einmal mit ihren Einstellungen. Während der Wiedergabe mischt er sich dazu.</summary>
    private void PreviewTrack(int track)
    {
        if (!_kit.HasTrack(track))
        {
            return;
        }

        var sound = EngineState.From(_project, _kit).Tracks[track];
        if (_output is not null || _previewOutput is not null)
        {
            _engine.Preview(sound);
            return;
        }

        WasapiPlayer? output = null;
        MMDevice? device = null;
        try
        {
            output = CreateOutput(out device);
            output.PlaybackStopped += PreviewOutput_PlaybackStopped;
            _engine.BeginPreview(sound);
            output.Init(_engine.ToWaveProvider());
            output.Play();
            _previewOutput = output;
            _previewDevice = device;
            previewTimer.Start();
        }
        catch (Exception ex) when (ex is COMException or InvalidOperationException or ArgumentException or NotSupportedException)
        {
            output?.Dispose();
            device?.Dispose();
            Dialogs.Error(this, "Die Audioausgabe konnte nicht gestartet werden.", ex.Message);
        }
    }

    private void PreviewTimer_Tick(object? sender, EventArgs e)
    {
        if (_engine.IsFinished)
        {
            StopPreview();
        }
    }

    private void PreviewOutput_PlaybackStopped(object? sender, StoppedEventArgs e) => StopPreview();

    private void StopPreview()
    {
        if (_previewOutput is null)
        {
            return;
        }

        _previewOutput.PlaybackStopped -= PreviewOutput_PlaybackStopped;
        _previewOutput.Dispose();
        _previewOutput = null;
        _previewDevice?.Dispose();
        _previewDevice = null;
        previewTimer.Stop();
    }

    /// <summary>
    /// Gewolltes Stoppen meldet sich vorher ab: Dieses Ereignis bedeutet, dass das Audiogerät weggefallen ist.
    /// Neu gestartet wird erst nach einer Pause, denn solange Windows umbaut, gelten gerade entfernte Geräte noch als vorhanden.
    /// </summary>
    private void Output_PlaybackStopped(object? sender, StoppedEventArgs e)
    {
        StopPlayback();
        if (e.Exception is null)
        {
            return;
        }

        // Fällt das Gerät immer wieder aus, ist etwas grundsätzlich nicht in Ordnung: dann lieber melden als endlos neu starten.
        var now = Stopwatch.GetTimestamp();
        if (now - _firstDeviceLoss > DeviceLossWindowSeconds * Stopwatch.Frequency)
        {
            _firstDeviceLoss = now;
            _deviceLosses = 0;
        }
        if (++_deviceLosses > MaxDeviceLosses)
        {
            Dialogs.Warning(this, "Die Audioausgabe wurde unterbrochen.", e.Exception.Message);
            return;
        }

        _deviceLossMessage = e.Exception.Message;
        _restartTries = 0;
        ShowNotice("Das Ausgabegerät ist weggefallen. Die Wiedergabe startet gleich neu …");
        restartTimer.Start();
    }

    private void RestartTimer_Tick(object? sender, EventArgs e)
    {
        if (_output is not null)
        {
            restartTimer.Stop();
            return;
        }

        _restartTries++;
        if (StartPlayback(reportErrors: false))
        {
            restartTimer.Stop();
            if (string.IsNullOrEmpty(_settings.AudioDeviceId))
            {
                ShowNotice("Das Ausgabegerät ist weggefallen, die Wiedergabe läuft über den Windows-Standard weiter.");
            }
        }
        else if (_restartTries >= MaxRestartTries)
        {
            restartTimer.Stop();
            Dialogs.Warning(this, "Die Audioausgabe wurde unterbrochen.", _deviceLossMessage ?? string.Empty);
        }
    }
    private void PlayheadTimer_Tick(object? sender, EventArgs e)
    {
        if (_engine.IsFinished)
        {
            StopPlayback();
            return;
        }

        // Während einer Abfolge zeigt das Raster immer das gerade klingende Pattern.
        var playing = _engine.CurrentPattern;
        if (_sequenceActive && playing >= 0 && playing != _project.ActivePattern)
        {
            ShowPattern(playing);
        }
        stepGrid.PlayStep = _engine.CurrentStep;
        UpdatePatternCaption();
        ShowCountIn(_engine.CountInBar);
        UpdateBarFlash();
        if (_engine.CurrentStep >= 0)
        {
            _trainerBpm = _engine.CurrentBpm;
        }
        UpdateTrainerStatus();
        UpdatePracticeStatus();
    }

    // ==================== Statuszeile ====================

    /// <summary>Takt und Spielzeit seit dem Start. Nach dem Stoppen bleiben sie stehen, die Zeit beginnt nach dem Vorzählen.</summary>
    private void UpdatePracticeStatus()
    {
        var now = Stopwatch.GetTimestamp();
        if (_engine.BarsPlayed == 0)
        {
            _playStart = now;
        }

        var time = TimeSpan.FromSeconds((now - _playStart) / Stopwatch.Frequency);
        var clock = time.TotalHours >= 1 ? time.ToString(@"h\:mm\:ss") : time.ToString(@"m\:ss");
        SetStatusText(statusBars, $"Takt {_engine.BarsPlayed}");
        SetStatusText(statusTime, $"Spielzeit {clock}");
    }

    private static void SetStatusText(ToolStripStatusLabel label, string text)
    {
        if (label.Text != text)
        {
            label.Text = text;
        }
    }

    /// <summary>Links steht, was das Element unter der Maus tut. Ein Eingabefehler hat Vorrang, bis er behoben ist, eine Meldung für einige Sekunden.</summary>
    private void UpdateStatusHint()
    {
        SetStatusText(statusHint, _statusError ?? _notice ?? _hoverHint ?? string.Empty);
        statusHint.ForeColor = _statusError is null ? SystemColors.ControlText : Theme.Error;
    }

    /// <summary>Beobachtet die Maus an einer Stelle, denn etliche Elemente wie Zahlenfelder melden sie nicht zuverlässig selbst.</summary>
    public bool PreFilterMessage(ref Message m)
    {
        if (m.Msg == WmMouseMove)
        {
            var point = new Point((short)(m.LParam.ToInt64() & 0xFFFF), (short)((m.LParam.ToInt64() >> 16) & 0xFFFF));
            _hoverHint = HintOf(Control.FromChildHandle(m.HWnd), point);
            UpdateStatusHint();
        }
        else if (m.Msg == WmMouseLeave && !ClientRectangle.Contains(PointToClient(Cursor.Position)))
        {
            _hoverHint = null;
            UpdateStatusHint();
        }
        return false;
    }

    private string? HintOf(Control? control, Point point)
    {
        if (control == stepGrid)
        {
            return stepGrid.HintAt(point);
        }

        // Ausgeschaltete Elemente bekommen keine Mausmeldungen, die gehen an die Gruppe.
        if (control is Panel group)
        {
            control = group.GetChildAtPoint(point) ?? group;
        }

        for (; control is not null && control != this; control = control.Parent)
        {
            if (!string.IsNullOrEmpty(control.AccessibleDescription))
            {
                return control.AccessibleDescription;
            }
        }
        return null;
    }

    /// <summary>Blitzt, sobald das Audiogerät den Taktanfang erreicht hat, und damit passend zum Ton statt zur Berechnung.</summary>
    private void UpdateBarFlash()
    {
        if (!_settings.FlashOnBar || _output is null)
        {
            stepGrid.BarFlash = false;
            return;
        }

        long played;
        try
        {
            played = _output.GetPosition() / _output.OutputWaveFormat.BlockAlign;
        }
        catch (COMException)
        {
            return;   // das Gerät ist gerade weggefallen, PlaybackStopped kümmert sich darum
        }

        var now = Stopwatch.GetTimestamp();
        var barStart = _engine.LastBarStart;
        if (barStart >= 0 && barStart != _flashedBar && played >= barStart)
        {
            _flashedBar = barStart;
            _flashEnd = now + (BarFlashMs * Stopwatch.Frequency / 1000);
            stepGrid.BarFlash = true;
        }
        else if (now >= _flashEnd)
        {
            stepGrid.BarFlash = false;
        }
    }

    private void ShowCountIn(int bar)
    {
        var text = bar > 0 ? $"Takt {bar}/{_project.CountInBars}" : CountInCaption;
        if (labelCountIn.Text != text)
        {
            labelCountIn.Text = text;
        }
    }

    private void NumericCountIn_ValueChanged(object? sender, EventArgs e)
    {
        if (!_updatingUi)
        {
            _project.CountInBars = (int)numericCountIn.Value;
            ProjectEdited("countin");
        }
    }

    private void CheckBoxTrainer_CheckedChanged(object? sender, EventArgs e)
    {
        EnableTrainerFields();
        if (_updatingUi)
        {
            return;
        }

        _project.TrainerEnabled = checkBoxTrainer.Checked;
        _trainerBpm = _output is null ? 0 : _project.Bpm;

        // Ein Ziel unter dem Starttempo hätte keine Wirkung.
        if (_project.TrainerEnabled && _project.TrainerTargetBpm <= _project.Bpm)
        {
            _project.TrainerTargetBpm = Math.Min(_project.Bpm + TrainerDefaultRange, Project.MaxBpm);
            _updatingUi = true;
            numericTrainerTarget.Value = _project.TrainerTargetBpm;
            _updatingUi = false;
        }
        ProjectEdited();
        UpdateTrainerStatus();
    }

    private void NumericTrainer_ValueChanged(object? sender, EventArgs e)
    {
        if (!_updatingUi)
        {
            _project.TrainerStepBpm = (int)numericTrainerStep.Value;
            _project.TrainerEveryBars = (int)numericTrainerBars.Value;
            _project.TrainerTargetBpm = (int)numericTrainerTarget.Value;
            ProjectEdited("trainer");
            UpdateTrainerStatus();
        }
    }

    private void EnableTrainerFields()
    {
        numericTrainerStep.Enabled = checkBoxTrainer.Checked;
        numericTrainerBars.Enabled = checkBoxTrainer.Checked;
        numericTrainerTarget.Enabled = checkBoxTrainer.Checked;
    }

    /// <summary>Das Tempo des Trainers steht in der Statuszeile. Nach dem Stoppen bleibt das erreichte Tempo sichtbar.</summary>
    private void UpdateTrainerStatus()
    {
        var text = !_project.TrainerEnabled || _trainerBpm == 0
            ? string.Empty
            : "Tempotrainer: " + (_output is null ? "zuletzt" : "jetzt") + $" {_trainerBpm} BPM"
                + (_trainerBpm >= _project.TrainerTargetBpm ? ", Ziel erreicht" : string.Empty);

        SetStatusText(statusTrainer, text);
        statusTrainer.Visible = text.Length > 0;
    }

    private void CheckBoxOpenLast_CheckedChanged(object? sender, EventArgs e)
    {
        if (_settings.OpenLastFile != checkBoxOpenLast.Checked)
        {
            _settings.OpenLastFile = checkBoxOpenLast.Checked;
            _settings.Save();
        }
    }

    /// <summary>Die Beschriftung steht über dem Schalter, ein Klick darauf schaltet wie bei einer gewöhnlichen Checkbox.</summary>
    private void LabelFlash_Click(object? sender, EventArgs e) => checkBoxFlash.Checked = !checkBoxFlash.Checked;

    private void CheckBoxFlash_CheckedChanged(object? sender, EventArgs e)
    {
        if (_settings.FlashOnBar != checkBoxFlash.Checked)
        {
            _settings.FlashOnBar = checkBoxFlash.Checked;
            _settings.Save();
        }
    }

    private void ShowPattern(int pattern)
    {
        _project.ActivePattern = pattern;
        _updatingUi = true;
        PatternButtons[pattern].Checked = true;
        _updatingUi = false;
        BindGrid();
    }

    /// <summary>Zeigt das aktive Pattern im Raster und seine Taktart in der Auswahl.</summary>
    private void BindGrid()
    {
        stepGrid.Bind(_kit.TrackNames, _project.Tracks, _project.ActiveSteps, _project.ActiveMeter);
        UpdatePatternCaption();

        var wasUpdating = _updatingUi;
        _updatingUi = true;
        comboMeter.SelectedItem = _project.ActiveMeter;
        _updatingUi = wasUpdating;
    }

    /// <summary>Oben links im Raster: das gezeigte Pattern, während einer Abfolge auch der wievielte Takt darin gerade spielt.</summary>
    private void UpdatePatternCaption()
    {
        var length = _project.ActiveSequence.Length;
        var index = _output is null ? -1 : _engine.CurrentSequenceIndex;
        var position = _sequenceActive && index >= 0 && index < length ? $"{index + 1}/{length}" : string.Empty;
        stepGrid.SetPatternCaption($"Pattern {_project.ActivePattern + 1}", position);
    }

    private void ComboMeter_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_updatingUi && comboMeter.SelectedItem is Meter meter)
        {
            _project.Patterns[_project.ActivePattern].MeterName = meter.Name;
            BindGrid();
            ProjectEdited();
        }
    }

    /// <summary>Solange eine Abfolge spielt, bestimmt sie das Pattern. Von Hand lässt es sich dann nicht wählen.</summary>
    private void UpdatePatternButtons()
    {
        var enabled = _output is null || !_sequenceActive;
        foreach (var button in PatternButtons)
        {
            button.Enabled = enabled;
        }
        comboMeter.Enabled = enabled;
    }

    // ==================== Bedienelemente ====================

    private void NumericTempo_ValueChanged(object? sender, EventArgs e)
    {
        if (!_updatingUi)
        {
            _project.Bpm = (int)numericTempo.Value;
            ProjectEdited("tempo");
        }
    }

    private void ButtonTap_Click(object? sender, EventArgs e)
    {
        // Das eigene Metronom würde sonst mitgehört und das Ergebnis bestimmen.
        StopPlayback();
        StopPreview();
        using var dialog = new FrmTempo();
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            numericTempo.Value = dialog.Bpm;
        }
    }

    private void TrackBarSwing_ValueChanged(object? sender, EventArgs e)
    {
        labelSwingValue.Text = $"{trackBarSwing.Value} %";
        if (!_updatingUi)
        {
            _project.SwingPercent = trackBarSwing.Value;
            ProjectEdited("swing");
        }
    }

    private void TrackBarVolume_ValueChanged(object? sender, EventArgs e)
    {
        labelVolumeValue.Text = $"{trackBarVolume.Value} %";
        if (!_updatingUi)
        {
            _project.VolumePercent = trackBarVolume.Value;
            ProjectEdited("volume");
        }
    }

    private void ComboKit_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_updatingUi && comboKit.SelectedItem is DrumKit kit)
        {
            UseKit(kit);
            _project.KitName = kit.Name;
            BindGrid();
            ProjectEdited();
        }
    }

    private void RadioPattern_CheckedChanged(object? sender, EventArgs e)
    {
        if (_updatingUi || sender is not RadioButton { Checked: true } radio)
        {
            return;
        }

        _project.ActivatePattern(Array.IndexOf(PatternButtons, radio));
        BindGrid();
        ProjectEdited();
    }

    private void CheckBoxSequence_CheckedChanged(object? sender, EventArgs e)
    {
        textBoxSequence.Enabled = checkBoxSequence.Checked;
        if (_updatingUi)
        {
            return;
        }

        _project.SequenceEnabled = checkBoxSequence.Checked;
        _project.PrepareSequencePatterns();
        ProjectEdited();
        if (checkBoxSequence.Checked && textBoxSequence.TextLength == 0)
        {
            textBoxSequence.Focus();
        }
    }

    private void CheckBoxRepeat_CheckedChanged(object? sender, EventArgs e)
    {
        if (!_updatingUi)
        {
            _project.SequenceRepeat = checkBoxRepeat.Checked;
            ProjectEdited();
        }
    }

    private void TextBoxSequence_TextChanged(object? sender, EventArgs e)
    {
        var valid = Project.TryParseSequence(textBoxSequence.Text, out var sequence);
        textBoxSequence.ForeColor = valid ? SystemColors.WindowText : Theme.Error;
        _statusError = valid ? null : SequenceError;
        UpdateStatusHint();
        if (valid && !_updatingUi)
        {
            _project.Sequence = Project.FormatSequence(sequence);
            _project.PrepareSequencePatterns();
            ProjectEdited("sequence");
        }
    }

    private void TextBoxSequence_Leave(object? sender, EventArgs e) => ShowSequenceText();

    /// <summary>Zeigt die gültige Abfolge in einheitlicher Schreibweise und verwirft ungültige Eingaben.</summary>
    private void ShowSequenceText()
    {
        _updatingUi = true;
        textBoxSequence.Text = _project.Sequence;
        _updatingUi = false;
    }

    private void StepGrid_Edited(object? sender, EventArgs e) => ProjectEdited();

    private void StepGrid_EditCompleted(object? sender, EventArgs e) => CommitHistory();

    private void ButtonClearPattern_Click(object? sender, EventArgs e)
    {
        foreach (var row in _project.ActiveSteps)
        {
            Array.Clear(row);
        }
        stepGrid.Invalidate();
        ProjectEdited();
        ShowNotice("Pattern geleert. Strg+Z holt es zurück.");
    }

    /// <summary>Zeigt eine kurze Rückmeldung in der Statuszeile, die nach einigen Sekunden wieder verschwindet.</summary>
    private void ShowNotice(string text)
    {
        _notice = text;
        noticeTimer.Stop();
        noticeTimer.Start();
        UpdateStatusHint();
    }

    private void ClearNotice()
    {
        noticeTimer.Stop();
        if (_notice is not null)
        {
            _notice = null;
            UpdateStatusHint();
        }
    }

    private void NoticeTimer_Tick(object? sender, EventArgs e) => ClearNotice();

    private RadioButton[] PatternButtons => [radioPattern1, radioPattern2, radioPattern3, radioPattern4];

    // ==================== Projektzustand ====================

    /// <param name="mergeKey">Änderungen mit gleichem Schlüssel fasst der Verlauf zusammen, wenn sie kurz aufeinander folgen.</param>
    private void ProjectEdited(string? mergeKey = null)
    {
        PublishState();
        _dirty = CurrentChanges().Count > 0;
        UpdateTitle();

        // Während eines Zugs im Raster sammelt sich alles, gespeichert wird der Stand am Ende des Zugs.
        if (!stepGrid.IsEditing)
        {
            CommitHistory(mergeKey);
        }
    }

    private void CommitHistory(string? mergeKey = null)
    {
        _history.Commit(_project, mergeKey);
        UpdateUndoMenu();
    }

    private void UpdateUndoMenu()
    {
        buttonUndo.Enabled = CanEditProject && _history.CanUndo;
        buttonRedo.Enabled = CanEditProject && _history.CanRedo;
    }

    private void ButtonUndo_Click(object? sender, EventArgs e)
    {
        if (CanEditProject && !stepGrid.IsEditing && _history.Undo(_project) is { } state)
        {
            RestoreProject(state);
        }
    }

    private void ButtonRedo_Click(object? sender, EventArgs e)
    {
        if (CanEditProject && !stepGrid.IsEditing && _history.Redo(_project) is { } state)
        {
            RestoreProject(state);
        }
    }

    /// <summary>Übernimmt einen Stand aus dem Verlauf. Die Wiedergabe läuft dabei einfach mit den neuen Daten weiter.</summary>
    private void RestoreProject(Project state)
    {
        ClearNotice();
        _project = state;
        ApplyProject();
        _dirty = CurrentChanges().Count > 0;
        UpdateTitle();
        UpdateUndoMenu();
    }

    /// <summary>Was sich gegenüber dem zuletzt geladenen oder gespeicherten Stand geändert hat.</summary>
    private List<string> CurrentChanges() => ProjectChanges.Describe(_savedProject, _project, _kit.TrackNames);

    private void PublishState()
    {
        _sequenceActive = _project.ActiveSequence.Length > 0;

        // Wiederholen hat nur Bedeutung, wenn eine Abfolge mit Inhalt eingeschaltet ist.
        checkBoxRepeat.Enabled = _sequenceActive;
        _engine.Update(EngineState.From(_project, _kit));
        UpdatePatternButtons();
    }

    /// <summary>Überträgt das geladene Projekt vollständig auf Oberfläche und Engine.</summary>
    private void ApplyProject()
    {
        _updatingUi = true;
        numericTempo.Value = _project.Bpm;
        trackBarSwing.Value = _project.SwingPercent;
        trackBarVolume.Value = _project.VolumePercent;
        labelSwingValue.Text = $"{_project.SwingPercent} %";
        labelVolumeValue.Text = $"{_project.VolumePercent} %";
        PatternButtons[_project.ActivePattern].Checked = true;
        checkBoxSequence.Checked = _project.SequenceEnabled;
        textBoxSequence.Enabled = _project.SequenceEnabled;
        textBoxSequence.Text = _project.Sequence;
        checkBoxRepeat.Checked = _project.SequenceRepeat;
        numericCountIn.Value = _project.CountInBars;
        checkBoxTrainer.Checked = _project.TrainerEnabled;
        numericTrainerStep.Value = _project.TrainerStepBpm;
        numericTrainerBars.Value = _project.TrainerEveryBars;
        numericTrainerTarget.Value = _project.TrainerTargetBpm;
        EnableTrainerFields();
        if (_output is null)
        {
            _trainerBpm = 0;
        }
        UpdateTrainerStatus();
        _project.PrepareSequencePatterns();

        var kit = _kits.Find(IsProjectKit) ?? _kits.FirstOrDefault() ?? DrumKit.Empty;
        comboKit.SelectedItem = kit;
        _updatingUi = false;

        UseKit(kit);
        BindGrid();
        PublishState();
        UpdateTitle();
    }

    private void UseKit(DrumKit kit)
    {
        var failed = kit.EnsureLoaded();
        _kit = kit;
        if (failed.Count > 0)
        {
            Dialogs.Warning(this, $"Diese Dateien aus „{kit.Name}“ konnten nicht gelesen werden:", string.Join("\n", failed));
        }
    }

    private void UpdateTitle()
    {
        var name = _filePath is null ? "Unbenannt" : Path.GetFileNameWithoutExtension(_filePath);
        Text = $"{AppSettings.AppName} – {name}{(_dirty ? " *" : string.Empty)}";
    }

    // ==================== Kits ====================

    private void ScanKits()
    {
        _kits = DrumKit.Scan(SharedKitFolder, KitFolder);
        comboKit.Items.Clear();
        comboKit.Items.AddRange([.. _kits]);
    }

    /// <summary>Die aufgeklappte Liste nennt auch den Ordner des Kits. Im geschlossenen Feld ist dafür zu wenig Platz.</summary>
    private void ComboKit_DrawItem(object? sender, DrawItemEventArgs e)
    {
        e.DrawBackground();
        if (e.Index >= 0 && comboKit.Items[e.Index] is DrumKit kit)
        {
            var text = e.State.HasFlag(DrawItemState.ComboBoxEdit) ? kit.Name : kit.ToString();
            TextRenderer.DrawText(e.Graphics, text, e.Font ?? comboKit.Font, e.Bounds, e.ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }
        e.DrawFocusRectangle();
    }

    /// <summary>Auf Groß- und Kleinschreibung kommt es nicht an, denn ältere Projekte kennen Namen wie „Rnb“ statt „RnB“.</summary>
    private bool IsProjectKit(DrumKit kit) => string.Equals(kit.Name, _project.KitName, StringComparison.OrdinalIgnoreCase);

    private void RenameTrackToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        var track = _menuTrack;
        using var dialog = new FrmRename(_kit.TrackNames[track], name => _kit.ValidateTrackName(track, name));
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            _kit.RenameTrack(track, dialog.NewName);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            Dialogs.Error(this, "Die Spur konnte nicht umbenannt werden.", ex.Message);
        }
        stepGrid.Invalidate();
    }

    private void ShowNoKitsMessage() =>
        Dialogs.Info(this, "Es wurden keine Kits gefunden.", $"Installiere ein Kit-Paket, oder lege im Ordner für eigene Kits für jedes Kit einen Unterordner mit bis zu acht WAV-Dateien an:\n{KitFolder}\n\nDu öffnest ihn auf der Karte „Einstellungen“.\n\nDie Spuren erscheinen in alphabetischer Reihenfolge der Dateinamen. Mit einer Zahl am Anfang des Namens legst du die Reihenfolge selbst fest, zum Beispiel „1 Crash.wav“. Die Zahl wird nicht angezeigt.");

    // ==================== Dateien ====================

    private void ButtonNew_Click(object? sender, EventArgs e)
    {
        if (ConfirmDiscard())
        {
            ReplaceProject(Project.CreateDefault());
            if (projectBrowser.Visible)
            {
                ShowProjectBrowser(false);
            }
        }
    }

    /// <summary>Wie in CalcTape: Die Datei ist zu, das Raster leer. Du bleibst auf der Karte „Datei“, um gleich ein anderes Projekt zu wählen.</summary>
    private void ButtonCloseProject_Click(object? sender, EventArgs e)
    {
        if (!ConfirmDiscard())
        {
            return;
        }

        var name = _filePath is null ? null : Path.GetFileNameWithoutExtension(_filePath);
        ReplaceProject(Project.CreateEmpty());

        // Eine geschlossene Datei soll beim nächsten Start nicht wieder aufgehen.
        _settings.LastFile = null;
        _settings.Save();
        if (projectBrowser.Visible)
        {
            RefreshProjectBrowser();
            FocusProjectList();
        }
        ShowNotice(name is null ? "Projekt geschlossen, das Raster ist leer." : $"„{name}“ geschlossen, das Raster ist leer.");
    }

    /// <summary>Ein neues, ungespeichertes Projekt ohne Vorgeschichte.</summary>
    private void ReplaceProject(Project project)
    {
        _project = project;
        _savedProject = _project.Snapshot();
        _history.Reset(_project);
        UpdateUndoMenu();
        _filePath = null;
        _dirty = false;
        ApplyProject();
    }

    /// <summary>Größe und Lage wie beim ersten Start, mittig auf dem Bildschirm, auf dem das Fenster gerade steht.</summary>
    private void ButtonResetLayout_Click(object? sender, EventArgs e)
    {
        WindowState = FormWindowState.Normal;
        Size = _defaultSize;
        CenterToScreen();
        projectBrowser.ResetColumnWidths();
        ShowNotice("Fensterlayout zurückgesetzt.");
    }

    private void ButtonExit_Click(object? sender, EventArgs e) => Close();

    /// <summary>Baut die Liste bei jedem Öffnen neu auf, damit neu installierte Pakete sofort erscheinen.</summary>
    private void ButtonDemos_Click(object? sender, EventArgs e)
    {
        var menu = demosMenu.Items;

        // Die Einträge vom letzten Mal freigeben. Das Bild gehört dem Knopf und muss erhalten bleiben.
        foreach (var old in menu.OfType<ToolStripItem>().Where(item => item != noDemosToolStripMenuItem).ToList())
        {
            if (old is ToolStripMenuItem oldItem)
            {
                oldItem.Click -= DemoItem_Click;
                oldItem.Image = null;
            }
            old.Dispose();
        }
        menu.Clear();

        string? lastFolder = null;
        foreach (var path in FindDemos())
        {
            var folder = Path.GetDirectoryName(path);
            if (lastFolder is not null && folder != lastFolder)
            {
                menu.Add(new ToolStripSeparator());
            }
            lastFolder = folder;

            var item = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(path), buttonDemos.Image) { Tag = path };
            item.Click += DemoItem_Click;
            menu.Add(item);
        }

        if (menu.Count == 0)
        {
            menu.Add(noDemosToolStripMenuItem);
        }
        demosMenu.Show(buttonDemos, new Point(buttonDemos.Width, 0));
    }

    private static List<string> FindDemos()
    {
        try
        {
            return Directory.Exists(SharedDemoFolder)
                ? [.. Directory.GetFiles(SharedDemoFolder, "*.d4m", SearchOption.AllDirectories).OrderBy(path => path, StringComparer.OrdinalIgnoreCase)]
                : [];
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return [];
        }
    }

    private void DemoItem_Click(object? sender, EventArgs e)
    {
        if (sender is ToolStripItem { Tag: string path } && ConfirmDiscard())
        {
            LoadProjectFile(path);
        }
    }

    /// <summary>Demos bleiben unverändert: Speichern fragt bei ihnen immer nach einem eigenen Dateinamen.</summary>
    private static bool IsDemoFile(string path) =>
        Path.GetFullPath(path).StartsWith(SharedDemoFolder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

    /// <summary>Ein weiterer Programmstart hat eine Datei übergeben, oder null, wenn nur das Fenster nach vorn soll.</summary>
    internal void HandleExternalOpen(string? path)
    {
        if (WindowState == FormWindowState.Minimized)
        {
            WindowState = FormWindowState.Normal;
        }
        Activate();

        if (path is not null)
        {
            _pendingFile = path;
            pendingOpenTimer.Start();
        }
    }

    /// <summary>Wartet, bis kein Dialog mehr offen ist. So laufen nie zwei Rückfragen gleichzeitig.</summary>
    private void PendingOpenTimer_Tick(object? sender, EventArgs e)
    {
        if (!NativeMethods.IsWindowEnabled(Handle))
        {
            return;
        }

        pendingOpenTimer.Stop();
        var path = _pendingFile;
        _pendingFile = null;
        if (path is null || (!_dirty && string.Equals(path, _filePath, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        if (ConfirmDiscard())
        {
            LoadProjectFile(path);
        }
    }

    /// <summary>Lädt ein Projekt und merkt es sich für den nächsten Start. Bei einem Fehler bleibt das bisherige Projekt erhalten.</summary>
    private bool LoadProjectFile(string path)
    {
        try
        {
            _project = Project.Load(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or InvalidDataException)
        {
            var reason = ex switch
            {
                JsonException => "Die Datei ist keine gültige drums4me-Projektdatei.",
                FileNotFoundException or DirectoryNotFoundException => "Die Datei wurde nicht gefunden.",
                _ => ex.Message,
            };
            Dialogs.Error(this, $"Das Projekt „{Path.GetFileName(path)}“ konnte nicht geöffnet werden.", reason);
            if (path == _settings.LastFile)
            {
                RememberFile(null);   // sonst käme die Meldung bei jedem Start wieder
            }
            return false;
        }

        // Geöffnet wird am Anfang, nicht bei dem Pattern, das beim Speichern zufällig zu sehen war:
        // mit Abfolge bei dem Pattern, mit dem sie beginnt, sonst bei Pattern 1.
        _project.ActivePattern = _project.ActiveSequence is [var first, ..] ? first : 0;

        _filePath = path;
        _dirty = false;
        RememberFile(path);
        var missingKit = _kits.Count > 0 && !_kits.Exists(IsProjectKit);
        var wantedKit = _project.KitName;
        ApplyProject();
        _savedProject = _project.Snapshot();   // erst hier, denn eine Abfolge kann beim Anwenden noch Patterns vorbereiten
        _history.Reset(_project);
        UpdateUndoMenu();
        if (projectBrowser.Visible)
        {
            ShowProjectBrowser(false);
        }
        if (missingKit)
        {
            Dialogs.Info(this, $"Das Kit „{wantedKit}“ wurde nicht gefunden.", $"Stattdessen wird „{_kit.Name}“ benutzt.");
        }
        return true;
    }

    private void RememberFile(string? path)
    {
        _settings.LastFile = path;
        _settings.Save();
    }

    private void ButtonSave_Click(object? sender, EventArgs e) => SaveProject();

    private void ButtonSaveAs_Click(object? sender, EventArgs e) => SaveProjectAs();

    private void ButtonAbout_Click(object? sender, EventArgs e) => Dialogs.ShowAbout(this);

    private void ButtonShortcuts_Click(object? sender, EventArgs e) => Dialogs.ShowShortcuts(this);

    // ==================== Karte Einstellungen ====================

    private void ButtonProjectFolder_Click(object? sender, EventArgs e)
    {
        projectFolderDialog.InitialDirectory = _projectFolder;
        if (projectFolderDialog.ShowDialog(this) == DialogResult.OK && projectFolderDialog.SelectedPath != _projectFolder)
        {
            _settings.ProjectFolder = projectFolderDialog.SelectedPath;
            ApplyProjectFolder();
            _settings.Save();
        }
    }

    /// <summary>Liest die Geräte bei jedem Aufklappen neu, damit ein gerade angestecktes Interface gleich erscheint.</summary>
    private void FillAudioDevices()
    {
        var choices = AudioDevices.List();
        var id = _settings.AudioDeviceId;
        if (!string.IsNullOrEmpty(id) && !choices.Exists(choice => choice.Id == id))
        {
            choices.Add(new AudioDeviceChoice(id, $"{_settings.AudioDeviceName} (nicht angeschlossen)"));
        }

        _updatingUi = true;
        comboAudioDevice.Items.Clear();
        comboAudioDevice.Items.AddRange([.. choices]);
        comboAudioDevice.SelectedItem = choices.Find(choice => choice.Id == (string.IsNullOrEmpty(id) ? null : id)) ?? choices[0];
        _updatingUi = false;
    }

    private void ComboAudioDevice_DropDown(object? sender, EventArgs e) => FillAudioDevices();

    private void ComboAudioDevice_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_updatingUi || comboAudioDevice.SelectedItem is not AudioDeviceChoice choice || choice.Id == _settings.AudioDeviceId)
        {
            return;
        }

        _settings.AudioDeviceId = choice.Id;
        _settings.AudioDeviceName = choice.Id is null ? null : choice.Name;
        _settings.Save();

        // Eine laufende Wiedergabe wechselt sofort auf das neue Gerät.
        StopPreview();
        if (_output is not null)
        {
            StopPlayback();
            StartPlayback();
        }
    }

    private void ShowColorMode()
    {
        _updatingUi = true;
        radioSystem.Checked = _settings.ColorMode == SystemColorMode.System;
        radioLight.Checked = _settings.ColorMode == SystemColorMode.Classic;
        radioDark.Checked = _settings.ColorMode == SystemColorMode.Dark;
        _updatingUi = false;
    }

    private void RadioColorMode_CheckedChanged(object? sender, EventArgs e)
    {
        if (!_updatingUi && sender is RadioButton { Checked: true })
        {
            _settings.ColorMode = radioDark.Checked ? SystemColorMode.Dark : radioLight.Checked ? SystemColorMode.Classic : SystemColorMode.System;
            _settings.Save();
        }
    }

    private void ButtonOwnKits_Click(object? sender, EventArgs e)
    {
        Directory.CreateDirectory(KitFolder);
        Process.Start(new ProcessStartInfo(KitFolder) { UseShellExecute = true });
    }

    private void ButtonInstalledKits_Click(object? sender, EventArgs e)
    {
        if (Directory.Exists(SharedKitFolder))
        {
            Process.Start(new ProcessStartInfo(SharedKitFolder) { UseShellExecute = true });
        }
        else
        {
            Dialogs.Info(this, "Es ist noch kein Kit-Paket installiert.", $"Nach der Installation eines Pakets liegen die Kits hier:\n{SharedKitFolder}");
        }
    }

    private void ButtonReloadKits_Click(object? sender, EventArgs e)
    {
        ScanKits();
        ApplyProject();
        ShowKitCount();
    }

    private void ShowKitCount() => labelKitCount.Text = _kits.Count switch
    {
        0 => "Keine Kits gefunden",
        1 => "1 Kit gefunden",
        _ => $"{_kits.Count} Kits gefunden",
    };

    /// <summary>Stellt den Projektordner ein. Ist der gewählte Ordner nicht erreichbar, gilt vorerst der Standardordner.</summary>
    private void ApplyProjectFolder()
    {
        _projectFolder = string.IsNullOrWhiteSpace(_settings.ProjectFolder) ? DefaultProjectFolder : _settings.ProjectFolder;
        try
        {
            Directory.CreateDirectory(_projectFolder);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            _projectFolder = DefaultProjectFolder;
            Directory.CreateDirectory(_projectFolder);
        }

        _otherFolder = null;
        saveFileDialog.InitialDirectory = _projectFolder;
        textBoxProjectFolder.Text = _projectFolder;
    }

    private bool SaveProject() => _filePath is null || IsDemoFile(_filePath) ? SaveProjectAs() : WriteProject(_filePath);

    private bool SaveProjectAs()
    {
        saveFileDialog.FileName = _filePath is null ? "Unbenannt" : Path.GetFileName(_filePath);
        return saveFileDialog.ShowDialog(this) == DialogResult.OK && WriteProject(saveFileDialog.FileName);
    }

    private bool WriteProject(string path)
    {
        try
        {
            _project.Save(path);
            _savedProject = _project.Snapshot();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Dialogs.Error(this, "Das Projekt konnte nicht gespeichert werden.", ex.Message);
            return false;
        }

        _filePath = path;
        _dirty = false;
        RememberFile(path);
        UpdateTitle();
        if (projectBrowser.Visible)
        {
            RefreshProjectBrowser();
        }
        return true;
    }

    /// <summary>Fragt bei ungespeicherten Änderungen nach. Liefert true, wenn es weitergehen darf.</summary>
    private bool ConfirmDiscard()
    {
        if (!_dirty)
        {
            return true;
        }

        var name = _filePath is null ? "Unbenannt" : Path.GetFileNameWithoutExtension(_filePath);
        return Dialogs.AskSaveChanges(this, name, CurrentChanges()) switch
        {
            DialogResult.Yes => SaveProject(),
            DialogResult.No => true,
            _ => false,
        };
    }
}
