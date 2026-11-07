using Drums4me.Classes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using NAudio.Wave;

namespace Drums4me.Forms;

public partial class FrmDrums4me : Form
{
    private const int OutputLatencyMs = 40;
    private const int BarFlashMs = 120;
    private const string CountInCaption = "Vorzählen";
    private const string SequenceHint = "Ein Pattern pro Takt, z. B. 1 1 1 2";
    private const string SequenceError = "Erlaubt sind nur die Ziffern 1 bis 4.";

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
    private string _projectFolder = DefaultProjectFolder;
    private readonly string? _startFile;
    private string? _pendingFile;
    private long _flashedBar = -1;
    private long _flashEnd;

    public FrmDrums4me()
    {
        InitializeComponent();
        _savedProject = _project.Snapshot();
        _history = new UndoHistory(_project);
        _engine = new SequencerEngine(EngineState.From(_project, _kit));
    }

    /// <summary>Startet mit der angegebenen Datei statt mit der zuletzt benutzten.</summary>
    public FrmDrums4me(string? startFile) : this() => _startFile = startFile;

    // ==================== Start und Ende ====================

    private void FrmDrums4me_Load(object? sender, EventArgs e)
    {
        Directory.CreateDirectory(KitFolder);
        ApplyProjectFolder();
        checkBoxFlash.Checked = _settings.FlashOnBar;
        comboMeter.Items.AddRange([.. Meter.All]);

        ScanKits();
        var opened = _startFile is not null
            ? LoadProjectFile(_startFile)
            : _settings.LastFile is { } lastFile && File.Exists(lastFile) && LoadProjectFile(lastFile);
        if (!opened)
        {
            ApplyProject();
        }

        if (_kits.Count == 0)
        {
            ShowNoKitsMessage();
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
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // Im Abfolge-Feld nimmt Strg+Z die Texteingabe zurück, nicht die letzte Projektänderung.
        if (keyData is (Keys.Control | Keys.Z) or (Keys.Control | Keys.Y) && ActiveControl is TextBox)
        {
            return false;
        }

        if (keyData == Keys.Space && ActiveControl is not TextBox)
        {
            TogglePlayback();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    // ==================== Wiedergabe ====================

    private void ButtonPlay_Click(object? sender, EventArgs e) => TogglePlayback();

    private void TogglePlayback()
    {
        if (_output is null)
        {
            StartPlayback();
        }
        else
        {
            StopPlayback();
        }
    }

    private void StartPlayback()
    {
        WasapiPlayer? output = null;
        try
        {
            output = new WasapiPlayerBuilder()
                .WithSharedMode()
                .WithEventSync()
                .WithLatency(OutputLatencyMs)
                .WithMmcssThreadPriority("Pro Audio")
                .Build();
            output.PlaybackStopped += Output_PlaybackStopped;
            _engine.Rewind();
            _flashedBar = -1;
            output.Init(_engine.ToWaveProvider());
            output.Play();
            _output = output;
            buttonPlay.Text = "■ Stopp";
            playheadTimer.Start();
            UpdatePatternButtons();
        }
        catch (Exception ex) when (ex is COMException or InvalidOperationException or ArgumentException or NotSupportedException)
        {
            output?.Dispose();
            Dialogs.Error(this, "Die Audioausgabe konnte nicht gestartet werden.", ex.Message);
        }
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
        playheadTimer.Stop();
        stepGrid.PlayStep = -1;
        stepGrid.BarFlash = false;
        ShowCountIn(0);
        buttonPlay.Text = "▶ Start";
        UpdatePatternButtons();
    }

    private void Output_PlaybackStopped(object? sender, StoppedEventArgs e)
    {
        // Gewolltes Stoppen meldet sich vorher ab: Dieses Ereignis bedeutet, dass das Audiogerät weggefallen ist.
        StopPlayback();
        if (e.Exception is not null)
        {
            Dialogs.Warning(this, "Die Audioausgabe wurde unterbrochen.", e.Exception.Message);
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
        ShowCountIn(_engine.CountInBar);
        UpdateBarFlash();
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

        var wasUpdating = _updatingUi;
        _updatingUi = true;
        comboMeter.SelectedItem = _project.ActiveMeter;
        _updatingUi = wasUpdating;
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
        using var dialog = new FrmTap();
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            numericTempo.Value = dialog.Bpm;
        }
    }

    private void TrackBarSwing_ValueChanged(object? sender, EventArgs e)
    {
        labelSwing.Text = $"Swing {trackBarSwing.Value} %";
        if (!_updatingUi)
        {
            _project.SwingPercent = trackBarSwing.Value;
            ProjectEdited("swing");
        }
    }

    private void TrackBarVolume_ValueChanged(object? sender, EventArgs e)
    {
        labelVolume.Text = $"Lautstärke {trackBarVolume.Value} %";
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
        labelSequenceHint.Text = valid ? SequenceHint : SequenceError;
        labelSequenceHint.ForeColor = valid ? SystemColors.GrayText : Color.Firebrick;
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

    private void ClearPatternToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        foreach (var row in _project.ActiveSteps)
        {
            Array.Clear(row);
        }
        stepGrid.Invalidate();
        ProjectEdited();
    }

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
        undoToolStripMenuItem.Enabled = _history.CanUndo;
        redoToolStripMenuItem.Enabled = _history.CanRedo;
    }

    private void UndoToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        if (!stepGrid.IsEditing && _history.Undo(_project) is { } state)
        {
            RestoreProject(state);
        }
    }

    private void RedoToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        if (!stepGrid.IsEditing && _history.Redo(_project) is { } state)
        {
            RestoreProject(state);
        }
    }

