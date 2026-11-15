namespace Drums4me.Forms
{
    partial class FrmDrums4me
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmDrums4me));
            ribbonTabs = new TabControl();
            tabPageFile = new TabPage();
            flowFile = new FlowLayoutPanel();
            groupFind = new Panel();
            labelSearch = new Label();
            textBoxSearch = new TextBox();
            labelFilterKit = new Label();
            comboFilterKit = new ComboBox();
            labelFilterMeter = new Label();
            comboFilterMeter = new ComboBox();
            captionFind = new Label();
            dividerFind = new Panel();
            groupStartup = new Panel();
            checkBoxOpenLast = new CheckBox();
            captionStartup = new Label();
            dividerStartup = new Panel();
            tabPageStart = new TabPage();
            flowStart = new FlowLayoutPanel();
            groupPlay = new Panel();
            buttonPlay = new Button();
            captionPlay = new Label();
            dividerPlay = new Panel();
            groupTempo = new Panel();
            labelTempo = new Label();
            numericTempo = new NumericUpDown();
            labelBpm = new Label();
            buttonTap = new Button();
            labelCountIn = new Label();
            numericCountIn = new NumericUpDown();
            labelCountInBars = new Label();
            captionTempo = new Label();
            dividerTempo = new Panel();
            groupTrainer = new Panel();
            checkBoxTrainer = new CheckBox();
            labelTrainerStepBpm = new Label();
            numericTrainerStep = new NumericUpDown();
            labelTrainerBars = new Label();
            labelTrainerEvery = new Label();
            numericTrainerBars = new NumericUpDown();
            labelTrainerTarget = new Label();
            numericTrainerTarget = new NumericUpDown();
            labelTrainerBpm = new Label();
            captionTrainer = new Label();
            dividerTrainer = new Panel();
            groupSwing = new Panel();
            radioSwingStraight = new RadioButton();
            radioSwingEighths = new RadioButton();
            radioSwingSixteenths = new RadioButton();
            captionSwing = new Label();
            dividerSwing = new Panel();
            groupDrumkit = new Panel();
            comboKit = new ComboBox();
            buttonSelectKits = new Button();
            captionDrumkit = new Label();
            dividerDrumkit = new Panel();
            groupPattern = new Panel();
            radioPattern1 = new RadioButton();
            radioPattern2 = new RadioButton();
            radioPattern3 = new RadioButton();
            radioPattern4 = new RadioButton();
            comboMeter = new ComboBox();
            buttonClearPattern = new Button();
            captionPattern = new Label();
            dividerPattern = new Panel();
            groupSequence = new Panel();
            checkBoxSequence = new CheckBox();
            checkBoxRepeat = new CheckBox();
            textBoxSequence = new TextBox();
            captionSequence = new Label();
            dividerSequence = new Panel();
            tabPageSettings = new TabPage();
            flowSettings = new FlowLayoutPanel();
            groupProjects = new Panel();
            labelProjectFolder = new Label();
            textBoxProjectFolder = new TextBox();
            buttonProjectFolder = new Button();
            labelProjectHint = new Label();
            captionProjects = new Label();
            dividerProjects = new Panel();
            groupAppearance = new Panel();
            radioSystem = new RadioButton();
            radioLight = new RadioButton();
            radioDark = new RadioButton();
            checkBoxFlash = new CheckBox();
            captionAppearance = new Label();
            dividerAppearance = new Panel();
            groupLanguage = new Panel();
            radioGerman = new RadioButton();
            radioEnglish = new RadioButton();
            captionLanguage = new Label();
            dividerLanguage = new Panel();
            groupKits = new Panel();
            labelOwnKits = new Label();
            buttonOwnKits = new Button();
            labelInstalledKits = new Label();
            buttonInstalledKits = new Button();
            lineKitFolders = new Panel();
            buttonReloadKits = new Button();
            buttonKitGuide = new Button();
            captionKits = new Label();
            dividerKits = new Panel();
            groupAudio = new Panel();
            labelAudioDevice = new Label();
            comboAudioDevice = new ComboBox();
            captionAudio = new Label();
            dividerAudio = new Panel();
            groupPlayback = new Panel();
            labelClickVolume = new Label();
            buttonKeepScreenOn = new Button();
            captionPlayback = new Label();
            dividerPlayback = new Panel();
            groupFlash = new Panel();
            captionFlash = new Label();
            dividerFlash = new Panel();
            numericClickVolume = new NumericUpDown();
            labelClickVolumeUnit = new Label();
            lineLatency = new Panel();
            labelLatency = new Label();
            numericLatency = new NumericUpDown();
            labelLatencyUnit = new Label();
            buttonLatencyDefault = new Button();
            buttonTopMost = new Button();
            ribbonImages = new ImageList(components);
            panelFileCommands = new Panel();
            buttonNew = new Button();
            buttonSave = new Button();
            buttonSaveAs = new Button();
            buttonExportAudio = new Button();
            buttonCloseProject = new Button();
            lineCommands = new Panel();
            buttonOtherFolder = new Button();
            buttonDefaultFolder = new Button();
            buttonDemos = new Button();
            buttonImportMidi = new Button();
            lineWindowCommands = new Panel();
            buttonResetLayout = new Button();
            buttonFileAbout = new Button();
            buttonExit = new Button();
            lineFileCommands = new Panel();
            tabMarker = new Panel();
            buttonUndo = new Button();
            buttonRedo = new Button();
            buttonQuickSave = new Button();
            buttonShortcuts = new Button();
            buttonAbout = new Button();
            stepGrid = new Drums4me.Controls.StepGrid();
            projectBrowser = new Drums4me.Controls.ProjectBrowser();
            statusStrip = new StatusStrip();
            statusHint = new ToolStripStatusLabel();
            statusProjects = new ToolStripStatusLabel();
            statusTrainer = new ToolStripStatusLabel();
            statusBars = new ToolStripStatusLabel();
            statusTime = new ToolStripStatusLabel();
            playheadTimer = new System.Windows.Forms.Timer(components);
            previewTimer = new System.Windows.Forms.Timer(components);
            noticeTimer = new System.Windows.Forms.Timer(components);
            restartTimer = new System.Windows.Forms.Timer(components);
            toolTip = new ToolTip(components);
            trackNameMenu = new ContextMenuStrip(components);
            previewTrackToolStripMenuItem = new ToolStripMenuItem();
            renameTrackToolStripMenuItem = new ToolStripMenuItem();
            demosMenu = new ContextMenuStrip(components);
            noDemosToolStripMenuItem = new ToolStripMenuItem();
            patternMenu = new ContextMenuStrip(components);
            copyPatternToolStripMenuItem = new ToolStripMenuItem();
            pastePatternToolStripMenuItem = new ToolStripMenuItem();
            clearPatternToolStripMenuItem = new ToolStripMenuItem();
            beatMenu = new ContextMenuStrip(components);
            copyBeatToolStripMenuItem = new ToolStripMenuItem();
            pasteBeatToolStripMenuItem = new ToolStripMenuItem();
            fillBeatsToolStripMenuItem = new ToolStripMenuItem();
            clearBeatToolStripMenuItem = new ToolStripMenuItem();
            pendingOpenTimer = new System.Windows.Forms.Timer(components);
            otherFolderDialog = new FolderBrowserDialog();
            projectFolderDialog = new FolderBrowserDialog();
            saveFileDialog = new SaveFileDialog();
            midiFileDialog = new OpenFileDialog();
            ribbonTabs.SuspendLayout();
            tabPageFile.SuspendLayout();
            flowFile.SuspendLayout();
            groupFind.SuspendLayout();
            groupStartup.SuspendLayout();
            tabPageStart.SuspendLayout();
            flowStart.SuspendLayout();
            groupPlay.SuspendLayout();
            groupTempo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericTempo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericCountIn).BeginInit();
            groupTrainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericTrainerStep).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericTrainerBars).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericTrainerTarget).BeginInit();
            groupSwing.SuspendLayout();
            groupDrumkit.SuspendLayout();
            groupPattern.SuspendLayout();
            groupSequence.SuspendLayout();
            tabPageSettings.SuspendLayout();
            flowSettings.SuspendLayout();
            groupProjects.SuspendLayout();
            groupAppearance.SuspendLayout();
            groupLanguage.SuspendLayout();
            groupKits.SuspendLayout();
            groupAudio.SuspendLayout();
            groupPlayback.SuspendLayout();
            groupFlash.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericClickVolume).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericLatency).BeginInit();
            panelFileCommands.SuspendLayout();
            statusStrip.SuspendLayout();
            trackNameMenu.SuspendLayout();
            demosMenu.SuspendLayout();
            patternMenu.SuspendLayout();
            beatMenu.SuspendLayout();
            SuspendLayout();
            // 
            // ribbonTabs
            // 
            ribbonTabs.Controls.Add(tabPageFile);
            ribbonTabs.Controls.Add(tabPageStart);
            ribbonTabs.Controls.Add(tabPageSettings);
            ribbonTabs.Dock = DockStyle.Top;
            ribbonTabs.ImageList = ribbonImages;
            ribbonTabs.Location = new Point(0, 0);
            ribbonTabs.Name = "ribbonTabs";
            ribbonTabs.SelectedIndex = 1;
            ribbonTabs.ShowToolTips = true;
            ribbonTabs.Size = new Size(1260, 124);
            ribbonTabs.TabIndex = 0;
            ribbonTabs.SelectedIndexChanged += RibbonTabs_SelectedIndexChanged;
            // 
            // tabPageFile
            // 
            tabPageFile.AccessibleName = "Datei";
            tabPageFile.Controls.Add(flowFile);
            tabPageFile.ImageIndex = 0;
            tabPageFile.Location = new Point(4, 26);
            tabPageFile.Name = "tabPageFile";
            tabPageFile.Padding = new Padding(236, 0, 0, 0);
            tabPageFile.Size = new Size(1252, 94);
            tabPageFile.TabIndex = 0;
            tabPageFile.ToolTipText = "Datei (Strg+O oder Alt+D): Projekte, Neu, Speichern und mehr. Esc führt zurück zur Karte Start.";
            // 
            // flowFile
            // 
            flowFile.Controls.Add(groupFind);
            flowFile.Controls.Add(dividerFind);
            flowFile.Controls.Add(groupStartup);
            flowFile.Controls.Add(dividerStartup);
            flowFile.Controls.Add(groupProjects);
            flowFile.Controls.Add(dividerProjects);
            flowFile.Dock = DockStyle.Fill;
            flowFile.Location = new Point(236, 0);
            flowFile.Name = "flowFile";
            flowFile.Size = new Size(1016, 94);
            flowFile.TabIndex = 0;
            // 
            // groupFind
            // 
            groupFind.Controls.Add(labelSearch);
            groupFind.Controls.Add(textBoxSearch);
            groupFind.Controls.Add(labelFilterKit);
            groupFind.Controls.Add(comboFilterKit);
            groupFind.Controls.Add(labelFilterMeter);
            groupFind.Controls.Add(comboFilterMeter);
            groupFind.Controls.Add(captionFind);
            groupFind.Location = new Point(0, 0);
            groupFind.Margin = new Padding(0);
            groupFind.Name = "groupFind";
            groupFind.Size = new Size(480, 90);
            groupFind.TabIndex = 0;
            // 
            // labelSearch
            // 
            labelSearch.AccessibleDescription = "Sucht in den Namen der Projekte und Ordner.";
            labelSearch.AutoSize = true;
            labelSearch.Location = new Point(8, 11);
            labelSearch.Name = "labelSearch";
            labelSearch.Size = new Size(53, 19);
            labelSearch.TabIndex = 0;
            labelSearch.Text = "Suchen";
            // 
            // textBoxSearch
            // 
            textBoxSearch.AccessibleDescription = "Sucht in den Namen der Projekte und Ordner. Strg+F. Enter öffnet das markierte Projekt, Pfeil runter springt in die Liste.";
            textBoxSearch.Location = new Point(62, 5);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.PlaceholderText = "Name oder Ordner (Strg+F)";
            textBoxSearch.Size = new Size(400, 28);
            textBoxSearch.TabIndex = 1;
            textBoxSearch.VisualStylesMode = VisualStylesMode.Net11;
            textBoxSearch.TextChanged += BrowserFilter_Changed;
            textBoxSearch.KeyDown += TextBoxSearch_KeyDown;
            // 
            // labelFilterKit
            // 
            labelFilterKit.AccessibleDescription = "Zeigt nur Projekte mit diesem Kit.";
            labelFilterKit.AutoSize = true;
            labelFilterKit.Location = new Point(8, 42);
            labelFilterKit.Name = "labelFilterKit";
            labelFilterKit.Size = new Size(25, 19);
            labelFilterKit.TabIndex = 2;
            labelFilterKit.Text = "Kit";
            // 
            // comboFilterKit
            // 
            comboFilterKit.AccessibleDescription = "Zeigt nur Projekte mit diesem Kit.";
            comboFilterKit.DropDownStyle = ComboBoxStyle.DropDownList;
            comboFilterKit.Location = new Point(62, 37);
            comboFilterKit.MaxDropDownItems = 20;
            comboFilterKit.Name = "comboFilterKit";
            comboFilterKit.Size = new Size(170, 28);
            comboFilterKit.TabIndex = 3;
            comboFilterKit.VisualStylesMode = VisualStylesMode.Net11;
            comboFilterKit.SelectedIndexChanged += BrowserFilter_Changed;
            // 
            // labelFilterMeter
            // 
            labelFilterMeter.AccessibleDescription = "Zeigt nur Projekte, in denen diese Taktart vorkommt.";
            labelFilterMeter.AutoSize = true;
            labelFilterMeter.Location = new Point(246, 42);
            labelFilterMeter.Name = "labelFilterMeter";
            labelFilterMeter.Size = new Size(50, 19);
            labelFilterMeter.TabIndex = 4;
            labelFilterMeter.Text = "Taktart";
            // 
            // comboFilterMeter
            // 
            comboFilterMeter.AccessibleDescription = "Zeigt nur Projekte, in denen diese Taktart vorkommt.";
            comboFilterMeter.DropDownStyle = ComboBoxStyle.DropDownList;
            comboFilterMeter.Location = new Point(300, 37);
            comboFilterMeter.Name = "comboFilterMeter";
            comboFilterMeter.Size = new Size(162, 28);
            comboFilterMeter.TabIndex = 5;
            comboFilterMeter.VisualStylesMode = VisualStylesMode.Net11;
            comboFilterMeter.SelectedIndexChanged += BrowserFilter_Changed;
            // 
            // captionFind
            // 
            captionFind.Dock = DockStyle.Bottom;
            captionFind.ForeColor = SystemColors.GrayText;
            captionFind.Location = new Point(0, 72);
            captionFind.Name = "captionFind";
            captionFind.Size = new Size(480, 18);
            captionFind.TabIndex = 6;
            captionFind.Text = "Finden";
            captionFind.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerFind
            // 
            dividerFind.BackColor = SystemColors.ControlDark;
            dividerFind.Location = new Point(480, 4);
            dividerFind.Margin = new Padding(0, 4, 0, 4);
            dividerFind.Name = "dividerFind";
            dividerFind.Size = new Size(1, 82);
            dividerFind.TabIndex = 1;
            // 
            // groupStartup
            // 
            groupStartup.Controls.Add(checkBoxOpenLast);
            groupStartup.Controls.Add(captionStartup);
            groupStartup.Location = new Point(481, 0);
            groupStartup.Margin = new Padding(0);
            groupStartup.Name = "groupStartup";
            groupStartup.Size = new Size(150, 90);
            groupStartup.TabIndex = 2;
            // 
            // checkBoxOpenLast
            // 
            checkBoxOpenLast.AccessibleDescription = "Lädt beim Programmstart das zuletzt benutzte Projekt. Sonst beginnst du mit einem neuen.";
            checkBoxOpenLast.Appearance = Appearance.ToggleSwitch;
            checkBoxOpenLast.AutoSize = true;
            checkBoxOpenLast.Location = new Point(10, 14);
            checkBoxOpenLast.Name = "checkBoxOpenLast";
            checkBoxOpenLast.Size = new Size(129, 38);
            checkBoxOpenLast.TabIndex = 0;
            checkBoxOpenLast.Text = "Letzte Datei\r\nwieder öffnen";
            checkBoxOpenLast.UseVisualStyleBackColor = true;
            checkBoxOpenLast.VisualStylesMode = VisualStylesMode.Net11;
            checkBoxOpenLast.CheckedChanged += CheckBoxOpenLast_CheckedChanged;
            // 
            // captionStartup
            // 
            captionStartup.Dock = DockStyle.Bottom;
            captionStartup.ForeColor = SystemColors.GrayText;
            captionStartup.Location = new Point(0, 72);
            captionStartup.Name = "captionStartup";
            captionStartup.Size = new Size(150, 18);
            captionStartup.TabIndex = 1;
            captionStartup.Text = "Programmstart";
            captionStartup.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerStartup
            // 
            dividerStartup.BackColor = SystemColors.ControlDark;
            dividerStartup.Location = new Point(631, 4);
            dividerStartup.Margin = new Padding(0, 4, 0, 4);
            dividerStartup.Name = "dividerStartup";
            dividerStartup.Size = new Size(1, 82);
            dividerStartup.TabIndex = 3;
            // 
            // tabPageStart
            // 
            tabPageStart.Controls.Add(flowStart);
            tabPageStart.Location = new Point(4, 24);
            tabPageStart.Name = "tabPageStart";
            tabPageStart.Size = new Size(1252, 96);
            tabPageStart.TabIndex = 1;
            tabPageStart.Text = "Start";
            // 
            // flowStart
            // 
            flowStart.Controls.Add(groupPlay);
            flowStart.Controls.Add(dividerPlay);
            flowStart.Controls.Add(groupTempo);
            flowStart.Controls.Add(dividerTempo);
            flowStart.Controls.Add(groupTrainer);
            flowStart.Controls.Add(dividerTrainer);
            flowStart.Controls.Add(groupSwing);
            flowStart.Controls.Add(dividerSwing);
            flowStart.Controls.Add(groupFlash);
            flowStart.Controls.Add(dividerFlash);
            flowStart.Controls.Add(groupDrumkit);
            flowStart.Controls.Add(dividerDrumkit);
            flowStart.Controls.Add(groupPattern);
            flowStart.Controls.Add(dividerPattern);
            flowStart.Controls.Add(groupSequence);
            flowStart.Controls.Add(dividerSequence);
            flowStart.Dock = DockStyle.Fill;
            flowStart.Location = new Point(0, 0);
            flowStart.Name = "flowStart";
            flowStart.Size = new Size(1252, 96);
            flowStart.TabIndex = 0;
            // 
            // groupPlay
            // 
            groupPlay.Controls.Add(buttonPlay);
            groupPlay.Controls.Add(captionPlay);
            groupPlay.Location = new Point(0, 0);
            groupPlay.Margin = new Padding(0);
            groupPlay.Name = "groupPlay";
            groupPlay.Size = new Size(88, 90);
            groupPlay.TabIndex = 0;
            // 
            // buttonPlay
            // 
            buttonPlay.AccessibleDescription = "Startet oder stoppt die Wiedergabe. Leertaste.";
            buttonPlay.AccessibleName = "Start";
            buttonPlay.Font = new Font("Segoe UI Symbol", 22F);
            buttonPlay.Location = new Point(12, 3);
            buttonPlay.Name = "buttonPlay";
            buttonPlay.Size = new Size(64, 64);
            buttonPlay.TabIndex = 0;
            buttonPlay.Text = "▶";
            toolTip.SetToolTip(buttonPlay, "Start (Leertaste)");
            buttonPlay.UseVisualStyleBackColor = true;
            buttonPlay.Click += ButtonPlay_Click;
            // 
            // captionPlay
            // 
            captionPlay.Dock = DockStyle.Bottom;
            captionPlay.ForeColor = SystemColors.GrayText;
            captionPlay.Location = new Point(0, 72);
            captionPlay.Name = "captionPlay";
            captionPlay.Size = new Size(88, 18);
            captionPlay.TabIndex = 1;
            captionPlay.Text = "Wiedergabe";
            captionPlay.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerPlay
            // 
            dividerPlay.BackColor = SystemColors.ControlDark;
            dividerPlay.Location = new Point(88, 4);
            dividerPlay.Margin = new Padding(0, 4, 0, 4);
            dividerPlay.Name = "dividerPlay";
            dividerPlay.Size = new Size(1, 82);
            dividerPlay.TabIndex = 1;
            // 
            // groupTempo
            // 
            groupTempo.Controls.Add(labelTempo);
            groupTempo.Controls.Add(numericTempo);
            groupTempo.Controls.Add(labelBpm);
            groupTempo.Controls.Add(buttonTap);
            groupTempo.Controls.Add(labelCountIn);
            groupTempo.Controls.Add(numericCountIn);
            groupTempo.Controls.Add(labelCountInBars);
            groupTempo.Controls.Add(captionTempo);
            groupTempo.Location = new Point(89, 0);
            groupTempo.Margin = new Padding(0);
            groupTempo.Name = "groupTempo";
            groupTempo.Size = new Size(222, 90);
            groupTempo.TabIndex = 2;
            // 
            // labelTempo
            // 
            labelTempo.AccessibleDescription = "Tempo in Viertelnoten pro Minute. Plus und Minus: 1 BPM. Strg+Plus und Strg+Minus: 5 BPM.";
            labelTempo.AutoSize = true;
            labelTempo.Location = new Point(8, 8);
            labelTempo.Name = "labelTempo";
            labelTempo.Size = new Size(50, 19);
            labelTempo.TabIndex = 0;
            labelTempo.Text = "Tempo";
            // 
            // numericTempo
            // 
            numericTempo.AccessibleDescription = "Tempo in Viertelnoten pro Minute. Plus und Minus: 1 BPM. Strg+Plus und Strg+Minus: 5 BPM.";
            numericTempo.Location = new Point(66, 5);
            numericTempo.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            numericTempo.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            numericTempo.Name = "numericTempo";
            numericTempo.Size = new Size(58, 25);
            numericTempo.TabIndex = 1;
            numericTempo.TextAlign = HorizontalAlignment.Center;
            numericTempo.Value = new decimal(new int[] { 80, 0, 0, 0 });
            numericTempo.ValueChanged += NumericTempo_ValueChanged;
            // 
            // labelBpm
            // 
            labelBpm.AccessibleDescription = "Tempo in Viertelnoten pro Minute. Plus und Minus: 1 BPM. Strg+Plus und Strg+Minus: 5 BPM.";
            labelBpm.AutoSize = true;
            labelBpm.Location = new Point(126, 8);
            labelBpm.Name = "labelBpm";
            labelBpm.Size = new Size(38, 19);
            labelBpm.TabIndex = 2;
            labelBpm.Text = "BPM";
            // 
            // buttonTap
            // 
            buttonTap.AccessibleDescription = "Öffnet ein Fenster, in dem du das Tempo mit der Leertaste tippst oder aus dem laufenden Song ermitteln lässt. Strg+T.";
            buttonTap.AccessibleName = "Tempo ermitteln";
            buttonTap.Location = new Point(168, 3);
            buttonTap.Name = "buttonTap";
            buttonTap.Size = new Size(48, 64);
            buttonTap.TabIndex = 3;
            toolTip.SetToolTip(buttonTap, "Tempo ermitteln (Strg+T)");
            buttonTap.UseVisualStyleBackColor = true;
            buttonTap.Click += ButtonTap_Click;
            buttonTap.Paint += ButtonTap_Paint;
            // 
            // labelCountIn
            // 
            labelCountIn.AccessibleDescription = "So viele Takte zählt ein Metronom vor, bevor die Patterns einsetzen. 0: kein Vorzählen.";
            labelCountIn.Location = new Point(8, 40);
            labelCountIn.Name = "labelCountIn";
            labelCountIn.Size = new Size(72, 19);
            labelCountIn.TabIndex = 4;
            labelCountIn.Text = "Vorzählen";
            // 
            // numericCountIn
            // 
            numericCountIn.AccessibleDescription = "So viele Takte zählt ein Metronom vor, bevor die Patterns einsetzen. 0: kein Vorzählen.";
            numericCountIn.Location = new Point(80, 37);
            numericCountIn.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
            numericCountIn.Name = "numericCountIn";
            numericCountIn.Size = new Size(44, 25);
            numericCountIn.TabIndex = 5;
            numericCountIn.TextAlign = HorizontalAlignment.Center;
            numericCountIn.ValueChanged += NumericCountIn_ValueChanged;
            // 
            // labelCountInBars
            // 
            labelCountInBars.AccessibleDescription = "So viele Takte zählt ein Metronom vor, bevor die Patterns einsetzen. 0: kein Vorzählen.";
            labelCountInBars.AutoSize = true;
            labelCountInBars.Location = new Point(126, 40);
            labelCountInBars.Name = "labelCountInBars";
            labelCountInBars.Size = new Size(40, 19);
            labelCountInBars.TabIndex = 6;
            labelCountInBars.Text = "Takte";
            // 
            // captionTempo
            // 
            captionTempo.Dock = DockStyle.Bottom;
            captionTempo.ForeColor = SystemColors.GrayText;
            captionTempo.Location = new Point(0, 72);
            captionTempo.Name = "captionTempo";
            captionTempo.Size = new Size(222, 18);
            captionTempo.TabIndex = 7;
            captionTempo.Text = "Tempo";
            captionTempo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerTempo
            // 
            dividerTempo.BackColor = SystemColors.ControlDark;
            dividerTempo.Location = new Point(311, 4);
            dividerTempo.Margin = new Padding(0, 4, 0, 4);
            dividerTempo.Name = "dividerTempo";
            dividerTempo.Size = new Size(1, 82);
            dividerTempo.TabIndex = 3;
            // 
            // groupTrainer
            // 
            groupTrainer.Controls.Add(checkBoxTrainer);
            groupTrainer.Controls.Add(labelTrainerStepBpm);
            groupTrainer.Controls.Add(numericTrainerStep);
            groupTrainer.Controls.Add(labelTrainerBars);
            groupTrainer.Controls.Add(labelTrainerEvery);
            groupTrainer.Controls.Add(numericTrainerBars);
            groupTrainer.Controls.Add(labelTrainerTarget);
            groupTrainer.Controls.Add(numericTrainerTarget);
            groupTrainer.Controls.Add(labelTrainerBpm);
            groupTrainer.Controls.Add(captionTrainer);
            groupTrainer.Location = new Point(312, 0);
            groupTrainer.Margin = new Padding(0);
            groupTrainer.Name = "groupTrainer";
            groupTrainer.Size = new Size(230, 90);
            groupTrainer.TabIndex = 4;
            // 
            // checkBoxTrainer
            // 
            checkBoxTrainer.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            checkBoxTrainer.AccessibleName = "Tempotrainer";
            checkBoxTrainer.Appearance = Appearance.ToggleSwitch;
            checkBoxTrainer.Location = new Point(8, 8);
            checkBoxTrainer.Name = "checkBoxTrainer";
            checkBoxTrainer.Size = new Size(84, 19);
            checkBoxTrainer.TabIndex = 0;
            checkBoxTrainer.Text = "Aktiv";
            checkBoxTrainer.UseVisualStyleBackColor = true;
            checkBoxTrainer.VisualStylesMode = VisualStylesMode.Net11;
            checkBoxTrainer.CheckedChanged += CheckBoxTrainer_CheckedChanged;
            // 
            // labelTrainerStepBpm
            // 
            labelTrainerStepBpm.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            labelTrainerStepBpm.AutoSize = true;
            labelTrainerStepBpm.Location = new Point(53, 40);
            labelTrainerStepBpm.Name = "labelTrainerStepBpm";
            labelTrainerStepBpm.Size = new Size(38, 19);
            labelTrainerStepBpm.TabIndex = 5;
            labelTrainerStepBpm.Text = "BPM";
            // 
            // numericTrainerStep
            // 
            numericTrainerStep.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            numericTrainerStep.Enabled = false;
            numericTrainerStep.Location = new Point(8, 37);
            numericTrainerStep.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            numericTrainerStep.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericTrainerStep.Name = "numericTrainerStep";
            numericTrainerStep.Size = new Size(44, 25);
            numericTrainerStep.TabIndex = 4;
            numericTrainerStep.TextAlign = HorizontalAlignment.Center;
            numericTrainerStep.Value = new decimal(new int[] { 2, 0, 0, 0 });
            numericTrainerStep.ValueChanged += NumericTrainer_ValueChanged;
            // 
            // labelTrainerBars
            // 
            labelTrainerBars.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            labelTrainerBars.AutoSize = true;
            labelTrainerBars.Location = new Point(191, 40);
            labelTrainerBars.Name = "labelTrainerBars";
            labelTrainerBars.Size = new Size(40, 19);
            labelTrainerBars.TabIndex = 8;
            labelTrainerBars.Text = "Takte";
            // 
            // labelTrainerEvery
            // 
            labelTrainerEvery.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            labelTrainerEvery.Location = new Point(92, 40);
            labelTrainerEvery.Name = "labelTrainerEvery";
            labelTrainerEvery.Size = new Size(42, 19);
            labelTrainerEvery.TabIndex = 6;
            labelTrainerEvery.Text = "alle";
            labelTrainerEvery.TextAlign = ContentAlignment.MiddleRight;
            // 
            // numericTrainerBars
            // 
            numericTrainerBars.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            numericTrainerBars.Enabled = false;
            numericTrainerBars.Location = new Point(134, 37);
            numericTrainerBars.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            numericTrainerBars.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericTrainerBars.Name = "numericTrainerBars";
            numericTrainerBars.Size = new Size(56, 25);
            numericTrainerBars.TabIndex = 7;
            numericTrainerBars.TextAlign = HorizontalAlignment.Center;
            numericTrainerBars.Value = new decimal(new int[] { 4, 0, 0, 0 });
            numericTrainerBars.ValueChanged += NumericTrainer_ValueChanged;
            // 
            // labelTrainerTarget
            // 
            labelTrainerTarget.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            labelTrainerTarget.Location = new Point(92, 8);
            labelTrainerTarget.Name = "labelTrainerTarget";
            labelTrainerTarget.Size = new Size(42, 19);
            labelTrainerTarget.TabIndex = 1;
            labelTrainerTarget.Text = "bis";
            labelTrainerTarget.TextAlign = ContentAlignment.MiddleRight;
            // 
            // numericTrainerTarget
            // 
            numericTrainerTarget.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            numericTrainerTarget.Enabled = false;
            numericTrainerTarget.Location = new Point(134, 5);
            numericTrainerTarget.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            numericTrainerTarget.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            numericTrainerTarget.Name = "numericTrainerTarget";
            numericTrainerTarget.Size = new Size(56, 25);
            numericTrainerTarget.TabIndex = 2;
            numericTrainerTarget.TextAlign = HorizontalAlignment.Center;
            numericTrainerTarget.Value = new decimal(new int[] { 120, 0, 0, 0 });
            numericTrainerTarget.ValueChanged += NumericTrainer_ValueChanged;
            // 
            // labelTrainerBpm
            // 
            labelTrainerBpm.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            labelTrainerBpm.AutoSize = true;
            labelTrainerBpm.Location = new Point(191, 8);
            labelTrainerBpm.Name = "labelTrainerBpm";
            labelTrainerBpm.Size = new Size(38, 19);
            labelTrainerBpm.TabIndex = 3;
            labelTrainerBpm.Text = "BPM";
            // 
            // captionTrainer
            // 
            captionTrainer.Dock = DockStyle.Bottom;
            captionTrainer.ForeColor = SystemColors.GrayText;
            captionTrainer.Location = new Point(0, 72);
            captionTrainer.Name = "captionTrainer";
            captionTrainer.Size = new Size(230, 18);
            captionTrainer.TabIndex = 9;
            captionTrainer.Text = "Tempotrainer";
            captionTrainer.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerTrainer
            // 
            dividerTrainer.BackColor = SystemColors.ControlDark;
            dividerTrainer.Location = new Point(542, 4);
            dividerTrainer.Margin = new Padding(0, 4, 0, 4);
            dividerTrainer.Name = "dividerTrainer";
            dividerTrainer.Size = new Size(1, 82);
            dividerTrainer.TabIndex = 5;
            // 
            // groupSwing
            //
            groupSwing.Controls.Add(radioSwingStraight);
            groupSwing.Controls.Add(radioSwingEighths);
            groupSwing.Controls.Add(radioSwingSixteenths);
            groupSwing.Controls.Add(captionSwing);
            groupSwing.Location = new Point(543, 0);
            groupSwing.Margin = new Padding(0);
            groupSwing.Name = "groupSwing";
            groupSwing.Size = new Size(104, 90);
            groupSwing.TabIndex = 6;
            //
            // radioSwingStraight
            //
            radioSwingStraight.AccessibleDescription = "Gerader Rhythmus: Alle Schritte sind gleich lang, so wie sie im Raster stehen.";
            radioSwingStraight.AutoSize = true;
            radioSwingStraight.Checked = true;
            radioSwingStraight.Location = new Point(6, 1);
            radioSwingStraight.Name = "radioSwingStraight";
            radioSwingStraight.Size = new Size(68, 23);
            radioSwingStraight.TabIndex = 0;
            radioSwingStraight.TabStop = true;
            radioSwingStraight.Text = "Gerade";
            radioSwingStraight.UseVisualStyleBackColor = true;
            radioSwingStraight.CheckedChanged += RadioSwing_CheckedChanged;
            //
            // radioSwingEighths
            //
            radioSwingEighths.AccessibleDescription = "Swing-Achtel: Die zweite Achtel jeder Zählzeit kommt später, wie die dritte Note einer Triole. Wirkt in 2/4, 3/4, 4/4 und 5/4.";
            radioSwingEighths.AutoSize = true;
            radioSwingEighths.Location = new Point(6, 24);
            radioSwingEighths.Name = "radioSwingEighths";
            radioSwingEighths.Size = new Size(65, 23);
            radioSwingEighths.TabIndex = 1;
            radioSwingEighths.Text = "Achtel";
            radioSwingEighths.UseVisualStyleBackColor = true;
            radioSwingEighths.CheckedChanged += RadioSwing_CheckedChanged;
            //
            // radioSwingSixteenths
            //
            radioSwingSixteenths.AccessibleDescription = "Swing-Sechzehntel: Jede zweite Sechzehntel kommt später, wie die dritte Note einer Triole.";
            radioSwingSixteenths.AutoSize = true;
            radioSwingSixteenths.Location = new Point(6, 47);
            radioSwingSixteenths.Name = "radioSwingSixteenths";
            radioSwingSixteenths.Size = new Size(94, 23);
            radioSwingSixteenths.TabIndex = 2;
            radioSwingSixteenths.Text = "Sechzehntel";
            radioSwingSixteenths.UseVisualStyleBackColor = true;
            radioSwingSixteenths.CheckedChanged += RadioSwing_CheckedChanged;
            //
            // captionSwing
            //
            captionSwing.Dock = DockStyle.Bottom;
            captionSwing.ForeColor = SystemColors.GrayText;
            captionSwing.Location = new Point(0, 72);
            captionSwing.Name = "captionSwing";
            captionSwing.Size = new Size(104, 18);
            captionSwing.TabIndex = 3;
            captionSwing.Text = "Swing";
            captionSwing.TextAlign = ContentAlignment.MiddleCenter;
            //
            // dividerSwing
            //
            dividerSwing.BackColor = SystemColors.ControlDark;
            dividerSwing.Location = new Point(647, 4);
            dividerSwing.Margin = new Padding(0, 4, 0, 4);
            dividerSwing.Name = "dividerSwing";
            dividerSwing.Size = new Size(1, 82);
            dividerSwing.TabIndex = 7;
            //
            // groupFlash
            // 
            groupFlash.Controls.Add(checkBoxFlash);
            groupFlash.Controls.Add(captionFlash);
            groupFlash.Location = new Point(648, 0);
            groupFlash.Margin = new Padding(0);
            groupFlash.Name = "groupFlash";
            groupFlash.Size = new Size(76, 90);
            groupFlash.TabIndex = 8;
            // 
            // checkBoxFlash
            // 
            checkBoxFlash.AccessibleDescription = "Lässt das Raster bei jedem Taktanfang kurz aufleuchten.";
            checkBoxFlash.AccessibleName = "Taktblitz";
            checkBoxFlash.Appearance = Appearance.ToggleSwitch;
            checkBoxFlash.AutoSize = true;
            checkBoxFlash.Location = new Point(20, 26);
            checkBoxFlash.Name = "checkBoxFlash";
            checkBoxFlash.Size = new Size(37, 16);
            checkBoxFlash.TabIndex = 0;
            checkBoxFlash.UseVisualStyleBackColor = true;
            checkBoxFlash.VisualStylesMode = VisualStylesMode.Net11;
            checkBoxFlash.CheckedChanged += CheckBoxFlash_CheckedChanged;
            // 
            // captionFlash
            // 
            captionFlash.AccessibleDescription = "Lässt das Raster bei jedem Taktanfang kurz aufleuchten.";
            captionFlash.Dock = DockStyle.Bottom;
            captionFlash.ForeColor = SystemColors.GrayText;
            captionFlash.Location = new Point(0, 72);
            captionFlash.Name = "captionFlash";
            captionFlash.Size = new Size(76, 18);
            captionFlash.TabIndex = 1;
            captionFlash.Text = "Taktblitz";
            captionFlash.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerFlash
            // 
            dividerFlash.BackColor = SystemColors.ControlDark;
            dividerFlash.Location = new Point(724, 4);
            dividerFlash.Margin = new Padding(0, 4, 0, 4);
            dividerFlash.Name = "dividerFlash";
            dividerFlash.Size = new Size(1, 82);
            dividerFlash.TabIndex = 9;
            // 
            // groupDrumkit
            // 
            groupDrumkit.Controls.Add(comboKit);
            groupDrumkit.Controls.Add(buttonSelectKits);
            groupDrumkit.Controls.Add(captionDrumkit);
            groupDrumkit.Location = new Point(723, 0);
            groupDrumkit.Margin = new Padding(0);
            groupDrumkit.Name = "groupDrumkit";
            groupDrumkit.Size = new Size(140, 90);
            groupDrumkit.TabIndex = 10;
            // 
            // comboKit
            // 
            comboKit.AccessibleDescription = "Die Klänge dieses Projekts. In der Liste steht der Ordner des Kits in Klammern.";
            comboKit.AccessibleName = "Drumkit";
            comboKit.DrawMode = DrawMode.OwnerDrawFixed;
            comboKit.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKit.DropDownWidth = 200;
            comboKit.ItemHeight = 19;
            comboKit.Location = new Point(8, 5);
            comboKit.MaxDropDownItems = 30;
            comboKit.Name = "comboKit";
            comboKit.Size = new Size(124, 25);
            comboKit.TabIndex = 0;
            comboKit.VisualStylesMode = VisualStylesMode.Net11;
            comboKit.DrawItem += ComboKit_DrawItem;
            comboKit.SelectedIndexChanged += ComboKit_SelectedIndexChanged;
            // 
            // buttonSelectKits
            // 
            buttonSelectKits.AccessibleDescription = "Legt fest, welche Kits in der Auswahl darüber erscheinen.";
            buttonSelectKits.Location = new Point(8, 35);
            buttonSelectKits.Name = "buttonSelectKits";
            buttonSelectKits.Size = new Size(124, 29);
            buttonSelectKits.TabIndex = 1;
            buttonSelectKits.Text = "Kits verwalten…";
            buttonSelectKits.UseVisualStyleBackColor = true;
            buttonSelectKits.Click += ButtonSelectKits_Click;
            // 
            // captionDrumkit
            // 
            captionDrumkit.Dock = DockStyle.Bottom;
            captionDrumkit.ForeColor = SystemColors.GrayText;
            captionDrumkit.Location = new Point(0, 72);
            captionDrumkit.Name = "captionDrumkit";
            captionDrumkit.Size = new Size(140, 18);
            captionDrumkit.TabIndex = 2;
            captionDrumkit.Text = "Drumkit";
            captionDrumkit.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerDrumkit
            // 
            dividerDrumkit.BackColor = SystemColors.ControlDark;
            dividerDrumkit.Location = new Point(863, 4);
            dividerDrumkit.Margin = new Padding(0, 4, 0, 4);
            dividerDrumkit.Name = "dividerDrumkit";
            dividerDrumkit.Size = new Size(1, 82);
            dividerDrumkit.TabIndex = 11;
            // 
            // groupPattern
            // 
            groupPattern.Controls.Add(radioPattern1);
            groupPattern.Controls.Add(radioPattern2);
            groupPattern.Controls.Add(radioPattern3);
            groupPattern.Controls.Add(radioPattern4);
            groupPattern.Controls.Add(comboMeter);
            groupPattern.Controls.Add(buttonClearPattern);
            groupPattern.Controls.Add(captionPattern);
            groupPattern.Location = new Point(864, 0);
            groupPattern.Margin = new Padding(0);
            groupPattern.Name = "groupPattern";
            groupPattern.Size = new Size(160, 90);
            groupPattern.TabIndex = 12;
            // 
            // radioPattern1
            // 
            radioPattern1.AccessibleDescription = "Zeigt und spielt dieses Pattern; beim ersten Aufruf übernimmt es den Inhalt des aktuellen. Ziehen auf ein anderes Pattern: dorthin kopieren. Rechtsklick: kopieren, einfügen, leeren.";
            radioPattern1.Appearance = Appearance.Button;
            radioPattern1.ContextMenuStrip = patternMenu;
            radioPattern1.Checked = true;
            radioPattern1.Location = new Point(8, 3);
            radioPattern1.Name = "radioPattern1";
            radioPattern1.Size = new Size(36, 29);
            radioPattern1.TabIndex = 0;
            radioPattern1.TabStop = true;
            radioPattern1.Text = "1";
            radioPattern1.TextAlign = ContentAlignment.MiddleCenter;
            radioPattern1.UseVisualStyleBackColor = true;
            radioPattern1.CheckedChanged += RadioPattern_CheckedChanged;
            radioPattern1.MouseDown += RadioPattern_MouseDown;
            radioPattern1.MouseMove += RadioPattern_MouseMove;
            radioPattern1.MouseUp += RadioPattern_MouseUp;
            // 
            // radioPattern2
            // 
            radioPattern2.AccessibleDescription = "Zeigt und spielt dieses Pattern; beim ersten Aufruf übernimmt es den Inhalt des aktuellen. Ziehen auf ein anderes Pattern: dorthin kopieren. Rechtsklick: kopieren, einfügen, leeren.";
            radioPattern2.Appearance = Appearance.Button;
            radioPattern2.ContextMenuStrip = patternMenu;
            radioPattern2.Location = new Point(44, 3);
            radioPattern2.Name = "radioPattern2";
            radioPattern2.Size = new Size(36, 29);
            radioPattern2.TabIndex = 1;
            radioPattern2.Text = "2";
            radioPattern2.TextAlign = ContentAlignment.MiddleCenter;
            radioPattern2.UseVisualStyleBackColor = true;
            radioPattern2.CheckedChanged += RadioPattern_CheckedChanged;
            radioPattern2.MouseDown += RadioPattern_MouseDown;
            radioPattern2.MouseMove += RadioPattern_MouseMove;
            radioPattern2.MouseUp += RadioPattern_MouseUp;
            // 
            // radioPattern3
            // 
            radioPattern3.AccessibleDescription = "Zeigt und spielt dieses Pattern; beim ersten Aufruf übernimmt es den Inhalt des aktuellen. Ziehen auf ein anderes Pattern: dorthin kopieren. Rechtsklick: kopieren, einfügen, leeren.";
            radioPattern3.Appearance = Appearance.Button;
            radioPattern3.ContextMenuStrip = patternMenu;
            radioPattern3.Location = new Point(80, 3);
            radioPattern3.Name = "radioPattern3";
            radioPattern3.Size = new Size(36, 29);
            radioPattern3.TabIndex = 2;
            radioPattern3.Text = "3";
            radioPattern3.TextAlign = ContentAlignment.MiddleCenter;
            radioPattern3.UseVisualStyleBackColor = true;
            radioPattern3.CheckedChanged += RadioPattern_CheckedChanged;
            radioPattern3.MouseDown += RadioPattern_MouseDown;
            radioPattern3.MouseMove += RadioPattern_MouseMove;
            radioPattern3.MouseUp += RadioPattern_MouseUp;
            // 
            // radioPattern4
            // 
            radioPattern4.AccessibleDescription = "Zeigt und spielt dieses Pattern; beim ersten Aufruf übernimmt es den Inhalt des aktuellen. Ziehen auf ein anderes Pattern: dorthin kopieren. Rechtsklick: kopieren, einfügen, leeren.";
            radioPattern4.Appearance = Appearance.Button;
            radioPattern4.ContextMenuStrip = patternMenu;
            radioPattern4.Location = new Point(116, 3);
            radioPattern4.Name = "radioPattern4";
            radioPattern4.Size = new Size(36, 29);
            radioPattern4.TabIndex = 3;
            radioPattern4.Text = "4";
            radioPattern4.TextAlign = ContentAlignment.MiddleCenter;
            radioPattern4.UseVisualStyleBackColor = true;
            radioPattern4.CheckedChanged += RadioPattern_CheckedChanged;
            radioPattern4.MouseDown += RadioPattern_MouseDown;
            radioPattern4.MouseMove += RadioPattern_MouseMove;
            radioPattern4.MouseUp += RadioPattern_MouseUp;
            // 
            // comboMeter
            // 
            comboMeter.AccessibleDescription = "Die Taktart des aktuellen Patterns.";
            comboMeter.AccessibleName = "Taktart";
            comboMeter.DropDownStyle = ComboBoxStyle.DropDownList;
            comboMeter.Location = new Point(8, 37);
            comboMeter.Name = "comboMeter";
            comboMeter.Size = new Size(104, 28);
            comboMeter.TabIndex = 4;
            comboMeter.VisualStylesMode = VisualStylesMode.Net11;
            comboMeter.SelectedIndexChanged += ComboMeter_SelectedIndexChanged;
            // 
            // buttonClearPattern
            // 
            buttonClearPattern.AccessibleDescription = "Leert das aktuelle Pattern. Strg+Entf. Strg+Z nimmt das zurück.";
            buttonClearPattern.AccessibleName = "Pattern leeren";
            buttonClearPattern.Image = (Image)resources.GetObject("buttonClearPattern.Image");
            buttonClearPattern.Location = new Point(116, 35);
            buttonClearPattern.Name = "buttonClearPattern";
            buttonClearPattern.Size = new Size(36, 29);
            buttonClearPattern.TabIndex = 5;
            toolTip.SetToolTip(buttonClearPattern, "Pattern leeren (Strg+Entf)");
            buttonClearPattern.UseVisualStyleBackColor = true;
            buttonClearPattern.Click += ButtonClearPattern_Click;
            // 
            // captionPattern
            // 
            captionPattern.Dock = DockStyle.Bottom;
            captionPattern.ForeColor = SystemColors.GrayText;
            captionPattern.Location = new Point(0, 72);
            captionPattern.Name = "captionPattern";
            captionPattern.Size = new Size(160, 18);
            captionPattern.TabIndex = 6;
            captionPattern.Text = "Pattern";
            captionPattern.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerPattern
            // 
            dividerPattern.BackColor = SystemColors.ControlDark;
            dividerPattern.Location = new Point(1024, 4);
            dividerPattern.Margin = new Padding(0, 4, 0, 4);
            dividerPattern.Name = "dividerPattern";
            dividerPattern.Size = new Size(1, 82);
            dividerPattern.TabIndex = 13;
            // 
            // groupSequence
            // 
            groupSequence.Controls.Add(checkBoxSequence);
            groupSequence.Controls.Add(checkBoxRepeat);
            groupSequence.Controls.Add(textBoxSequence);
            groupSequence.Controls.Add(captionSequence);
            groupSequence.Location = new Point(1025, 0);
            groupSequence.Margin = new Padding(0);
            groupSequence.Name = "groupSequence";
            groupSequence.Size = new Size(222, 90);
            groupSequence.TabIndex = 14;
            // 
            // checkBoxSequence
            // 
            checkBoxSequence.AccessibleDescription = "Spielt die Patterns in fester Reihenfolge: eine Ziffer je Takt, zum Beispiel 1 1 1 2. Kurz: 1x8 für acht Takte. 0 ist ein stummer Takt.";
            checkBoxSequence.AccessibleName = "Abfolge";
            checkBoxSequence.Appearance = Appearance.ToggleSwitch;
            checkBoxSequence.Location = new Point(8, 8);
            checkBoxSequence.Name = "checkBoxSequence";
            checkBoxSequence.Size = new Size(84, 19);
            checkBoxSequence.TabIndex = 0;
            checkBoxSequence.Text = "Aktiv";
            checkBoxSequence.UseVisualStyleBackColor = true;
            checkBoxSequence.VisualStylesMode = VisualStylesMode.Net11;
            checkBoxSequence.CheckedChanged += CheckBoxSequence_CheckedChanged;
            // 
            // checkBoxRepeat
            // 
            checkBoxRepeat.AccessibleDescription = "Beginnt die Abfolge am Ende von vorn. Sonst stoppt die Wiedergabe nach dem letzten Takt.";
            checkBoxRepeat.Appearance = Appearance.ToggleSwitch;
            checkBoxRepeat.Checked = true;
            checkBoxRepeat.CheckState = CheckState.Checked;
            checkBoxRepeat.Enabled = false;
            checkBoxRepeat.Location = new Point(92, 8);
            checkBoxRepeat.Name = "checkBoxRepeat";
            checkBoxRepeat.Size = new Size(126, 19);
            checkBoxRepeat.TabIndex = 1;
            checkBoxRepeat.Text = "Wiederholen";
            checkBoxRepeat.UseVisualStyleBackColor = true;
            checkBoxRepeat.VisualStylesMode = VisualStylesMode.Net11;
            checkBoxRepeat.CheckedChanged += CheckBoxRepeat_CheckedChanged;
            // 
            // textBoxSequence
            // 
            textBoxSequence.AccessibleDescription = "Spielt die Patterns in fester Reihenfolge: eine Ziffer je Takt, zum Beispiel 1 1 1 2. Kurz: 1x8 für acht Takte. 0 ist ein stummer Takt.";
            textBoxSequence.Enabled = false;
            textBoxSequence.Location = new Point(8, 37);
            textBoxSequence.Name = "textBoxSequence";
            textBoxSequence.PlaceholderText = "Patternreihenfolge";
            textBoxSequence.Size = new Size(208, 28);
            textBoxSequence.TabIndex = 2;
            textBoxSequence.VisualStylesMode = VisualStylesMode.Net11;
            textBoxSequence.TextChanged += TextBoxSequence_TextChanged;
            textBoxSequence.Leave += TextBoxSequence_Leave;
            // 
            // captionSequence
            // 
            captionSequence.Dock = DockStyle.Bottom;
            captionSequence.ForeColor = SystemColors.GrayText;
            captionSequence.Location = new Point(0, 72);
            captionSequence.Name = "captionSequence";
            captionSequence.Size = new Size(222, 18);
            captionSequence.TabIndex = 3;
            captionSequence.Text = "Abfolge";
            captionSequence.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerSequence
            // 
            dividerSequence.BackColor = SystemColors.ControlDark;
            dividerSequence.Location = new Point(1247, 4);
            dividerSequence.Margin = new Padding(0, 4, 0, 4);
            dividerSequence.Name = "dividerSequence";
            dividerSequence.Size = new Size(1, 82);
            dividerSequence.TabIndex = 15;
            // 
            // tabPageSettings
            // 
            tabPageSettings.Controls.Add(flowSettings);
            tabPageSettings.Location = new Point(4, 24);
            tabPageSettings.Name = "tabPageSettings";
            tabPageSettings.Size = new Size(1252, 96);
            tabPageSettings.TabIndex = 2;
            tabPageSettings.Text = "Einstellungen";
            // 
            // flowSettings
            // 
            flowSettings.Controls.Add(groupAppearance);
            flowSettings.Controls.Add(dividerAppearance);
            flowSettings.Controls.Add(groupLanguage);
            flowSettings.Controls.Add(dividerLanguage);
            flowSettings.Controls.Add(groupKits);
            flowSettings.Controls.Add(dividerKits);
            flowSettings.Controls.Add(groupAudio);
            flowSettings.Controls.Add(dividerAudio);
            flowSettings.Controls.Add(groupPlayback);
            flowSettings.Controls.Add(dividerPlayback);
            flowSettings.Dock = DockStyle.Fill;
            flowSettings.Location = new Point(0, 0);
            flowSettings.Name = "flowSettings";
            flowSettings.Size = new Size(1252, 96);
            flowSettings.TabIndex = 0;
            // 
            // groupProjects
            // 
            groupProjects.Controls.Add(labelProjectFolder);
            groupProjects.Controls.Add(textBoxProjectFolder);
            groupProjects.Controls.Add(buttonProjectFolder);
            groupProjects.Controls.Add(labelProjectHint);
            groupProjects.Controls.Add(captionProjects);
            groupProjects.Location = new Point(632, 0);
            groupProjects.Margin = new Padding(0);
            groupProjects.Name = "groupProjects";
            groupProjects.Size = new Size(376, 90);
            groupProjects.TabIndex = 4;
            // 
            // labelProjectFolder
            // 
            labelProjectFolder.AutoSize = true;
            labelProjectFolder.Location = new Point(8, 8);
            labelProjectFolder.Name = "labelProjectFolder";
            labelProjectFolder.Size = new Size(78, 19);
            labelProjectFolder.TabIndex = 0;
            labelProjectFolder.Text = "Speicherort";
            // 
            // textBoxProjectFolder
            // 
            textBoxProjectFolder.Location = new Point(92, 5);
            textBoxProjectFolder.Name = "textBoxProjectFolder";
            textBoxProjectFolder.ReadOnly = true;
            textBoxProjectFolder.Size = new Size(240, 28);
            textBoxProjectFolder.TabIndex = 1;
            textBoxProjectFolder.TabStop = false;
            textBoxProjectFolder.VisualStylesMode = VisualStylesMode.Net11;
            // 
            // buttonProjectFolder
            // 
            buttonProjectFolder.AccessibleDescription = "Wählt den Ordner für deine Projekte.";
            buttonProjectFolder.AccessibleName = "Speicherort ändern";
            buttonProjectFolder.Image = (Image)resources.GetObject("buttonProjectFolder.Image");
            buttonProjectFolder.Location = new Point(336, 3);
            buttonProjectFolder.Name = "buttonProjectFolder";
            buttonProjectFolder.Size = new Size(34, 29);
            buttonProjectFolder.TabIndex = 2;
            toolTip.SetToolTip(buttonProjectFolder, "Speicherort ändern");
            buttonProjectFolder.UseVisualStyleBackColor = true;
            buttonProjectFolder.Click += ButtonProjectFolder_Click;
            // 
            // labelProjectHint
            // 
            labelProjectHint.AutoSize = true;
            labelProjectHint.ForeColor = SystemColors.GrayText;
            labelProjectHint.Location = new Point(8, 41);
            labelProjectHint.Name = "labelProjectHint";
            labelProjectHint.Size = new Size(350, 19);
            labelProjectHint.TabIndex = 3;
            labelProjectHint.Text = "Gilt für Dateidialoge. Ändern verschiebt keine Dateien.";
            // 
            // captionProjects
            // 
            captionProjects.Dock = DockStyle.Bottom;
            captionProjects.ForeColor = SystemColors.GrayText;
            captionProjects.Location = new Point(0, 72);
            captionProjects.Name = "captionProjects";
            captionProjects.Size = new Size(376, 18);
            captionProjects.TabIndex = 4;
            captionProjects.Text = "Projekte";
            captionProjects.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerProjects
            // 
            dividerProjects.BackColor = SystemColors.ControlDark;
            dividerProjects.Location = new Point(1008, 4);
            dividerProjects.Margin = new Padding(0, 4, 0, 4);
            dividerProjects.Name = "dividerProjects";
            dividerProjects.Size = new Size(1, 82);
            dividerProjects.TabIndex = 5;
            // 
            // groupAppearance
            // 
            groupAppearance.Controls.Add(radioSystem);
            groupAppearance.Controls.Add(radioLight);
            groupAppearance.Controls.Add(radioDark);
            groupAppearance.Controls.Add(captionAppearance);
            groupAppearance.Location = new Point(397, 0);
            groupAppearance.Margin = new Padding(0);
            groupAppearance.Name = "groupAppearance";
            groupAppearance.Size = new Size(90, 90);
            groupAppearance.TabIndex = 2;
            // 
            // radioSystem
            // 
            radioSystem.AccessibleDescription = "Hell, dunkel oder wie Windows. Gilt ab dem nächsten Start.";
            radioSystem.AutoSize = true;
            radioSystem.Location = new Point(8, 1);
            radioSystem.Name = "radioSystem";
            radioSystem.Size = new Size(71, 23);
            radioSystem.TabIndex = 0;
            radioSystem.TabStop = true;
            radioSystem.Text = "System";
            radioSystem.UseVisualStyleBackColor = true;
            radioSystem.CheckedChanged += RadioColorMode_CheckedChanged;
            // 
            // radioLight
            // 
            radioLight.AccessibleDescription = "Hell, dunkel oder wie Windows. Gilt ab dem nächsten Start.";
            radioLight.AutoSize = true;
            radioLight.Location = new Point(8, 24);
            radioLight.Name = "radioLight";
            radioLight.Size = new Size(50, 23);
            radioLight.TabIndex = 1;
            radioLight.Text = "Hell";
            radioLight.UseVisualStyleBackColor = true;
            radioLight.CheckedChanged += RadioColorMode_CheckedChanged;
            // 
            // radioDark
            // 
            radioDark.AccessibleDescription = "Hell, dunkel oder wie Windows. Gilt ab dem nächsten Start.";
            radioDark.AutoSize = true;
            radioDark.Location = new Point(8, 47);
            radioDark.Name = "radioDark";
            radioDark.Size = new Size(70, 23);
            radioDark.TabIndex = 2;
            radioDark.Text = "Dunkel";
            radioDark.UseVisualStyleBackColor = true;
            radioDark.CheckedChanged += RadioColorMode_CheckedChanged;
            // 
            // captionAppearance
            // 
            captionAppearance.Dock = DockStyle.Bottom;
            captionAppearance.ForeColor = SystemColors.GrayText;
            captionAppearance.Location = new Point(0, 72);
            captionAppearance.Name = "captionAppearance";
            captionAppearance.Size = new Size(90, 18);
            captionAppearance.TabIndex = 6;
            captionAppearance.Text = "Darstellung";
            captionAppearance.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerAppearance
            // 
            dividerAppearance.BackColor = SystemColors.ControlDark;
            dividerAppearance.Location = new Point(565, 4);
            dividerAppearance.Margin = new Padding(0, 4, 0, 4);
            dividerAppearance.Name = "dividerAppearance";
            dividerAppearance.Size = new Size(1, 82);
            dividerAppearance.TabIndex = 3;
            // 
            // groupLanguage
            // 
            groupLanguage.Controls.Add(radioGerman);
            groupLanguage.Controls.Add(radioEnglish);
            groupLanguage.Controls.Add(captionLanguage);
            groupLanguage.Location = new Point(566, 0);
            groupLanguage.Margin = new Padding(0);
            groupLanguage.Name = "groupLanguage";
            groupLanguage.Size = new Size(100, 90);
            groupLanguage.TabIndex = 4;
            // 
            // radioGerman
            // 
            radioGerman.AccessibleDescription = "Die Sprache des Programms. Gilt ab dem nächsten Start.";
            radioGerman.AutoSize = true;
            radioGerman.Location = new Point(8, 5);
            radioGerman.Name = "radioGerman";
            radioGerman.Size = new Size(77, 23);
            radioGerman.TabIndex = 0;
            radioGerman.TabStop = true;
            radioGerman.Text = "Deutsch";
            radioGerman.UseVisualStyleBackColor = true;
            radioGerman.CheckedChanged += RadioLanguage_CheckedChanged;
            // 
            // radioEnglish
            // 
            radioEnglish.AccessibleDescription = "Die Sprache des Programms. Gilt ab dem nächsten Start.";
            radioEnglish.AutoSize = true;
            radioEnglish.Location = new Point(8, 37);
            radioEnglish.Name = "radioEnglish";
            radioEnglish.Size = new Size(70, 23);
            radioEnglish.TabIndex = 1;
            radioEnglish.Text = "English";
            radioEnglish.UseVisualStyleBackColor = true;
            radioEnglish.CheckedChanged += RadioLanguage_CheckedChanged;
            // 
            // captionLanguage
            // 
            captionLanguage.Dock = DockStyle.Bottom;
            captionLanguage.ForeColor = SystemColors.GrayText;
            captionLanguage.Location = new Point(0, 72);
            captionLanguage.Name = "captionLanguage";
            captionLanguage.Size = new Size(100, 18);
            captionLanguage.TabIndex = 2;
            captionLanguage.Text = "Sprache";
            captionLanguage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerLanguage
            // 
            dividerLanguage.BackColor = SystemColors.ControlDark;
            dividerLanguage.Location = new Point(666, 4);
            dividerLanguage.Margin = new Padding(0, 4, 0, 4);
            dividerLanguage.Name = "dividerLanguage";
            dividerLanguage.Size = new Size(1, 82);
            dividerLanguage.TabIndex = 5;
            // 
            // groupKits
            // 
            groupKits.Controls.Add(labelOwnKits);
            groupKits.Controls.Add(buttonOwnKits);
            groupKits.Controls.Add(labelInstalledKits);
            groupKits.Controls.Add(buttonInstalledKits);
            groupKits.Controls.Add(lineKitFolders);
            groupKits.Controls.Add(buttonReloadKits);
            groupKits.Controls.Add(buttonKitGuide);
            groupKits.Controls.Add(captionKits);
            groupKits.Location = new Point(667, 0);
            groupKits.Margin = new Padding(0);
            groupKits.Name = "groupKits";
            groupKits.Size = new Size(280, 90);
            groupKits.TabIndex = 6;
            // 
            // labelOwnKits
            // 
            labelOwnKits.AutoSize = true;
            labelOwnKits.Location = new Point(8, 40);
            labelOwnKits.Name = "labelOwnKits";
            labelOwnKits.Size = new Size(49, 19);
            labelOwnKits.TabIndex = 2;
            labelOwnKits.Text = "Eigene";
            // 
            // buttonOwnKits
            // 
            buttonOwnKits.AccessibleName = "Ordner für eigene Kits öffnen";
            buttonOwnKits.Image = (Image)resources.GetObject("buttonOwnKits.Image");
            buttonOwnKits.Location = new Point(78, 35);
            buttonOwnKits.Name = "buttonOwnKits";
            buttonOwnKits.Size = new Size(34, 29);
            buttonOwnKits.TabIndex = 3;
            toolTip.SetToolTip(buttonOwnKits, "Ordner für eigene Kits öffnen");
            buttonOwnKits.UseVisualStyleBackColor = true;
            buttonOwnKits.Click += ButtonOwnKits_Click;
            // 
            // labelInstalledKits
            // 
            labelInstalledKits.AutoSize = true;
            labelInstalledKits.Location = new Point(8, 8);
            labelInstalledKits.Name = "labelInstalledKits";
            labelInstalledKits.Size = new Size(65, 19);
            labelInstalledKits.TabIndex = 0;
            labelInstalledKits.Text = "Installiert";
            // 
            // buttonInstalledKits
            // 
            buttonInstalledKits.AccessibleName = "Ordner der installierten Kits öffnen";
            buttonInstalledKits.Image = (Image)resources.GetObject("buttonInstalledKits.Image");
            buttonInstalledKits.Location = new Point(78, 3);
            buttonInstalledKits.Name = "buttonInstalledKits";
            buttonInstalledKits.Size = new Size(34, 29);
            buttonInstalledKits.TabIndex = 1;
            toolTip.SetToolTip(buttonInstalledKits, "Ordner der installierten Kits öffnen");
            buttonInstalledKits.UseVisualStyleBackColor = true;
            buttonInstalledKits.Click += ButtonInstalledKits_Click;
            // 
            // lineKitFolders
            // 
            lineKitFolders.BackColor = SystemColors.ScrollBar;
            lineKitFolders.Location = new Point(122, 6);
            lineKitFolders.Name = "lineKitFolders";
            lineKitFolders.Size = new Size(1, 56);
            lineKitFolders.TabIndex = 7;
            // 
            // buttonReloadKits
            // 
            buttonReloadKits.AccessibleDescription = "Sucht die Kits erneut, etwa nach dem Kopieren eines neuen Ordners.";
            buttonReloadKits.Location = new Point(132, 3);
            buttonReloadKits.Name = "buttonReloadKits";
            buttonReloadKits.Size = new Size(140, 29);
            buttonReloadKits.TabIndex = 4;
            buttonReloadKits.Text = "Kits neu einlesen";
            buttonReloadKits.UseVisualStyleBackColor = true;
            buttonReloadKits.Click += ButtonReloadKits_Click;
            // 
            // buttonKitGuide
            // 
            buttonKitGuide.AccessibleDescription = "Erklärt, wie du eigene Kits anlegst und ihre Klänge aufbereitest.";
            buttonKitGuide.Location = new Point(132, 35);
            buttonKitGuide.Name = "buttonKitGuide";
            buttonKitGuide.Size = new Size(140, 29);
            buttonKitGuide.TabIndex = 5;
            buttonKitGuide.Text = "Anleitung…";
            buttonKitGuide.UseVisualStyleBackColor = true;
            buttonKitGuide.Click += ButtonKitGuide_Click;
            // 
            // captionKits
            // 
            captionKits.Dock = DockStyle.Bottom;
            captionKits.ForeColor = SystemColors.GrayText;
            captionKits.Location = new Point(0, 72);
            captionKits.Name = "captionKits";
            captionKits.Size = new Size(280, 18);
            captionKits.TabIndex = 6;
            captionKits.Text = "Drumkits";
            captionKits.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerKits
            // 
            dividerKits.BackColor = SystemColors.ControlDark;
            dividerKits.Location = new Point(947, 4);
            dividerKits.Margin = new Padding(0, 4, 0, 4);
            dividerKits.Name = "dividerKits";
            dividerKits.Size = new Size(1, 82);
            dividerKits.TabIndex = 7;
            // 
            // groupAudio
            // 
            groupAudio.Controls.Add(labelAudioDevice);
            groupAudio.Controls.Add(comboAudioDevice);
            groupAudio.Controls.Add(lineLatency);
            groupAudio.Controls.Add(labelLatency);
            groupAudio.Controls.Add(numericLatency);
            groupAudio.Controls.Add(labelLatencyUnit);
            groupAudio.Controls.Add(buttonLatencyDefault);
            groupAudio.Controls.Add(captionAudio);
            groupAudio.Location = new Point(948, 0);
            groupAudio.Margin = new Padding(0);
            groupAudio.Name = "groupAudio";
            groupAudio.Size = new Size(456, 90);
            groupAudio.TabIndex = 8;
            // 
            // labelAudioDevice
            // 
            labelAudioDevice.AccessibleDescription = "Über dieses Gerät spielt drums4me, zum Beispiel ein Audio-Interface für den Kopfhörer. Windows-Standard folgt der Einstellung von Windows.";
            labelAudioDevice.AutoSize = true;
            labelAudioDevice.Location = new Point(8, 9);
            labelAudioDevice.Name = "labelAudioDevice";
            labelAudioDevice.Size = new Size(94, 19);
            labelAudioDevice.TabIndex = 0;
            labelAudioDevice.Text = "Ausgabegerät";
            // 
            // comboAudioDevice
            // 
            comboAudioDevice.AccessibleDescription = "Über dieses Gerät spielt drums4me, zum Beispiel ein Audio-Interface für den Kopfhörer. Windows-Standard folgt der Einstellung von Windows.";
            comboAudioDevice.AccessibleName = "Ausgabegerät";
            comboAudioDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            comboAudioDevice.DropDownWidth = 340;
            comboAudioDevice.Location = new Point(8, 35);
            comboAudioDevice.Name = "comboAudioDevice";
            comboAudioDevice.Size = new Size(283, 28);
            comboAudioDevice.TabIndex = 1;
            comboAudioDevice.VisualStylesMode = VisualStylesMode.Net11;
            comboAudioDevice.DropDown += ComboAudioDevice_DropDown;
            comboAudioDevice.SelectedIndexChanged += ComboAudioDevice_SelectedIndexChanged;
            // 
            // lineLatency
            // 
            lineLatency.BackColor = SystemColors.ScrollBar;
            lineLatency.Location = new Point(300, 6);
            lineLatency.Name = "lineLatency";
            lineLatency.Size = new Size(1, 56);
            lineLatency.TabIndex = 2;
            // 
            // labelLatency
            // 
            labelLatency.AccessibleDescription = "Puffer der Tonausgabe in Millisekunden. Größer hilft, wenn der Ton knackt oder aussetzt. Kleiner lässt den Ton schneller auf Eingaben folgen. Standard: 40.";
            labelLatency.AutoSize = true;
            labelLatency.Location = new Point(310, 9);
            labelLatency.Name = "labelLatency";
            labelLatency.Size = new Size(48, 19);
            labelLatency.TabIndex = 3;
            labelLatency.Text = "Latenz";
            // 
            // numericLatency
            // 
            numericLatency.AccessibleDescription = "Puffer der Tonausgabe in Millisekunden. Größer hilft, wenn der Ton knackt oder aussetzt. Kleiner lässt den Ton schneller auf Eingaben folgen. Standard: 40.";
            numericLatency.AccessibleName = "Latenz in Millisekunden";
            numericLatency.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            numericLatency.Location = new Point(366, 6);
            numericLatency.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            numericLatency.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numericLatency.Name = "numericLatency";
            numericLatency.Size = new Size(54, 25);
            numericLatency.TabIndex = 4;
            numericLatency.TextAlign = HorizontalAlignment.Center;
            numericLatency.Value = new decimal(new int[] { 40, 0, 0, 0 });
            numericLatency.ValueChanged += NumericLatency_ValueChanged;
            // 
            // labelLatencyUnit
            // 
            labelLatencyUnit.AccessibleDescription = "Puffer der Tonausgabe in Millisekunden. Größer hilft, wenn der Ton knackt oder aussetzt. Kleiner lässt den Ton schneller auf Eingaben folgen. Standard: 40.";
            labelLatencyUnit.AutoSize = true;
            labelLatencyUnit.Location = new Point(424, 9);
            labelLatencyUnit.Name = "labelLatencyUnit";
            labelLatencyUnit.Size = new Size(26, 19);
            labelLatencyUnit.TabIndex = 5;
            labelLatencyUnit.Text = "ms";
            // 
            // buttonLatencyDefault
            // 
            buttonLatencyDefault.AccessibleDescription = "Stellt die Latenz auf den Standardwert von 40 Millisekunden zurück.";
            buttonLatencyDefault.Enabled = false;
            buttonLatencyDefault.Location = new Point(310, 35);
            buttonLatencyDefault.Name = "buttonLatencyDefault";
            buttonLatencyDefault.Size = new Size(138, 29);
            buttonLatencyDefault.TabIndex = 6;
            buttonLatencyDefault.Text = "Standard";
            buttonLatencyDefault.UseVisualStyleBackColor = true;
            buttonLatencyDefault.Click += ButtonLatencyDefault_Click;
            // 
            // captionAudio
            // 
            captionAudio.Dock = DockStyle.Bottom;
            captionAudio.ForeColor = SystemColors.GrayText;
            captionAudio.Location = new Point(0, 72);
            captionAudio.Name = "captionAudio";
            captionAudio.Size = new Size(456, 18);
            captionAudio.TabIndex = 7;
            captionAudio.Text = "Audio";
            captionAudio.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerAudio
            // 
            dividerAudio.BackColor = SystemColors.ControlDark;
            dividerAudio.Location = new Point(1247, 4);
            dividerAudio.Margin = new Padding(0, 4, 0, 4);
            dividerAudio.Name = "dividerAudio";
            dividerAudio.Size = new Size(1, 82);
            dividerAudio.TabIndex = 9;
            // 
            // groupPlayback
            // 
            groupPlayback.Controls.Add(labelClickVolume);
            groupPlayback.Controls.Add(numericClickVolume);
            groupPlayback.Controls.Add(labelClickVolumeUnit);
            groupPlayback.Controls.Add(captionPlayback);
            groupPlayback.Location = new Point(931, 0);
            groupPlayback.Margin = new Padding(0);
            groupPlayback.Name = "groupPlayback";
            groupPlayback.Size = new Size(184, 90);
            groupPlayback.TabIndex = 10;
            // 
            // labelClickVolume
            // 
            labelClickVolume.AccessibleDescription = "Lautstärke der Klicks beim Vorzählen in Prozent der Gesamtlautstärke. Nach jeder Änderung hörst du den Klick.";
            labelClickVolume.Location = new Point(8, 13);
            labelClickVolume.Name = "labelClickVolume";
            labelClickVolume.Size = new Size(80, 40);
            labelClickVolume.TabIndex = 0;
            labelClickVolume.Text = "Lautstärke Vorzählen";
            // 
            // numericClickVolume
            // 
            numericClickVolume.AccessibleDescription = "Lautstärke der Klicks beim Vorzählen in Prozent der Gesamtlautstärke. Nach jeder Änderung hörst du den Klick.";
            numericClickVolume.AccessibleName = "Lautstärke Vorzählen";
            numericClickVolume.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            numericClickVolume.Location = new Point(96, 20);
            numericClickVolume.Name = "numericClickVolume";
            numericClickVolume.Size = new Size(54, 25);
            numericClickVolume.TabIndex = 1;
            numericClickVolume.TextAlign = HorizontalAlignment.Center;
            numericClickVolume.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numericClickVolume.ValueChanged += NumericClickVolume_ValueChanged;
            // 
            // labelClickVolumeUnit
            // 
            labelClickVolumeUnit.AccessibleDescription = "Lautstärke der Klicks beim Vorzählen in Prozent der Gesamtlautstärke. Nach jeder Änderung hörst du den Klick.";
            labelClickVolumeUnit.AutoSize = true;
            labelClickVolumeUnit.Location = new Point(154, 23);
            labelClickVolumeUnit.Name = "labelClickVolumeUnit";
            labelClickVolumeUnit.Size = new Size(19, 19);
            labelClickVolumeUnit.TabIndex = 2;
            labelClickVolumeUnit.Text = "%";
            // 
            // captionPlayback
            // 
            captionPlayback.Dock = DockStyle.Bottom;
            captionPlayback.ForeColor = SystemColors.GrayText;
            captionPlayback.Location = new Point(0, 72);
            captionPlayback.Name = "captionPlayback";
            captionPlayback.Size = new Size(184, 18);
            captionPlayback.TabIndex = 3;
            captionPlayback.Text = "Wiedergabe";
            captionPlayback.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dividerPlayback
            // 
            dividerPlayback.BackColor = SystemColors.ControlDark;
            dividerPlayback.Location = new Point(1084, 4);
            dividerPlayback.Margin = new Padding(0, 4, 0, 4);
            dividerPlayback.Name = "dividerPlayback";
            dividerPlayback.Size = new Size(1, 82);
            dividerPlayback.TabIndex = 11;
            // 
            // ribbonImages
            // 
            ribbonImages.ColorDepth = ColorDepth.Depth32Bit;
            ribbonImages.ImageStream = (ImageListStreamer)resources.GetObject("ribbonImages.ImageStream");
            ribbonImages.TransparentColor = Color.Transparent;
            ribbonImages.Images.SetKeyName(0, "file");
            // 
            // panelFileCommands
            // 
            panelFileCommands.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panelFileCommands.Controls.Add(buttonNew);
            panelFileCommands.Controls.Add(buttonSave);
            panelFileCommands.Controls.Add(buttonSaveAs);
            panelFileCommands.Controls.Add(buttonExportAudio);
            panelFileCommands.Controls.Add(buttonCloseProject);
            panelFileCommands.Controls.Add(lineCommands);
            panelFileCommands.Controls.Add(buttonOtherFolder);
            panelFileCommands.Controls.Add(buttonDefaultFolder);
            panelFileCommands.Controls.Add(buttonDemos);
            panelFileCommands.Controls.Add(buttonImportMidi);
            panelFileCommands.Controls.Add(lineWindowCommands);
            panelFileCommands.Controls.Add(buttonResetLayout);
            panelFileCommands.Controls.Add(buttonFileAbout);
            panelFileCommands.Controls.Add(buttonExit);
            panelFileCommands.Controls.Add(lineFileCommands);
            panelFileCommands.Location = new Point(0, 25);
            panelFileCommands.Name = "panelFileCommands";
            panelFileCommands.Size = new Size(240, 605);
            panelFileCommands.TabIndex = 9;
            panelFileCommands.Visible = false;
            // 
            // buttonNew
            // 
            buttonNew.AccessibleDescription = "Beginnt ein neues Projekt. Strg+N.";
            buttonNew.BackColor = Color.Transparent;
            buttonNew.FlatAppearance.BorderSize = 0;
            buttonNew.FlatAppearance.MouseOverBackColor = SystemColors.ControlLight;
            buttonNew.FlatStyle = FlatStyle.Flat;
            buttonNew.Image = (Image)resources.GetObject("buttonNew.Image");
            buttonNew.ImageAlign = ContentAlignment.MiddleLeft;
            buttonNew.Location = new Point(0, 12);
            buttonNew.Name = "buttonNew";
            buttonNew.Padding = new Padding(16, 0, 0, 0);
            buttonNew.Size = new Size(239, 36);
            buttonNew.TabIndex = 0;
            buttonNew.Text = "Neu";
            buttonNew.TextAlign = ContentAlignment.MiddleLeft;
            buttonNew.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonNew.UseVisualStyleBackColor = false;
            buttonNew.Click += ButtonNew_Click;
            // 
            // buttonSave
            // 
            buttonSave.AccessibleDescription = "Speichert das Projekt. Strg+S.";
            buttonSave.BackColor = Color.Transparent;
            buttonSave.FlatAppearance.BorderSize = 0;
            buttonSave.FlatAppearance.MouseOverBackColor = SystemColors.ControlLight;
            buttonSave.FlatStyle = FlatStyle.Flat;
            buttonSave.Image = (Image)resources.GetObject("buttonSave.Image");
            buttonSave.ImageAlign = ContentAlignment.MiddleLeft;
            buttonSave.Location = new Point(0, 52);
            buttonSave.Name = "buttonSave";
            buttonSave.Padding = new Padding(16, 0, 0, 0);
            buttonSave.Size = new Size(239, 36);
            buttonSave.TabIndex = 1;
            buttonSave.Text = "Speichern";
            buttonSave.TextAlign = ContentAlignment.MiddleLeft;
            buttonSave.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonSave.UseVisualStyleBackColor = false;
            buttonSave.Click += ButtonSave_Click;
            // 
            // buttonSaveAs
            // 
            buttonSaveAs.AccessibleDescription = "Speichert das Projekt unter einem anderen Namen. Strg+Umschalt+S.";
            buttonSaveAs.BackColor = Color.Transparent;
            buttonSaveAs.FlatAppearance.BorderSize = 0;
            buttonSaveAs.FlatAppearance.MouseOverBackColor = SystemColors.ControlLight;
            buttonSaveAs.FlatStyle = FlatStyle.Flat;
            buttonSaveAs.Image = (Image)resources.GetObject("buttonSaveAs.Image");
            buttonSaveAs.ImageAlign = ContentAlignment.MiddleLeft;
            buttonSaveAs.Location = new Point(0, 92);
            buttonSaveAs.Name = "buttonSaveAs";
            buttonSaveAs.Padding = new Padding(16, 0, 0, 0);
            buttonSaveAs.Size = new Size(239, 36);
            buttonSaveAs.TabIndex = 2;
            buttonSaveAs.Text = "Speichern unter…";
            buttonSaveAs.TextAlign = ContentAlignment.MiddleLeft;
            buttonSaveAs.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonSaveAs.UseVisualStyleBackColor = false;
            buttonSaveAs.Click += ButtonSaveAs_Click;
            // 
            // buttonExportAudio
            // 
            buttonExportAudio.AccessibleDescription = "Speichert, was die Wiedergabe spielt, als WAV, FLAC oder MP3. Länge und Format wählst du im nächsten Fenster.";
            buttonExportAudio.BackColor = Color.Transparent;
            buttonExportAudio.FlatAppearance.BorderSize = 0;
            buttonExportAudio.FlatAppearance.MouseOverBackColor = SystemColors.ControlLight;
            buttonExportAudio.FlatStyle = FlatStyle.Flat;
            buttonExportAudio.Image = (Image)resources.GetObject("buttonExportAudio.Image");
            buttonExportAudio.ImageAlign = ContentAlignment.MiddleLeft;
            buttonExportAudio.Location = new Point(0, 132);
            buttonExportAudio.Name = "buttonExportAudio";
            buttonExportAudio.Padding = new Padding(16, 0, 0, 0);
            buttonExportAudio.Size = new Size(239, 36);
            buttonExportAudio.TabIndex = 3;
            buttonExportAudio.Text = "Als Audiodatei speichern…";
            buttonExportAudio.TextAlign = ContentAlignment.MiddleLeft;
            buttonExportAudio.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonExportAudio.UseVisualStyleBackColor = false;
            buttonExportAudio.Click += ButtonExportAudio_Click;
            // 
            // buttonCloseProject
            // 
            buttonCloseProject.AccessibleDescription = "Schließt die Datei und leert das Raster. Beim nächsten Start wird sie nicht wieder geöffnet.";
            buttonCloseProject.BackColor = Color.Transparent;
            buttonCloseProject.FlatAppearance.BorderSize = 0;
            buttonCloseProject.FlatAppearance.MouseOverBackColor = SystemColors.ControlLight;
            buttonCloseProject.FlatStyle = FlatStyle.Flat;
            buttonCloseProject.Image = (Image)resources.GetObject("buttonCloseProject.Image");
            buttonCloseProject.ImageAlign = ContentAlignment.MiddleLeft;
            buttonCloseProject.Location = new Point(0, 172);
            buttonCloseProject.Name = "buttonCloseProject";
            buttonCloseProject.Padding = new Padding(16, 0, 0, 0);
            buttonCloseProject.Size = new Size(239, 36);
            buttonCloseProject.TabIndex = 4;
            buttonCloseProject.Text = "Schließen";
            buttonCloseProject.TextAlign = ContentAlignment.MiddleLeft;
            buttonCloseProject.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonCloseProject.UseVisualStyleBackColor = false;
            buttonCloseProject.Click += ButtonCloseProject_Click;
            // 
            // lineCommands
            // 
            lineCommands.BackColor = SystemColors.ScrollBar;
            lineCommands.Location = new Point(16, 216);
            lineCommands.Name = "lineCommands";
            lineCommands.Size = new Size(208, 1);
            lineCommands.TabIndex = 5;
            // 
            // buttonOtherFolder
            // 
            buttonOtherFolder.AccessibleDescription = "Zeigt die Projekte eines anderen Ordners. Der Speicherort in den Einstellungen bleibt, wie er ist.";
            buttonOtherFolder.BackColor = Color.Transparent;
            buttonOtherFolder.FlatAppearance.BorderSize = 0;
            buttonOtherFolder.FlatAppearance.MouseOverBackColor = SystemColors.ControlLight;
            buttonOtherFolder.FlatStyle = FlatStyle.Flat;
            buttonOtherFolder.Image = (Image)resources.GetObject("buttonOtherFolder.Image");
            buttonOtherFolder.ImageAlign = ContentAlignment.MiddleLeft;
            buttonOtherFolder.Location = new Point(0, 225);
            buttonOtherFolder.Name = "buttonOtherFolder";
            buttonOtherFolder.Padding = new Padding(16, 0, 0, 0);
            buttonOtherFolder.Size = new Size(239, 36);
            buttonOtherFolder.TabIndex = 6;
            buttonOtherFolder.Text = "Anderen Ordner…";
            buttonOtherFolder.TextAlign = ContentAlignment.MiddleLeft;
            buttonOtherFolder.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonOtherFolder.UseVisualStyleBackColor = false;
            buttonOtherFolder.Click += ButtonOtherFolder_Click;
            // 
            // buttonDefaultFolder
            // 
            buttonDefaultFolder.AccessibleDescription = "Zeigt wieder die Projekte im Speicherort aus den Einstellungen.";
            buttonDefaultFolder.BackColor = Color.Transparent;
            buttonDefaultFolder.FlatAppearance.BorderSize = 0;
            buttonDefaultFolder.FlatAppearance.MouseOverBackColor = SystemColors.ControlLight;
            buttonDefaultFolder.FlatStyle = FlatStyle.Flat;
            buttonDefaultFolder.Image = (Image)resources.GetObject("buttonDefaultFolder.Image");
            buttonDefaultFolder.ImageAlign = ContentAlignment.MiddleLeft;
            buttonDefaultFolder.Location = new Point(0, 265);
            buttonDefaultFolder.Name = "buttonDefaultFolder";
            buttonDefaultFolder.Padding = new Padding(16, 0, 0, 0);
            buttonDefaultFolder.Size = new Size(239, 36);
            buttonDefaultFolder.TabIndex = 7;
            buttonDefaultFolder.Text = "Standardordner";
            buttonDefaultFolder.TextAlign = ContentAlignment.MiddleLeft;
            buttonDefaultFolder.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonDefaultFolder.UseVisualStyleBackColor = false;
            buttonDefaultFolder.Click += ButtonDefaultFolder_Click;
            // 
            // buttonDemos
            // 
            buttonDemos.AccessibleDescription = "Öffnet eines der installierten Demo-Projekte.";
            buttonDemos.BackColor = Color.Transparent;
            buttonDemos.FlatAppearance.BorderSize = 0;
            buttonDemos.FlatAppearance.MouseOverBackColor = SystemColors.ControlLight;
            buttonDemos.FlatStyle = FlatStyle.Flat;
            buttonDemos.Image = (Image)resources.GetObject("buttonDemos.Image");
            buttonDemos.ImageAlign = ContentAlignment.MiddleLeft;
            buttonDemos.Location = new Point(0, 305);
            buttonDemos.Name = "buttonDemos";
            buttonDemos.Padding = new Padding(16, 0, 0, 0);
            buttonDemos.Size = new Size(239, 36);
            buttonDemos.TabIndex = 8;
            buttonDemos.Text = "Demos öffnen▸";
            buttonDemos.TextAlign = ContentAlignment.MiddleLeft;
            buttonDemos.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonDemos.UseVisualStyleBackColor = false;
            buttonDemos.Click += ButtonDemos_Click;
            // 
            // buttonImportMidi
            // 
            buttonImportMidi.AccessibleDescription = "Liest das Schlagzeug einer MIDI-Datei und macht aus bis zu vier Takten ein neues Projekt. Gedacht für kurze Grooves, nicht für ganze Songs.";
            buttonImportMidi.BackColor = Color.Transparent;
            buttonImportMidi.FlatAppearance.BorderSize = 0;
            buttonImportMidi.FlatAppearance.MouseOverBackColor = SystemColors.ControlLight;
            buttonImportMidi.FlatStyle = FlatStyle.Flat;
            buttonImportMidi.Image = (Image)resources.GetObject("buttonImportMidi.Image");
            buttonImportMidi.ImageAlign = ContentAlignment.MiddleLeft;
            buttonImportMidi.Location = new Point(0, 345);
            buttonImportMidi.Name = "buttonImportMidi";
            buttonImportMidi.Padding = new Padding(16, 0, 0, 0);
            buttonImportMidi.Size = new Size(239, 36);
            buttonImportMidi.TabIndex = 9;
            buttonImportMidi.Text = "MIDI importieren…";
            buttonImportMidi.TextAlign = ContentAlignment.MiddleLeft;
            buttonImportMidi.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonImportMidi.UseVisualStyleBackColor = false;
            buttonImportMidi.Click += ButtonImportMidi_Click;
            // 
            // lineWindowCommands
            // 
            lineWindowCommands.BackColor = SystemColors.ScrollBar;
            lineWindowCommands.Location = new Point(16, 389);
            lineWindowCommands.Name = "lineWindowCommands";
            lineWindowCommands.Size = new Size(208, 1);
            lineWindowCommands.TabIndex = 10;
            // 
            // buttonResetLayout
            // 
            buttonResetLayout.AccessibleDescription = "Stellt Fenstergröße, Fensterlage und Spaltenbreiten der Projektliste wieder her wie beim ersten Start.";
            buttonResetLayout.BackColor = Color.Transparent;
            buttonResetLayout.FlatAppearance.BorderSize = 0;
            buttonResetLayout.FlatAppearance.MouseOverBackColor = SystemColors.ControlLight;
            buttonResetLayout.FlatStyle = FlatStyle.Flat;
            buttonResetLayout.Image = (Image)resources.GetObject("buttonResetLayout.Image");
            buttonResetLayout.ImageAlign = ContentAlignment.MiddleLeft;
            buttonResetLayout.Location = new Point(0, 398);
            buttonResetLayout.Name = "buttonResetLayout";
            buttonResetLayout.Padding = new Padding(16, 0, 0, 0);
            buttonResetLayout.Size = new Size(239, 36);
            buttonResetLayout.TabIndex = 11;
            buttonResetLayout.Text = "Fensterlayout zurücksetzen";
            buttonResetLayout.TextAlign = ContentAlignment.MiddleLeft;
            buttonResetLayout.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonResetLayout.UseVisualStyleBackColor = false;
            buttonResetLayout.Click += ButtonResetLayout_Click;
            // 
            // buttonFileAbout
            // 
            buttonFileAbout.AccessibleDescription = "Zeigt Version und Lizenz von drums4me.";
            buttonFileAbout.BackColor = Color.Transparent;
            buttonFileAbout.FlatAppearance.BorderSize = 0;
            buttonFileAbout.FlatAppearance.MouseOverBackColor = SystemColors.ControlLight;
            buttonFileAbout.FlatStyle = FlatStyle.Flat;
            buttonFileAbout.Image = (Image)resources.GetObject("buttonFileAbout.Image");
            buttonFileAbout.ImageAlign = ContentAlignment.MiddleLeft;
            buttonFileAbout.Location = new Point(0, 438);
            buttonFileAbout.Name = "buttonFileAbout";
            buttonFileAbout.Padding = new Padding(16, 0, 0, 0);
            buttonFileAbout.Size = new Size(239, 36);
            buttonFileAbout.TabIndex = 12;
            buttonFileAbout.Text = "Über";
            buttonFileAbout.TextAlign = ContentAlignment.MiddleLeft;
            buttonFileAbout.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonFileAbout.UseVisualStyleBackColor = false;
            buttonFileAbout.Click += ButtonAbout_Click;
            // 
            // buttonExit
            // 
            buttonExit.AccessibleDescription = "Beendet drums4me. Ungesicherte Änderungen werden vorher abgefragt.";
            buttonExit.BackColor = Color.Transparent;
            buttonExit.FlatAppearance.BorderSize = 0;
            buttonExit.FlatAppearance.MouseOverBackColor = SystemColors.ControlLight;
            buttonExit.FlatStyle = FlatStyle.Flat;
            buttonExit.Image = (Image)resources.GetObject("buttonExit.Image");
            buttonExit.ImageAlign = ContentAlignment.MiddleLeft;
            buttonExit.Location = new Point(0, 478);
            buttonExit.Name = "buttonExit";
            buttonExit.Padding = new Padding(16, 0, 0, 0);
            buttonExit.Size = new Size(239, 36);
            buttonExit.TabIndex = 13;
            buttonExit.Text = "Beenden";
            buttonExit.TextAlign = ContentAlignment.MiddleLeft;
            buttonExit.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonExit.UseVisualStyleBackColor = false;
            buttonExit.Click += ButtonExit_Click;
            // 
            // lineFileCommands
            // 
            lineFileCommands.BackColor = SystemColors.ControlDark;
            lineFileCommands.Dock = DockStyle.Right;
            lineFileCommands.Location = new Point(239, 0);
            lineFileCommands.Name = "lineFileCommands";
            lineFileCommands.Size = new Size(1, 605);
            lineFileCommands.TabIndex = 14;
            // 
            // tabMarker
            // 
            tabMarker.Location = new Point(66, 52);
            tabMarker.Name = "tabMarker";
            tabMarker.Size = new Size(48, 3);
            tabMarker.TabIndex = 10;
            // 
            // buttonTopMost
            // 
            buttonTopMost.AccessibleDescription = "Hält das Fenster über anderen Programmen, etwa über Noten oder Tabulaturen. Ein Klick schaltet um.";
            buttonTopMost.AccessibleName = "Immer im Vordergrund";
            buttonTopMost.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonTopMost.Location = new Point(1032, 0);
            buttonTopMost.Name = "buttonTopMost";
            buttonTopMost.Size = new Size(28, 24);
            buttonTopMost.TabIndex = 11;
            buttonTopMost.UseVisualStyleBackColor = true;
            buttonTopMost.Click += ButtonTopMost_Click;
            buttonTopMost.Paint += ButtonTopMost_Paint;
            // 
            // buttonKeepScreenOn
            // 
            buttonKeepScreenOn.AccessibleDescription = "Verhindert, dass Windows den Bildschirm abschaltet, solange die Wiedergabe läuft. Ein Klick schaltet um.";
            buttonKeepScreenOn.AccessibleName = "Bildschirm anlassen";
            buttonKeepScreenOn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonKeepScreenOn.Location = new Point(1062, 0);
            buttonKeepScreenOn.Name = "buttonKeepScreenOn";
            buttonKeepScreenOn.Size = new Size(28, 24);
            buttonKeepScreenOn.TabIndex = 12;
            buttonKeepScreenOn.UseVisualStyleBackColor = true;
            buttonKeepScreenOn.Click += ButtonKeepScreenOn_Click;
            buttonKeepScreenOn.Paint += ButtonKeepScreenOn_Paint;
            // 
            // buttonUndo
            // 
            buttonUndo.AccessibleDescription = "Nimmt die letzte Änderung zurück. Strg+Z.";
            buttonUndo.AccessibleName = "Rückgängig";
            buttonUndo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonUndo.Enabled = false;
            buttonUndo.Image = (Image)resources.GetObject("buttonUndo.Image");
            buttonUndo.Location = new Point(1100, 0);
            buttonUndo.Name = "buttonUndo";
            buttonUndo.Size = new Size(28, 24);
            buttonUndo.TabIndex = 1;
            toolTip.SetToolTip(buttonUndo, "Rückgängig (Strg+Z)");
            buttonUndo.UseVisualStyleBackColor = true;
            buttonUndo.Click += ButtonUndo_Click;
            // 
            // buttonRedo
            // 
            buttonRedo.AccessibleDescription = "Stellt die zurückgenommene Änderung wieder her. Strg+Y.";
            buttonRedo.AccessibleName = "Wiederholen";
            buttonRedo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonRedo.Enabled = false;
            buttonRedo.Image = (Image)resources.GetObject("buttonRedo.Image");
            buttonRedo.Location = new Point(1130, 0);
            buttonRedo.Name = "buttonRedo";
            buttonRedo.Size = new Size(28, 24);
            buttonRedo.TabIndex = 2;
            toolTip.SetToolTip(buttonRedo, "Wiederholen (Strg+Y)");
            buttonRedo.UseVisualStyleBackColor = true;
            buttonRedo.Click += ButtonRedo_Click;
            // 
            // buttonQuickSave
            // 
            buttonQuickSave.AccessibleDescription = "Speichert das Projekt. Strg+S.";
            buttonQuickSave.AccessibleName = "Speichern";
            buttonQuickSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonQuickSave.Image = (Image)resources.GetObject("buttonQuickSave.Image");
            buttonQuickSave.Location = new Point(1160, 0);
            buttonQuickSave.Name = "buttonQuickSave";
            buttonQuickSave.Size = new Size(28, 24);
            buttonQuickSave.TabIndex = 3;
            toolTip.SetToolTip(buttonQuickSave, "Speichern (Strg+S)");
            buttonQuickSave.UseVisualStyleBackColor = true;
            buttonQuickSave.Click += ButtonSave_Click;
            // 
            // buttonShortcuts
            // 
            buttonShortcuts.AccessibleDescription = "Zeigt die Tastenkombinationen. F1.";
            buttonShortcuts.AccessibleName = "Tastenkombinationen";
            buttonShortcuts.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonShortcuts.Image = (Image)resources.GetObject("buttonShortcuts.Image");
            buttonShortcuts.Location = new Point(1198, 0);
            buttonShortcuts.Name = "buttonShortcuts";
            buttonShortcuts.Size = new Size(28, 24);
            buttonShortcuts.TabIndex = 4;
            toolTip.SetToolTip(buttonShortcuts, "Tastenkombinationen (F1)");
            buttonShortcuts.UseVisualStyleBackColor = true;
            buttonShortcuts.Click += ButtonShortcuts_Click;
            // 
            // buttonAbout
            // 
            buttonAbout.AccessibleDescription = "Zeigt Version und Lizenz von drums4me.";
            buttonAbout.AccessibleName = "Über drums4me";
            buttonAbout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonAbout.Image = (Image)resources.GetObject("buttonAbout.Image");
            buttonAbout.Location = new Point(1228, 0);
            buttonAbout.Name = "buttonAbout";
            buttonAbout.Size = new Size(28, 24);
            buttonAbout.TabIndex = 5;
            toolTip.SetToolTip(buttonAbout, "Über drums4me");
            buttonAbout.UseVisualStyleBackColor = true;
            buttonAbout.Click += ButtonAbout_Click;
            // 
            // stepGrid
            // 
            stepGrid.BackColor = SystemColors.Window;
            stepGrid.Dock = DockStyle.Fill;
            stepGrid.Location = new Point(0, 124);
            stepGrid.Name = "stepGrid";
            stepGrid.Size = new Size(1260, 506);
            stepGrid.TabIndex = 6;
            stepGrid.Edited += StepGrid_Edited;
            stepGrid.EditCompleted += StepGrid_EditCompleted;
            stepGrid.TrackNameClicked += StepGrid_TrackNameClicked;
            stepGrid.BeatMenuRequested += StepGrid_BeatMenuRequested;
            stepGrid.BeatCopied += StepGrid_BeatCopied;
            stepGrid.EmptyStepRightClicked += StepGrid_EmptyStepRightClicked;
            stepGrid.MouseDown += StepGrid_MouseDown;
            // 
            // projectBrowser
            // 
            projectBrowser.Dock = DockStyle.Fill;
            projectBrowser.Font = new Font("Segoe UI", 10F);
            projectBrowser.Location = new Point(0, 124);
            projectBrowser.Name = "projectBrowser";
            projectBrowser.Padding = new Padding(240, 0, 0, 0);
            projectBrowser.Size = new Size(1260, 506);
            projectBrowser.TabIndex = 7;
            projectBrowser.Visible = false;
            projectBrowser.ProjectChosen += ProjectBrowser_ProjectChosen;
            projectBrowser.CountChanged += ProjectBrowser_CountChanged;
            projectBrowser.RenameRequested += ProjectBrowser_RenameRequested;
            projectBrowser.DuplicateRequested += ProjectBrowser_DuplicateRequested;
            projectBrowser.DeleteRequested += ProjectBrowser_DeleteRequested;
            // 
            // statusStrip
            // 
            statusStrip.Font = new Font("Segoe UI", 10F);
            statusStrip.Items.AddRange(new ToolStripItem[] { statusHint, statusProjects, statusTrainer, statusBars, statusTime });
            statusStrip.Location = new Point(0, 630);
            statusStrip.Name = "statusStrip";
            statusStrip.ShowItemToolTips = true;
            statusStrip.Size = new Size(1260, 24);
            statusStrip.TabIndex = 8;
            // 
            // statusHint
            // 
            statusHint.Name = "statusHint";
            statusHint.Size = new Size(1032, 19);
            statusHint.Spring = true;
            statusHint.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // statusProjects
            // 
            statusProjects.BorderSides = ToolStripStatusLabelBorderSides.Left;
            statusProjects.Name = "statusProjects";
            statusProjects.Size = new Size(4, 19);
            statusProjects.Visible = false;
            // 
            // statusTrainer
            // 
            statusTrainer.BorderSides = ToolStripStatusLabelBorderSides.Left;
            statusTrainer.Name = "statusTrainer";
            statusTrainer.Size = new Size(4, 19);
            statusTrainer.Visible = false;
            // 
            // statusBars
            // 
            statusBars.AutoSize = false;
            statusBars.BorderSides = ToolStripStatusLabelBorderSides.Left;
            statusBars.Name = "statusBars";
            statusBars.Size = new Size(90, 19);
            statusBars.Text = "Takt 0";
            // 
            // statusTime
            // 
            statusTime.AutoSize = false;
            statusTime.BorderSides = ToolStripStatusLabelBorderSides.Left;
            statusTime.Name = "statusTime";
            statusTime.Size = new Size(120, 19);
            statusTime.Text = "Spielzeit 0:00";
            // 
            // playheadTimer
            // 
            playheadTimer.Interval = 15;
            playheadTimer.Tick += PlayheadTimer_Tick;
            // 
            // previewTimer
            // 
            previewTimer.Interval = 50;
            previewTimer.Tick += PreviewTimer_Tick;
            // 
            // noticeTimer
            // 
            noticeTimer.Interval = 4000;
            noticeTimer.Tick += NoticeTimer_Tick;
            // 
            // restartTimer
            // 
            restartTimer.Interval = 1000;
            restartTimer.Tick += RestartTimer_Tick;
            // 
            // trackNameMenu
            // 
            trackNameMenu.Font = new Font("Segoe UI", 10F);
            trackNameMenu.Items.AddRange(new ToolStripItem[] { previewTrackToolStripMenuItem, renameTrackToolStripMenuItem });
            trackNameMenu.Name = "trackNameMenu";
            trackNameMenu.Size = new Size(172, 52);
            // 
            // previewTrackToolStripMenuItem
            // 
            previewTrackToolStripMenuItem.Name = "previewTrackToolStripMenuItem";
            previewTrackToolStripMenuItem.Size = new Size(171, 24);
            previewTrackToolStripMenuItem.Text = "&Anhören";
            previewTrackToolStripMenuItem.Click += PreviewTrackToolStripMenuItem_Click;
            // 
            // renameTrackToolStripMenuItem
            // 
            renameTrackToolStripMenuItem.Name = "renameTrackToolStripMenuItem";
            renameTrackToolStripMenuItem.Size = new Size(171, 24);
            renameTrackToolStripMenuItem.Text = "&Umbenennen…";
            renameTrackToolStripMenuItem.Click += RenameTrackToolStripMenuItem_Click;
            // 
            // patternMenu
            // 
            patternMenu.Font = new Font("Segoe UI", 10F);
            patternMenu.Items.AddRange(new ToolStripItem[] { copyPatternToolStripMenuItem, pastePatternToolStripMenuItem, clearPatternToolStripMenuItem });
            patternMenu.Name = "patternMenu";
            patternMenu.Size = new Size(141, 76);
            patternMenu.Opening += PatternMenu_Opening;
            // 
            // copyPatternToolStripMenuItem
            // 
            copyPatternToolStripMenuItem.Name = "copyPatternToolStripMenuItem";
            copyPatternToolStripMenuItem.Size = new Size(140, 24);
            copyPatternToolStripMenuItem.Text = "&Kopieren";
            copyPatternToolStripMenuItem.Click += CopyPatternToolStripMenuItem_Click;
            // 
            // pastePatternToolStripMenuItem
            // 
            pastePatternToolStripMenuItem.Name = "pastePatternToolStripMenuItem";
            pastePatternToolStripMenuItem.Size = new Size(140, 24);
            pastePatternToolStripMenuItem.Text = "&Einfügen";
            pastePatternToolStripMenuItem.Click += PastePatternToolStripMenuItem_Click;
            // 
            // clearPatternToolStripMenuItem
            // 
            clearPatternToolStripMenuItem.Name = "clearPatternToolStripMenuItem";
            clearPatternToolStripMenuItem.Size = new Size(140, 24);
            clearPatternToolStripMenuItem.Text = "&Leeren";
            clearPatternToolStripMenuItem.Click += ClearPatternToolStripMenuItem_Click;
            // 
            // beatMenu
            // 
            beatMenu.Font = new Font("Segoe UI", 10F);
            beatMenu.Items.AddRange(new ToolStripItem[] { copyBeatToolStripMenuItem, pasteBeatToolStripMenuItem, fillBeatsToolStripMenuItem, clearBeatToolStripMenuItem });
            beatMenu.Name = "beatMenu";
            beatMenu.Size = new Size(290, 100);
            // 
            // copyBeatToolStripMenuItem
            // 
            copyBeatToolStripMenuItem.Name = "copyBeatToolStripMenuItem";
            copyBeatToolStripMenuItem.ShortcutKeyDisplayString = "Strg+C";
            copyBeatToolStripMenuItem.Size = new Size(289, 24);
            copyBeatToolStripMenuItem.Text = "&Kopieren";
            copyBeatToolStripMenuItem.Click += CopyBeatToolStripMenuItem_Click;
            // 
            // pasteBeatToolStripMenuItem
            // 
            pasteBeatToolStripMenuItem.Name = "pasteBeatToolStripMenuItem";
            pasteBeatToolStripMenuItem.ShortcutKeyDisplayString = "Strg+V";
            pasteBeatToolStripMenuItem.Size = new Size(289, 24);
            pasteBeatToolStripMenuItem.Text = "&Einfügen";
            pasteBeatToolStripMenuItem.Click += PasteBeatToolStripMenuItem_Click;
            // 
            // fillBeatsToolStripMenuItem
            // 
            fillBeatsToolStripMenuItem.Name = "fillBeatsToolStripMenuItem";
            fillBeatsToolStripMenuItem.Size = new Size(289, 24);
            fillBeatsToolStripMenuItem.Text = "Auf &alle Zählzeiten übertragen";
            fillBeatsToolStripMenuItem.Click += FillBeatsToolStripMenuItem_Click;
            // 
            // clearBeatToolStripMenuItem
            // 
            clearBeatToolStripMenuItem.Name = "clearBeatToolStripMenuItem";
            clearBeatToolStripMenuItem.Size = new Size(289, 24);
            clearBeatToolStripMenuItem.Text = "&Leeren";
            clearBeatToolStripMenuItem.Click += ClearBeatToolStripMenuItem_Click;
            // 
            // demosMenu
            // 
            demosMenu.Font = new Font("Segoe UI", 10F);
            demosMenu.Items.AddRange(new ToolStripItem[] { noDemosToolStripMenuItem });
            demosMenu.Name = "demosMenu";
            demosMenu.Size = new Size(218, 28);
            // 
            // noDemosToolStripMenuItem
            // 
            noDemosToolStripMenuItem.Enabled = false;
            noDemosToolStripMenuItem.Name = "noDemosToolStripMenuItem";
            noDemosToolStripMenuItem.Size = new Size(217, 24);
            noDemosToolStripMenuItem.Text = "Keine Demos installiert";
            // 
            // pendingOpenTimer
            // 
            pendingOpenTimer.Interval = 200;
            pendingOpenTimer.Tick += PendingOpenTimer_Tick;
            // 
            // otherFolderDialog
            // 
            otherFolderDialog.Description = "Wähle den Ordner, dessen Projekte du sehen möchtest.";
            otherFolderDialog.ShowNewFolderButton = false;
            otherFolderDialog.UseDescriptionForTitle = true;
            // 
            // projectFolderDialog
            // 
            projectFolderDialog.Description = "Wähle den Ordner für deine Projekte.";
            projectFolderDialog.UseDescriptionForTitle = true;
            // 
            // saveFileDialog
            // 
            saveFileDialog.DefaultExt = "d4m";
            saveFileDialog.Filter = "drums4me-Projekte (*.d4m)|*.d4m|Alle Dateien (*.*)|*.*";
            saveFileDialog.Title = "Projekt speichern";
            // 
            // midiFileDialog
            // 
            midiFileDialog.Filter = "MIDI-Dateien (*.mid;*.midi)|*.mid;*.midi|Alle Dateien (*.*)|*.*";
            midiFileDialog.Title = "MIDI-Groove importieren";
            // 
            // FrmDrums4me
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1260, 654);
            Controls.Add(tabMarker);
            Controls.Add(buttonTopMost);
            Controls.Add(buttonKeepScreenOn);
            Controls.Add(buttonUndo);
            Controls.Add(buttonRedo);
            Controls.Add(buttonQuickSave);
            Controls.Add(buttonShortcuts);
            Controls.Add(buttonAbout);
            Controls.Add(panelFileCommands);
            Controls.Add(stepGrid);
            Controls.Add(projectBrowser);
            Controls.Add(statusStrip);
            Controls.Add(ribbonTabs);
            Font = new Font("Segoe UI", 10F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(960, 614);
            Name = "FrmDrums4me";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "drums4me";
            FormClosing += FrmDrums4me_FormClosing;
            Load += FrmDrums4me_Load;
            Resize += FrmDrums4me_Resize;
            ribbonTabs.ResumeLayout(false);
            tabPageFile.ResumeLayout(false);
            flowFile.ResumeLayout(false);
            groupFind.ResumeLayout(false);
            groupFind.PerformLayout();
            groupStartup.ResumeLayout(false);
            groupStartup.PerformLayout();
            tabPageStart.ResumeLayout(false);
            flowStart.ResumeLayout(false);
            groupPlay.ResumeLayout(false);
            groupTempo.ResumeLayout(false);
            groupTempo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericTempo).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericCountIn).EndInit();
            groupTrainer.ResumeLayout(false);
            groupTrainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericTrainerStep).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericTrainerBars).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericTrainerTarget).EndInit();
            groupSwing.ResumeLayout(false);
            groupSwing.PerformLayout();
            groupDrumkit.ResumeLayout(false);
            groupPattern.ResumeLayout(false);
            groupSequence.ResumeLayout(false);
            groupSequence.PerformLayout();
            tabPageSettings.ResumeLayout(false);
            flowSettings.ResumeLayout(false);
            groupProjects.ResumeLayout(false);
            groupProjects.PerformLayout();
            groupAppearance.ResumeLayout(false);
            groupAppearance.PerformLayout();
            groupLanguage.ResumeLayout(false);
            groupLanguage.PerformLayout();
            groupKits.ResumeLayout(false);
            groupKits.PerformLayout();
            groupAudio.ResumeLayout(false);
            groupAudio.PerformLayout();
            groupPlayback.ResumeLayout(false);
            groupPlayback.PerformLayout();
            groupFlash.ResumeLayout(false);
            groupFlash.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericClickVolume).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericLatency).EndInit();
            panelFileCommands.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            trackNameMenu.ResumeLayout(false);
            demosMenu.ResumeLayout(false);
            patternMenu.ResumeLayout(false);
            beatMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl ribbonTabs;
        private TabPage tabPageFile;
        private ImageList ribbonImages;
        private FlowLayoutPanel flowFile;
        private Panel panelFileCommands;
        private Panel tabMarker;
        private Panel lineFileCommands;
        private Button buttonNew;
        private Button buttonSave;
        private Button buttonSaveAs;
        private Panel groupFind;
        private Label labelSearch;
        private TextBox textBoxSearch;
        private Label labelFilterKit;
        private ComboBox comboFilterKit;
        private Label labelFilterMeter;
        private ComboBox comboFilterMeter;
        private Label captionFind;
        private Panel dividerFind;
        private Panel dividerStartup;
        private Button buttonExportAudio;
        private Panel lineCommands;
        private Button buttonCloseProject;
        private Panel lineWindowCommands;
        private Button buttonResetLayout;
        private Button buttonFileAbout;
        private Button buttonExit;
        private Button buttonOtherFolder;
        private Button buttonDefaultFolder;
        private Button buttonDemos;
        private Button buttonImportMidi;
        private TabPage tabPageStart;
        private FlowLayoutPanel flowStart;
        private Panel groupPlay;
        private Button buttonPlay;
        private Label captionPlay;
        private Panel dividerPlay;
        private Panel groupTempo;
        private Label labelTempo;
        private NumericUpDown numericTempo;
        private Label labelBpm;
        private Button buttonTap;
        private Label labelCountIn;
        private NumericUpDown numericCountIn;
        private Label labelCountInBars;
        private Label captionTempo;
        private Panel dividerTempo;
        private Panel groupTrainer;
        private CheckBox checkBoxTrainer;
        private Label labelTrainerStepBpm;
        private NumericUpDown numericTrainerStep;
        private Label labelTrainerBars;
        private Label labelTrainerEvery;
        private NumericUpDown numericTrainerBars;
        private Label labelTrainerTarget;
        private NumericUpDown numericTrainerTarget;
        private Label labelTrainerBpm;
        private Label captionTrainer;
        private Panel dividerTrainer;
        private Panel groupSwing;
        private RadioButton radioSwingStraight;
        private RadioButton radioSwingEighths;
        private RadioButton radioSwingSixteenths;
        private Label captionSwing;
        private Panel dividerSwing;
        private Panel groupDrumkit;
        private ComboBox comboKit;
        private Label captionDrumkit;
        private Panel dividerDrumkit;
        private Panel groupPattern;
        private RadioButton radioPattern1;
        private RadioButton radioPattern2;
        private RadioButton radioPattern3;
        private RadioButton radioPattern4;
        private ComboBox comboMeter;
        private Button buttonClearPattern;
        private Label captionPattern;
        private Panel dividerPattern;
        private Panel dividerSequence;
        private Panel groupSequence;
        private CheckBox checkBoxSequence;
        private CheckBox checkBoxRepeat;
        private TextBox textBoxSequence;
        private Label captionSequence;
        private TabPage tabPageSettings;
        private FlowLayoutPanel flowSettings;
        private Panel groupProjects;
        private Label labelProjectFolder;
        private TextBox textBoxProjectFolder;
        private Button buttonProjectFolder;
        private Label labelProjectHint;
        private Panel groupStartup;
        private CheckBox checkBoxOpenLast;
        private Label captionStartup;
        private Label captionProjects;
        private Panel dividerProjects;
        private Panel groupAppearance;
        private RadioButton radioSystem;
        private RadioButton radioLight;
        private RadioButton radioDark;
        private CheckBox checkBoxFlash;
        private Label captionAppearance;
        private Panel dividerAppearance;
        private Panel groupLanguage;
        private RadioButton radioGerman;
        private RadioButton radioEnglish;
        private Label captionLanguage;
        private Panel dividerLanguage;
        private Panel groupKits;
        private Label labelOwnKits;
        private Button buttonOwnKits;
        private Label labelInstalledKits;
        private Button buttonInstalledKits;
        private Panel lineKitFolders;
        private Button buttonReloadKits;
        private Button buttonKitGuide;
        private Button buttonSelectKits;
        private Label captionKits;
        private Panel dividerKits;
        private Panel dividerAudio;
        private Panel groupPlayback;
        private Label labelClickVolume;
        private Button buttonKeepScreenOn;
        private Label captionPlayback;
        private Panel dividerPlayback;
        private Panel groupFlash;
        private Label captionFlash;
        private Panel dividerFlash;
        private NumericUpDown numericClickVolume;
        private Label labelClickVolumeUnit;
        private Panel lineLatency;
        private Label labelLatency;
        private NumericUpDown numericLatency;
        private Label labelLatencyUnit;
        private Button buttonLatencyDefault;
        private Button buttonTopMost;
        private Panel groupAudio;
        private Label labelAudioDevice;
        private ComboBox comboAudioDevice;
        private Label captionAudio;
        private Button buttonUndo;
        private Button buttonRedo;
        private Button buttonQuickSave;
        private Button buttonShortcuts;
        private Button buttonAbout;
        private System.Windows.Forms.Timer previewTimer;
        private System.Windows.Forms.Timer noticeTimer;
        private System.Windows.Forms.Timer restartTimer;
        private ToolTip toolTip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusHint;
        private ToolStripStatusLabel statusProjects;
        private ToolStripStatusLabel statusTrainer;
        private ToolStripStatusLabel statusBars;
        private ToolStripStatusLabel statusTime;
        private ContextMenuStrip trackNameMenu;
        private ToolStripMenuItem previewTrackToolStripMenuItem;
        private ToolStripMenuItem renameTrackToolStripMenuItem;
        private ContextMenuStrip demosMenu;
        private ToolStripMenuItem noDemosToolStripMenuItem;
        private ContextMenuStrip patternMenu;
        private ToolStripMenuItem copyPatternToolStripMenuItem;
        private ToolStripMenuItem pastePatternToolStripMenuItem;
        private ToolStripMenuItem clearPatternToolStripMenuItem;
        private ContextMenuStrip beatMenu;
        private ToolStripMenuItem copyBeatToolStripMenuItem;
        private ToolStripMenuItem pasteBeatToolStripMenuItem;
        private ToolStripMenuItem fillBeatsToolStripMenuItem;
        private ToolStripMenuItem clearBeatToolStripMenuItem;
        private Drums4me.Controls.StepGrid stepGrid;
        private Drums4me.Controls.ProjectBrowser projectBrowser;
        private System.Windows.Forms.Timer playheadTimer;
        private System.Windows.Forms.Timer pendingOpenTimer;
        private FolderBrowserDialog otherFolderDialog;
        private FolderBrowserDialog projectFolderDialog;
        private SaveFileDialog saveFileDialog;
        private OpenFileDialog midiFileDialog;
    }
}
