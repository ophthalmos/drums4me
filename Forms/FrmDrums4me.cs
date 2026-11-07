using Drums4me.Classes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using NAudio.Wave;

namespace Drums4me.Forms;

public partial class FrmDrums4me : Form, IMessageFilter
{
    private const int OutputLatencyMs = 40;
    private const int BarFlashMs = 120;
    private const int TempoKeyStep = 1;
    private const int TempoPageStep = 5;
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
    private string _projectFolder = DefaultProjectFolder;
    private readonly string? _startFile;
    private string? _pendingFile;
    private long _flashedBar = -1;
    private long _flashEnd;
    private int _trainerBpm;
    private WasapiPlayer? _previewOutput;
    private int _menuTrack;
    private long _playStart;
    private string? _hoverHint;
    private string? _statusError;

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
        if (Application.IsDarkModeEnabled)
        {
            // Die Trennlinien der Statuszeile wären auf Dunkel grell weiß.
            statusBars.BorderSides = ToolStripStatusLabelBorderSides.None;
            statusTime.BorderSides = ToolStripStatusLabelBorderSides.None;
        }
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
        StopPreview();
        Application.RemoveMessageFilter(this);
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

        var tempoChange = keyData switch
        {
            Keys.Up or Keys.Right => TempoKeyStep,
            Keys.Down or Keys.Left => -TempoKeyStep,
            Keys.PageUp => TempoPageStep,
            Keys.PageDown => -TempoPageStep,
            _ => 0,
        };
        if (tempoChange != 0 && !ActiveControlNeedsArrowKeys())
        {
            ChangeTempo(tempoChange);
            return true;
        }

        // Plus und Minus brauchen die anderen Elemente nicht, sie wirken überall außer im Abfolge-Feld.
        var plusMinus = keyData switch
        {
            Keys.Oemplus or Keys.Add => TempoKeyStep,
            Keys.OemMinus or Keys.Subtract => -TempoKeyStep,
            (Keys.Control | Keys.Oemplus) or (Keys.Control | Keys.Add) => TempoPageStep,
            (Keys.Control | Keys.OemMinus) or (Keys.Control | Keys.Subtract) => -TempoPageStep,
            _ => 0,
        };
        if (plusMinus != 0 && ActiveControl is not TextBox)
        {
            ChangeTempo(plusMinus);
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void ChangeTempo(int change) =>
        numericTempo.Value = Math.Clamp(numericTempo.Value + change, numericTempo.Minimum, numericTempo.Maximum);

    /// <summary>Beim Spielen gehören die Tasten dem Tempo, damit es sich ohne Blick auf den Fokus ändern lässt.</summary>
    private bool ActiveControlNeedsArrowKeys() => ActiveControl switch
    {
        TextBox => true,
        ComboBox combo => combo.DroppedDown,
        NumericUpDown numeric when numeric != numericTempo => _output is null,
        TrackBar => _output is null,
        _ => false,
    };

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

    private static WasapiPlayer CreateOutput() => new WasapiPlayerBuilder()
        .WithSharedMode()
        .WithEventSync()
        .WithLatency(OutputLatencyMs)
        .WithMmcssThreadPriority("Pro Audio")
        .Build();

    private void StartPlayback()
    {
        StopPreview();
        WasapiPlayer? output = null;
        try
        {
            output = CreateOutput();
            output.PlaybackStopped += Output_PlaybackStopped;
            _engine.Rewind();
            _flashedBar = -1;
            output.Init(_engine.ToWaveProvider());
            output.Play();
            _output = output;
            buttonPlay.Text = "■ Stopp";
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
        try
        {
            output = CreateOutput();
            output.PlaybackStopped += PreviewOutput_PlaybackStopped;
            _engine.BeginPreview(sound);
            output.Init(_engine.ToWaveProvider());
            output.Play();
            _previewOutput = output;
            previewTimer.Start();
        }
        catch (Exception ex) when (ex is COMException or InvalidOperationException or ArgumentException or NotSupportedException)
        {
            output?.Dispose();
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
        previewTimer.Stop();
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

    /// <summary>Links steht, was das Element unter der Maus tut. Ein Eingabefehler hat Vorrang, bis er behoben ist.</summary>
    private void UpdateStatusHint()
    {
        SetStatusText(statusHint, _statusError ?? _hoverHint ?? string.Empty);
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

        // Ausgeschaltete Elemente bekommen keine Mausmeldungen, die gehen an die Zeile.
        if (control is FlowLayoutPanel row)
        {
            control = row.GetChildAtPoint(point) ?? row;
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

    /// <summary>Das Tempo des Trainers steht neben seinen Reglern. Nach dem Stoppen bleibt das erreichte Tempo sichtbar.</summary>
    private void UpdateTrainerStatus()
    {
        var text = !_project.TrainerEnabled || _trainerBpm == 0
            ? string.Empty
            : (_output is null ? "Zuletzt" : "Jetzt") + $" {_trainerBpm} BPM"
                + (_trainerBpm >= _project.TrainerTargetBpm ? ", Ziel erreicht" : string.Empty);

        if (labelTrainerStatus.Text != text)
        {
            labelTrainerStatus.Text = text;
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
        Dialogs.Info(this, "Es wurden keine Kits gefunden.", $"Installiere ein Kit-Paket, oder lege im Ordner für eigene Kits für jedes Kit einen Unterordner mit bis zu acht WAV-Dateien an:\n{KitFolder}\n\nDu öffnest ihn über Einstellungen, Eigene Kits öffnen.\n\nDie Spuren erscheinen in alphabetischer Reihenfolge der Dateinamen. Mit einer Zahl am Anfang des Namens legst du die Reihenfolge selbst fest, zum Beispiel „1 Crash.wav“. Die Zahl wird nicht angezeigt.");

    private void OwnKitsToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        Directory.CreateDirectory(KitFolder);
        Process.Start(new ProcessStartInfo(KitFolder) { UseShellExecute = true });
    }

    private void InstalledKitsToolStripMenuItem_Click(object? sender, EventArgs e)
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

        _filePath = path;
        _dirty = false;
        RememberFile(path);
        var missingKit = _kits.Count > 0 && !_kits.Exists(IsProjectKit);
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

    private void ShortcutsToolStripMenuItem_Click(object? sender, EventArgs e) => Dialogs.ShowShortcuts(this);

    private void AppearanceToolStripMenuItem_DropDownOpening(object? sender, EventArgs e)
    {
        appearanceSystemToolStripMenuItem.Checked = _settings.ColorMode == SystemColorMode.System;
        appearanceLightToolStripMenuItem.Checked = _settings.ColorMode == SystemColorMode.Classic;
        appearanceDarkToolStripMenuItem.Checked = _settings.ColorMode == SystemColorMode.Dark;
    }

    private void AppearanceSystemToolStripMenuItem_Click(object? sender, EventArgs e) => ChooseColorMode(SystemColorMode.System);

    private void AppearanceLightToolStripMenuItem_Click(object? sender, EventArgs e) => ChooseColorMode(SystemColorMode.Classic);

    private void AppearanceDarkToolStripMenuItem_Click(object? sender, EventArgs e) => ChooseColorMode(SystemColorMode.Dark);

    private void ChooseColorMode(SystemColorMode mode)
    {
        if (_settings.ColorMode != mode)
        {
            _settings.ColorMode = mode;
            _settings.Save();
            Dialogs.Info(this, "Die Darstellung wurde geändert.", "Sie gilt ab dem nächsten Start von drums4me.");
        }
    }

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