    /// <summary>Übernimmt einen Stand aus dem Verlauf. Die Wiedergabe läuft dabei einfach mit den neuen Daten weiter.</summary>
    private void RestoreProject(Project state)
    {
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
        labelSwing.Text = $"Swing {_project.SwingPercent} %";
        labelVolume.Text = $"Lautstärke {_project.VolumePercent} %";
        PatternButtons[_project.ActivePattern].Checked = true;
        checkBoxSequence.Checked = _project.SequenceEnabled;
        textBoxSequence.Enabled = _project.SequenceEnabled;
        textBoxSequence.Text = _project.Sequence;
        checkBoxRepeat.Checked = _project.SequenceRepeat;
        numericCountIn.Value = _project.CountInBars;
        _project.PrepareSequencePatterns();

        var kit = _kits.Find(candidate => candidate.Name == _project.KitName) ?? _kits.FirstOrDefault() ?? DrumKit.Empty;
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
        _kits = DrumKit.ScanFolders(SharedKitFolder, KitFolder);   // eigene Kits haben Vorrang vor installierten
        comboKit.Items.Clear();
        comboKit.Items.AddRange([.. _kits]);
    }

    private void ShowNoKitsMessage() =>
        Dialogs.Info(this, "Es wurden keine Kits gefunden.", $"Installiere ein Kit-Paket, oder lege in deinem Kit-Ordner für jedes Kit einen Unterordner mit bis zu acht WAV-Dateien an:\n{KitFolder}\n\nDie Spuren erscheinen in alphabetischer Reihenfolge der Dateinamen. Mit einer Zahl am Anfang des Namens legst du die Reihenfolge selbst fest, zum Beispiel „1 Crash.wav“. Die Zahl wird nicht angezeigt.");

    private void OpenKitFolderToolStripMenuItem_Click(object? sender, EventArgs e) =>
        Process.Start(new ProcessStartInfo(KitFolder) { UseShellExecute = true });

    private void ReloadKitsToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        ScanKits();
        ApplyProject();
        if (_kits.Count == 0)
        {
            ShowNoKitsMessage();
        }
    }

    // ==================== Dateien ====================

    private void NewToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        if (ConfirmDiscard())
        {
            _project = Project.CreateDefault();
            _savedProject = _project.Snapshot();
            _history.Reset(_project);
            UpdateUndoMenu();
            _filePath = null;
            _dirty = false;
            ApplyProject();
        }
    }

    /// <summary>Baut die Liste bei jedem Öffnen neu auf, damit neu installierte Pakete sofort erscheinen.</summary>
    private void DemosToolStripMenuItem_DropDownOpening(object? sender, EventArgs e)
    {
        var menu = demosToolStripMenuItem.DropDownItems;
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

            var item = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(path), demosToolStripMenuItem.Image) { Tag = path };
            item.Click += DemoItem_Click;
            menu.Add(item);
        }

        if (menu.Count == 0)
        {
            menu.Add(noDemosToolStripMenuItem);
        }
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

    private void OpenToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        if (ConfirmDiscard() && openFileDialog.ShowDialog(this) == DialogResult.OK)
        {
            LoadProjectFile(openFileDialog.FileName);
        }
    }

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
        if (!IsWindowEnabled(Handle))
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

    [DllImport("user32.dll")]
    private static extern bool IsWindowEnabled(IntPtr window);

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

        _filePath = path;
        _dirty = false;
        RememberFile(path);
        var missingKit = _kits.Count > 0 && !_kits.Exists(kit => kit.Name == _project.KitName);
        var wantedKit = _project.KitName;
        ApplyProject();
        _savedProject = _project.Snapshot();   // erst hier, denn eine Abfolge kann beim Anwenden noch Patterns vorbereiten
        _history.Reset(_project);
        UpdateUndoMenu();
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

    private void SaveToolStripMenuItem_Click(object? sender, EventArgs e) => SaveProject();

    private void SaveAsToolStripMenuItem_Click(object? sender, EventArgs e) => SaveProjectAs();

    private void ExitToolStripMenuItem_Click(object? sender, EventArgs e) => Close();

    private void AboutToolStripMenuItem_Click(object? sender, EventArgs e) => Dialogs.ShowAbout(this);

    private void ProjectFolderToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        folderBrowserDialog.InitialDirectory = _projectFolder;
        if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _settings.ProjectFolder = folderBrowserDialog.SelectedPath;
        _settings.Save();
        ApplyProjectFolder();
        Dialogs.Info(this, "Der Speicherort für Projekte wurde geändert.", $"{_projectFolder}\n\nDie Datei-Dialoge beginnen künftig dort. Vorhandene Projekte bleiben an ihrem bisherigen Ort.");
    }

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

        openFileDialog.InitialDirectory = _projectFolder;
        saveFileDialog.InitialDirectory = _projectFolder;
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
