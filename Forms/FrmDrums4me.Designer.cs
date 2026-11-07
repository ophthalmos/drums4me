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
            menuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            demosToolStripMenuItem = new ToolStripMenuItem();
            noDemosToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            fileSeparator = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            undoToolStripMenuItem = new ToolStripMenuItem();
            redoToolStripMenuItem = new ToolStripMenuItem();
            editSeparator = new ToolStripSeparator();
            clearPatternToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            projectFolderToolStripMenuItem = new ToolStripMenuItem();
            appearanceToolStripMenuItem = new ToolStripMenuItem();
            appearanceSystemToolStripMenuItem = new ToolStripMenuItem();
            appearanceLightToolStripMenuItem = new ToolStripMenuItem();
            appearanceDarkToolStripMenuItem = new ToolStripMenuItem();
            settingsSeparator = new ToolStripSeparator();
            ownKitsToolStripMenuItem = new ToolStripMenuItem();
            installedKitsToolStripMenuItem = new ToolStripMenuItem();
            reloadKitsToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            shortcutsToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            rowMain = new FlowLayoutPanel();
            buttonPlay = new Button();
            labelTempo = new Label();
            numericTempo = new NumericUpDown();
            labelBpm = new Label();
            buttonTap = new Button();
            dividerTempo = new Panel();
            labelSwing = new Label();
            trackBarSwing = new TrackBar();
            labelVolume = new Label();
            trackBarVolume = new TrackBar();
            labelKit = new Label();
            comboKit = new ComboBox();
            dividerKit = new Panel();
            labelCountIn = new Label();
            numericCountIn = new NumericUpDown();
            checkBoxFlash = new CheckBox();
            lineSequence = new Panel();
            rowSequence = new FlowLayoutPanel();
            checkBoxSequence = new CheckBox();
            textBoxSequence = new TextBox();
            checkBoxRepeat = new CheckBox();
            dividerPattern = new Panel();
            labelPattern = new Label();
            radioPattern1 = new RadioButton();
            radioPattern2 = new RadioButton();
            radioPattern3 = new RadioButton();
            radioPattern4 = new RadioButton();
            labelMeter = new Label();
            comboMeter = new ComboBox();
            lineTrainer = new Panel();
            rowTrainer = new FlowLayoutPanel();
            checkBoxTrainer = new CheckBox();
            labelTrainerStep = new Label();
            numericTrainerStep = new NumericUpDown();
            labelTrainerBars = new Label();
            numericTrainerBars = new NumericUpDown();
            labelTrainerTarget = new Label();
            numericTrainerTarget = new NumericUpDown();
            labelTrainerBpm = new Label();
            labelTrainerStatus = new Label();
            stepGrid = new Drums4me.Controls.StepGrid();
            statusStrip = new StatusStrip();
            statusHint = new ToolStripStatusLabel();
            statusBars = new ToolStripStatusLabel();
            statusTime = new ToolStripStatusLabel();
            playheadTimer = new System.Windows.Forms.Timer(components);
            previewTimer = new System.Windows.Forms.Timer(components);
            trackNameMenu = new ContextMenuStrip(components);
            previewTrackToolStripMenuItem = new ToolStripMenuItem();
            renameTrackToolStripMenuItem = new ToolStripMenuItem();
            pendingOpenTimer = new System.Windows.Forms.Timer(components);
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            folderBrowserDialog = new FolderBrowserDialog();
            menuStrip.SuspendLayout();
            rowMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericTempo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarSwing).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVolume).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericCountIn).BeginInit();
            rowSequence.SuspendLayout();
            rowTrainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericTrainerStep).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericTrainerBars).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericTrainerTarget).BeginInit();
            statusStrip.SuspendLayout();
            trackNameMenu.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.Font = new Font("Segoe UI", 10F);
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, settingsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1260, 27);
            menuStrip.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, demosToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, fileSeparator, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(53, 23);
            fileToolStripMenuItem.Text = "&Datei";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Image = (Image)resources.GetObject("newToolStripMenuItem.Image");
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem.Size = new Size(332, 24);
            newToolStripMenuItem.Text = "&Neu";
            newToolStripMenuItem.Click += NewToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Image = (Image)resources.GetObject("openToolStripMenuItem.Image");
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.Size = new Size(332, 24);
            openToolStripMenuItem.Text = "Ö&ffnen…";
            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            // 
            // demosToolStripMenuItem
            // 
            demosToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { noDemosToolStripMenuItem });
            demosToolStripMenuItem.Image = (Image)resources.GetObject("demosToolStripMenuItem.Image");
            demosToolStripMenuItem.Name = "demosToolStripMenuItem";
            demosToolStripMenuItem.Size = new Size(332, 24);
            demosToolStripMenuItem.Text = "De&mos";
            demosToolStripMenuItem.DropDownOpening += DemosToolStripMenuItem_DropDownOpening;
            // 
            // noDemosToolStripMenuItem
            // 
            noDemosToolStripMenuItem.Enabled = false;
            noDemosToolStripMenuItem.Name = "noDemosToolStripMenuItem";
            noDemosToolStripMenuItem.Size = new Size(217, 24);
            noDemosToolStripMenuItem.Text = "Keine Demos installiert";
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Image = (Image)resources.GetObject("saveToolStripMenuItem.Image");
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(332, 24);
            saveToolStripMenuItem.Text = "&Speichern";
            saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Image = (Image)resources.GetObject("saveAsToolStripMenuItem.Image");
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            saveAsToolStripMenuItem.Size = new Size(332, 24);
            saveAsToolStripMenuItem.Text = "Speichern &unter…";
            saveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;
            // 
            // fileSeparator
            // 
            fileSeparator.Name = "fileSeparator";
            fileSeparator.Size = new Size(329, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Image = (Image)resources.GetObject("exitToolStripMenuItem.Image");
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(332, 24);
            exitToolStripMenuItem.Text = "&Beenden";
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem, editSeparator, clearPatternToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(86, 23);
            editToolStripMenuItem.Text = "&Bearbeiten";
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Enabled = false;
            undoToolStripMenuItem.Image = (Image)resources.GetObject("undoToolStripMenuItem.Image");
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            undoToolStripMenuItem.Size = new Size(207, 24);
            undoToolStripMenuItem.Text = "&Rückgängig";
            undoToolStripMenuItem.Click += UndoToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Image = (Image)resources.GetObject("redoToolStripMenuItem.Image");
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            redoToolStripMenuItem.Size = new Size(207, 24);
            redoToolStripMenuItem.Text = "&Wiederholen";
            redoToolStripMenuItem.Click += RedoToolStripMenuItem_Click;
            // 
            // editSeparator
            // 
            editSeparator.Name = "editSeparator";
            editSeparator.Size = new Size(204, 6);
            // 
            // clearPatternToolStripMenuItem
            // 
            clearPatternToolStripMenuItem.Image = (Image)resources.GetObject("clearPatternToolStripMenuItem.Image");
            clearPatternToolStripMenuItem.Name = "clearPatternToolStripMenuItem";
            clearPatternToolStripMenuItem.Size = new Size(207, 24);
            clearPatternToolStripMenuItem.Text = "Pattern &leeren";
            clearPatternToolStripMenuItem.Click += ClearPatternToolStripMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { projectFolderToolStripMenuItem, appearanceToolStripMenuItem, settingsSeparator, ownKitsToolStripMenuItem, installedKitsToolStripMenuItem, reloadKitsToolStripMenuItem });
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(102, 23);
            settingsToolStripMenuItem.Text = "&Einstellungen";
            // 
            // projectFolderToolStripMenuItem
            // 
            projectFolderToolStripMenuItem.Image = (Image)resources.GetObject("projectFolderToolStripMenuItem.Image");
            projectFolderToolStripMenuItem.Name = "projectFolderToolStripMenuItem";
            projectFolderToolStripMenuItem.Size = new Size(232, 24);
            projectFolderToolStripMenuItem.Text = "&Speicherort für Projekte…";
            projectFolderToolStripMenuItem.Click += ProjectFolderToolStripMenuItem_Click;
            // 
            // appearanceToolStripMenuItem
            //
            appearanceToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { appearanceSystemToolStripMenuItem, appearanceLightToolStripMenuItem, appearanceDarkToolStripMenuItem });
            appearanceToolStripMenuItem.Image = (Image)resources.GetObject("appearanceToolStripMenuItem.Image");
            appearanceToolStripMenuItem.Name = "appearanceToolStripMenuItem";
            appearanceToolStripMenuItem.Size = new Size(232, 24);
            appearanceToolStripMenuItem.Text = "&Darstellung";
            appearanceToolStripMenuItem.DropDownOpening += AppearanceToolStripMenuItem_DropDownOpening;
            //
            // appearanceSystemToolStripMenuItem
            //
            appearanceSystemToolStripMenuItem.Name = "appearanceSystemToolStripMenuItem";
            appearanceSystemToolStripMenuItem.Size = new Size(160, 24);
            appearanceSystemToolStripMenuItem.Text = "Wie &Windows";
            appearanceSystemToolStripMenuItem.Click += AppearanceSystemToolStripMenuItem_Click;
            //
            // appearanceLightToolStripMenuItem
            //
            appearanceLightToolStripMenuItem.Name = "appearanceLightToolStripMenuItem";
            appearanceLightToolStripMenuItem.Size = new Size(160, 24);
            appearanceLightToolStripMenuItem.Text = "&Hell";
            appearanceLightToolStripMenuItem.Click += AppearanceLightToolStripMenuItem_Click;
            //
            // appearanceDarkToolStripMenuItem
            //
            appearanceDarkToolStripMenuItem.Name = "appearanceDarkToolStripMenuItem";
            appearanceDarkToolStripMenuItem.Size = new Size(160, 24);
            appearanceDarkToolStripMenuItem.Text = "&Dunkel";
            appearanceDarkToolStripMenuItem.Click += AppearanceDarkToolStripMenuItem_Click;
            //
            // settingsSeparator
            // 
            settingsSeparator.Name = "settingsSeparator";
            settingsSeparator.Size = new Size(229, 6);
            // 
            // ownKitsToolStripMenuItem
            // 
            ownKitsToolStripMenuItem.Image = (Image)resources.GetObject("ownKitsToolStripMenuItem.Image");
            ownKitsToolStripMenuItem.Name = "ownKitsToolStripMenuItem";
            ownKitsToolStripMenuItem.Size = new Size(232, 24);
            ownKitsToolStripMenuItem.Text = "Ei&gene Kits öffnen";
            ownKitsToolStripMenuItem.Click += OwnKitsToolStripMenuItem_Click;
            // 
            // installedKitsToolStripMenuItem
            // 
            installedKitsToolStripMenuItem.Image = (Image)resources.GetObject("installedKitsToolStripMenuItem.Image");
            installedKitsToolStripMenuItem.Name = "installedKitsToolStripMenuItem";
            installedKitsToolStripMenuItem.Size = new Size(232, 24);
            installedKitsToolStripMenuItem.Text = "&Installierte Kits öffnen";
            installedKitsToolStripMenuItem.Click += InstalledKitsToolStripMenuItem_Click;
            // 
            // reloadKitsToolStripMenuItem
            // 
            reloadKitsToolStripMenuItem.Image = (Image)resources.GetObject("reloadKitsToolStripMenuItem.Image");
            reloadKitsToolStripMenuItem.Name = "reloadKitsToolStripMenuItem";
            reloadKitsToolStripMenuItem.Size = new Size(232, 24);
            reloadKitsToolStripMenuItem.Text = "Kits neu &einlesen";
            reloadKitsToolStripMenuItem.Click += ReloadKitsToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { shortcutsToolStripMenuItem, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(48, 23);
            helpToolStripMenuItem.Text = "&Hilfe";
            // 
            // shortcutsToolStripMenuItem
            //
            shortcutsToolStripMenuItem.Image = (Image)resources.GetObject("shortcutsToolStripMenuItem.Image");
            shortcutsToolStripMenuItem.Name = "shortcutsToolStripMenuItem";
            shortcutsToolStripMenuItem.ShortcutKeys = Keys.F1;
            shortcutsToolStripMenuItem.Size = new Size(212, 24);
            shortcutsToolStripMenuItem.Text = "&Tastenkombinationen…";
            shortcutsToolStripMenuItem.Click += ShortcutsToolStripMenuItem_Click;
            //
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Image = (Image)resources.GetObject("aboutToolStripMenuItem.Image");
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(212, 24);
            aboutToolStripMenuItem.Text = "Ü&ber drums4me…";
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // rowMain
            // 
            rowMain.AutoSize = true;
            rowMain.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            rowMain.Controls.Add(buttonPlay);
            rowMain.Controls.Add(labelTempo);
            rowMain.Controls.Add(numericTempo);
            rowMain.Controls.Add(labelBpm);
            rowMain.Controls.Add(buttonTap);
            rowMain.Controls.Add(dividerTempo);
            rowMain.Controls.Add(labelSwing);
            rowMain.Controls.Add(trackBarSwing);
            rowMain.Controls.Add(labelVolume);
            rowMain.Controls.Add(trackBarVolume);
            rowMain.Controls.Add(labelKit);
            rowMain.Controls.Add(comboKit);
            rowMain.Controls.Add(dividerKit);
            rowMain.Controls.Add(labelCountIn);
            rowMain.Controls.Add(numericCountIn);
            rowMain.Controls.Add(checkBoxFlash);
            rowMain.Dock = DockStyle.Top;
            rowMain.Location = new Point(0, 27);
            rowMain.Name = "rowMain";
            rowMain.Padding = new Padding(6, 3, 6, 3);
            rowMain.Size = new Size(1260, 44);
            rowMain.TabIndex = 1;
            // 
            // buttonPlay
            // 
            buttonPlay.AccessibleDescription = "Startet oder stoppt die Wiedergabe. Leertaste.";
            buttonPlay.Anchor = AnchorStyles.Left;
            buttonPlay.Location = new Point(9, 6);
            buttonPlay.Name = "buttonPlay";
            buttonPlay.Size = new Size(92, 32);
            buttonPlay.TabIndex = 0;
            buttonPlay.Text = "▶ Start";
            buttonPlay.UseVisualStyleBackColor = true;
            buttonPlay.Click += ButtonPlay_Click;
            // 
            // labelTempo
            // 
            labelTempo.AccessibleDescription = "Tempo in Viertelnoten pro Minute. Plus, Minus, Pfeile: 1 BPM. Strg+Plus, Strg+Minus, Bild auf, Bild ab: 5 BPM.";
            labelTempo.Anchor = AnchorStyles.Left;
            labelTempo.AutoSize = true;
            labelTempo.Location = new Point(114, 12);
            labelTempo.Margin = new Padding(10, 0, 0, 0);
            labelTempo.Name = "labelTempo";
            labelTempo.Size = new Size(50, 19);
            labelTempo.TabIndex = 1;
            labelTempo.Text = "Tempo";
            // 
            // numericTempo
            // 
            numericTempo.AccessibleDescription = "Tempo in Viertelnoten pro Minute. Plus, Minus, Pfeile: 1 BPM. Strg+Plus, Strg+Minus, Bild auf, Bild ab: 5 BPM.";
            numericTempo.Anchor = AnchorStyles.Left;
            numericTempo.Location = new Point(167, 9);
            numericTempo.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            numericTempo.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            numericTempo.Name = "numericTempo";
            numericTempo.Size = new Size(62, 25);
            numericTempo.TabIndex = 2;
            numericTempo.TextAlign = HorizontalAlignment.Center;
            numericTempo.Value = new decimal(new int[] { 80, 0, 0, 0 });
            numericTempo.ValueChanged += NumericTempo_ValueChanged;
            // 
            // labelBpm
            // 
            labelBpm.AccessibleDescription = "Tempo in Viertelnoten pro Minute. Plus, Minus, Pfeile: 1 BPM. Strg+Plus, Strg+Minus, Bild auf, Bild ab: 5 BPM.";
            labelBpm.Anchor = AnchorStyles.Left;
            labelBpm.AutoSize = true;
            labelBpm.Location = new Point(232, 12);
            labelBpm.Margin = new Padding(0);
            labelBpm.Name = "labelBpm";
            labelBpm.Size = new Size(38, 19);
            labelBpm.TabIndex = 3;
            labelBpm.Text = "BPM";
            // 
            // buttonTap
            // 
            buttonTap.AccessibleDescription = "Öffnet ein Fenster, in dem du das Tempo mit der Leertaste vorgibst.";
            buttonTap.Anchor = AnchorStyles.Left;
            buttonTap.Location = new Point(273, 6);
            buttonTap.Name = "buttonTap";
            buttonTap.Size = new Size(58, 32);
            buttonTap.TabIndex = 4;
            buttonTap.Text = "Tap…";
            buttonTap.UseVisualStyleBackColor = true;
            buttonTap.Click += ButtonTap_Click;
            // 
            // dividerTempo
            // 
            dividerTempo.Anchor = AnchorStyles.Left;
            dividerTempo.BackColor = SystemColors.ControlDark;
            dividerTempo.Location = new Point(338, 9);
            dividerTempo.Margin = new Padding(4, 0, 4, 0);
            dividerTempo.Name = "dividerTempo";
            dividerTempo.Size = new Size(1, 26);
            dividerTempo.TabIndex = 5;
            // 
            // labelSwing
            // 
            labelSwing.AccessibleDescription = "Verzögert jeden zweiten Schritt und gibt dem Rhythmus so mehr Schwung.";
            labelSwing.Anchor = AnchorStyles.Left;
            labelSwing.Location = new Point(347, 12);
            labelSwing.Margin = new Padding(4, 0, 0, 0);
            labelSwing.Name = "labelSwing";
            labelSwing.Size = new Size(86, 19);
            labelSwing.TabIndex = 5;
            labelSwing.Text = "Swing 0 %";
            labelSwing.TextAlign = ContentAlignment.MiddleRight;
            // 
            // trackBarSwing
            // 
            trackBarSwing.AccessibleDescription = "Verzögert jeden zweiten Schritt und gibt dem Rhythmus so mehr Schwung.";
            trackBarSwing.Anchor = AnchorStyles.Left;
            trackBarSwing.AutoSize = false;
            trackBarSwing.Location = new Point(436, 7);
            trackBarSwing.Maximum = 100;
            trackBarSwing.Name = "trackBarSwing";
            trackBarSwing.Size = new Size(100, 30);
            trackBarSwing.TabIndex = 6;
            trackBarSwing.TickStyle = TickStyle.None;
            trackBarSwing.ValueChanged += TrackBarSwing_ValueChanged;
            // 
            // labelVolume
            // 
            labelVolume.AccessibleDescription = "Gesamtlautstärke aller Spuren.";
            labelVolume.Anchor = AnchorStyles.Left;
            labelVolume.Location = new Point(539, 12);
            labelVolume.Margin = new Padding(0);
            labelVolume.Name = "labelVolume";
            labelVolume.Size = new Size(118, 19);
            labelVolume.TabIndex = 7;
            labelVolume.Text = "Lautstärke 80 %";
            labelVolume.TextAlign = ContentAlignment.MiddleRight;
            // 
            // trackBarVolume
            // 
            trackBarVolume.AccessibleDescription = "Gesamtlautstärke aller Spuren.";
            trackBarVolume.Anchor = AnchorStyles.Left;
            trackBarVolume.AutoSize = false;
            trackBarVolume.Location = new Point(660, 7);
            trackBarVolume.Maximum = 100;
            trackBarVolume.Name = "trackBarVolume";
            trackBarVolume.Size = new Size(100, 30);
            trackBarVolume.TabIndex = 8;
            trackBarVolume.TickStyle = TickStyle.None;
            trackBarVolume.Value = 80;
            trackBarVolume.ValueChanged += TrackBarVolume_ValueChanged;
            // 
            // labelKit
            // 
            labelKit.AccessibleDescription = "Die Klänge dieses Projekts. In der Liste steht der Ordner des Kits in Klammern.";
            labelKit.Anchor = AnchorStyles.Left;
            labelKit.AutoSize = true;
            labelKit.Location = new Point(767, 12);
            labelKit.Margin = new Padding(4, 0, 0, 0);
            labelKit.Name = "labelKit";
            labelKit.Size = new Size(25, 19);
            labelKit.TabIndex = 9;
            labelKit.Text = "Kit";
            // 
            // comboKit
            // 
            comboKit.AccessibleDescription = "Die Klänge dieses Projekts. In der Liste steht der Ordner des Kits in Klammern.";
            comboKit.Anchor = AnchorStyles.Left;
            comboKit.DrawMode = DrawMode.OwnerDrawFixed;
            comboKit.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKit.DropDownWidth = 200;
            comboKit.ItemHeight = 19;
            comboKit.Location = new Point(795, 9);
            comboKit.MaxDropDownItems = 30;
            comboKit.Name = "comboKit";
            comboKit.Size = new Size(120, 25);
            comboKit.TabIndex = 10;
            comboKit.DrawItem += ComboKit_DrawItem;
            comboKit.SelectedIndexChanged += ComboKit_SelectedIndexChanged;
            // 
            // dividerKit
            // 
            dividerKit.Anchor = AnchorStyles.Left;
            dividerKit.BackColor = SystemColors.ControlDark;
            dividerKit.Location = new Point(922, 9);
            dividerKit.Margin = new Padding(4, 0, 4, 0);
            dividerKit.Name = "dividerKit";
            dividerKit.Size = new Size(1, 26);
            dividerKit.TabIndex = 11;
            // 
            // labelCountIn
            // 
            labelCountIn.AccessibleDescription = "So viele Takte zählt ein Metronom vor, bevor die Patterns einsetzen. 0: kein Vorzählen.";
            labelCountIn.Anchor = AnchorStyles.Left;
            labelCountIn.Location = new Point(931, 12);
            labelCountIn.Margin = new Padding(4, 0, 0, 0);
            labelCountIn.Name = "labelCountIn";
            labelCountIn.Size = new Size(72, 19);
            labelCountIn.TabIndex = 11;
            labelCountIn.Text = "Vorzählen";
            labelCountIn.TextAlign = ContentAlignment.MiddleRight;
            // 
            // numericCountIn
            // 
            numericCountIn.AccessibleDescription = "So viele Takte zählt ein Metronom vor, bevor die Patterns einsetzen. 0: kein Vorzählen.";
            numericCountIn.Anchor = AnchorStyles.Left;
            numericCountIn.Location = new Point(1006, 9);
            numericCountIn.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
            numericCountIn.Name = "numericCountIn";
            numericCountIn.Size = new Size(44, 25);
            numericCountIn.TabIndex = 12;
            numericCountIn.TextAlign = HorizontalAlignment.Center;
            numericCountIn.ValueChanged += NumericCountIn_ValueChanged;
            // 
            // checkBoxFlash
            // 
            checkBoxFlash.AccessibleDescription = "Lässt das Raster bei jedem Taktanfang kurz aufleuchten.";
            checkBoxFlash.Anchor = AnchorStyles.Left;
            checkBoxFlash.Appearance = Appearance.ToggleSwitch;
            checkBoxFlash.AutoSize = true;
            checkBoxFlash.Location = new Point(1065, 12);
            checkBoxFlash.Margin = new Padding(12, 3, 3, 3);
            checkBoxFlash.Name = "checkBoxFlash";
            checkBoxFlash.Size = new Size(175, 19);
            checkBoxFlash.TabIndex = 13;
            checkBoxFlash.Text = "Taktwechsel anzeigen";
            checkBoxFlash.UseVisualStyleBackColor = true;
            checkBoxFlash.VisualStylesMode = VisualStylesMode.Net11;
            checkBoxFlash.CheckedChanged += CheckBoxFlash_CheckedChanged;
            // 
            // lineSequence
            // 
            lineSequence.BackColor = SystemColors.ControlDark;
            lineSequence.Dock = DockStyle.Top;
            lineSequence.Location = new Point(0, 71);
            lineSequence.Name = "lineSequence";
            lineSequence.Size = new Size(1260, 1);
            lineSequence.TabIndex = 2;
            // 
            // rowSequence
            // 
            rowSequence.AutoSize = true;
            rowSequence.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            rowSequence.Controls.Add(checkBoxSequence);
            rowSequence.Controls.Add(textBoxSequence);
            rowSequence.Controls.Add(checkBoxRepeat);
            rowSequence.Controls.Add(dividerPattern);
            rowSequence.Controls.Add(labelPattern);
            rowSequence.Controls.Add(radioPattern1);
            rowSequence.Controls.Add(radioPattern2);
            rowSequence.Controls.Add(radioPattern3);
            rowSequence.Controls.Add(radioPattern4);
            rowSequence.Controls.Add(labelMeter);
            rowSequence.Controls.Add(comboMeter);
            rowSequence.Dock = DockStyle.Top;
            rowSequence.Location = new Point(0, 72);
            rowSequence.Name = "rowSequence";
            rowSequence.Padding = new Padding(6, 3, 6, 3);
            rowSequence.Size = new Size(1260, 44);
            rowSequence.TabIndex = 3;
            // 
            // checkBoxSequence
            // 
            checkBoxSequence.AccessibleDescription = "Spielt die Patterns in fester Reihenfolge: eine Ziffer je Takt, zum Beispiel 1 1 1 2.";
            checkBoxSequence.Anchor = AnchorStyles.Left;
            checkBoxSequence.Appearance = Appearance.ToggleSwitch;
            checkBoxSequence.Location = new Point(12, 12);
            checkBoxSequence.Margin = new Padding(6, 3, 3, 3);
            checkBoxSequence.Name = "checkBoxSequence";
            checkBoxSequence.Size = new Size(134, 19);
            checkBoxSequence.TabIndex = 0;
            checkBoxSequence.Text = "Abfolge";
            checkBoxSequence.UseVisualStyleBackColor = true;
            checkBoxSequence.VisualStylesMode = VisualStylesMode.Net11;
            checkBoxSequence.CheckedChanged += CheckBoxSequence_CheckedChanged;
            // 
            // textBoxSequence
            // 
            textBoxSequence.AccessibleDescription = "Spielt die Patterns in fester Reihenfolge: eine Ziffer je Takt, zum Beispiel 1 1 1 2.";
            textBoxSequence.Anchor = AnchorStyles.Left;
            textBoxSequence.Enabled = false;
            textBoxSequence.Location = new Point(155, 9);
            textBoxSequence.Margin = new Padding(6, 3, 3, 3);
            textBoxSequence.Name = "textBoxSequence";
            textBoxSequence.Size = new Size(260, 25);
            textBoxSequence.TabIndex = 1;
            textBoxSequence.TextChanged += TextBoxSequence_TextChanged;
            textBoxSequence.Leave += TextBoxSequence_Leave;
            // 
            // checkBoxRepeat
            // 
            checkBoxRepeat.AccessibleDescription = "Beginnt die Abfolge am Ende von vorn. Sonst stoppt die Wiedergabe nach dem letzten Takt.";
            checkBoxRepeat.Anchor = AnchorStyles.Left;
            checkBoxRepeat.Appearance = Appearance.ToggleSwitch;
            checkBoxRepeat.AutoSize = true;
            checkBoxRepeat.Checked = true;
            checkBoxRepeat.CheckState = CheckState.Checked;
            checkBoxRepeat.Enabled = false;
            checkBoxRepeat.Location = new Point(424, 12);
            checkBoxRepeat.Margin = new Padding(6, 3, 3, 3);
            checkBoxRepeat.Name = "checkBoxRepeat";
            checkBoxRepeat.Size = new Size(123, 19);
            checkBoxRepeat.TabIndex = 2;
            checkBoxRepeat.Text = "Wiederholen";
            checkBoxRepeat.UseVisualStyleBackColor = true;
            checkBoxRepeat.VisualStylesMode = VisualStylesMode.Net11;
            checkBoxRepeat.CheckedChanged += CheckBoxRepeat_CheckedChanged;
            // 
            // dividerPattern
            // 
            dividerPattern.Anchor = AnchorStyles.Left;
            dividerPattern.BackColor = SystemColors.ControlDark;
            dividerPattern.Location = new Point(560, 9);
            dividerPattern.Margin = new Padding(10, 0, 10, 0);
            dividerPattern.Name = "dividerPattern";
            dividerPattern.Size = new Size(1, 26);
            dividerPattern.TabIndex = 4;
            // 
            // labelPattern
            // 
            labelPattern.AccessibleDescription = "Wählt das Pattern, das du siehst und hörst. Beim ersten Aufruf übernimmt es den Inhalt des aktuellen Patterns.";
            labelPattern.Anchor = AnchorStyles.Left;
            labelPattern.AutoSize = true;
            labelPattern.Location = new Point(577, 12);
            labelPattern.Margin = new Padding(6, 0, 0, 0);
            labelPattern.Name = "labelPattern";
            labelPattern.Size = new Size(54, 19);
            labelPattern.TabIndex = 4;
            labelPattern.Text = "Pattern";
            // 
            // radioPattern1
            // 
            radioPattern1.AccessibleDescription = "Wählt das Pattern, das du siehst und hörst. Beim ersten Aufruf übernimmt es den Inhalt des aktuellen Patterns.";
            radioPattern1.Anchor = AnchorStyles.Left;
            radioPattern1.Appearance = Appearance.Button;
            radioPattern1.Checked = true;
            radioPattern1.Location = new Point(634, 6);
            radioPattern1.Margin = new Padding(3, 3, 0, 3);
            radioPattern1.Name = "radioPattern1";
            radioPattern1.Size = new Size(34, 32);
            radioPattern1.TabIndex = 5;
            radioPattern1.TabStop = true;
            radioPattern1.Text = "1";
            radioPattern1.TextAlign = ContentAlignment.MiddleCenter;
            radioPattern1.UseVisualStyleBackColor = true;
            radioPattern1.CheckedChanged += RadioPattern_CheckedChanged;
            // 
            // radioPattern2
            // 
            radioPattern2.AccessibleDescription = "Wählt das Pattern, das du siehst und hörst. Beim ersten Aufruf übernimmt es den Inhalt des aktuellen Patterns.";
            radioPattern2.Anchor = AnchorStyles.Left;
            radioPattern2.Appearance = Appearance.Button;
            radioPattern2.Location = new Point(668, 6);
            radioPattern2.Margin = new Padding(0, 3, 0, 3);
            radioPattern2.Name = "radioPattern2";
            radioPattern2.Size = new Size(34, 32);
            radioPattern2.TabIndex = 6;
            radioPattern2.Text = "2";
            radioPattern2.TextAlign = ContentAlignment.MiddleCenter;
            radioPattern2.UseVisualStyleBackColor = true;
            radioPattern2.CheckedChanged += RadioPattern_CheckedChanged;
            // 
            // radioPattern3
            // 
            radioPattern3.AccessibleDescription = "Wählt das Pattern, das du siehst und hörst. Beim ersten Aufruf übernimmt es den Inhalt des aktuellen Patterns.";
            radioPattern3.Anchor = AnchorStyles.Left;
            radioPattern3.Appearance = Appearance.Button;
            radioPattern3.Location = new Point(702, 6);
            radioPattern3.Margin = new Padding(0, 3, 0, 3);
            radioPattern3.Name = "radioPattern3";
            radioPattern3.Size = new Size(34, 32);
            radioPattern3.TabIndex = 7;
            radioPattern3.Text = "3";
            radioPattern3.TextAlign = ContentAlignment.MiddleCenter;
            radioPattern3.UseVisualStyleBackColor = true;
            radioPattern3.CheckedChanged += RadioPattern_CheckedChanged;
            // 
            // radioPattern4
            // 
            radioPattern4.AccessibleDescription = "Wählt das Pattern, das du siehst und hörst. Beim ersten Aufruf übernimmt es den Inhalt des aktuellen Patterns.";
            radioPattern4.Anchor = AnchorStyles.Left;
            radioPattern4.Appearance = Appearance.Button;
            radioPattern4.Location = new Point(736, 6);
            radioPattern4.Margin = new Padding(0, 3, 0, 3);
            radioPattern4.Name = "radioPattern4";
            radioPattern4.Size = new Size(34, 32);
            radioPattern4.TabIndex = 8;
            radioPattern4.Text = "4";
            radioPattern4.TextAlign = ContentAlignment.MiddleCenter;
            radioPattern4.UseVisualStyleBackColor = true;
            radioPattern4.CheckedChanged += RadioPattern_CheckedChanged;
            // 
            // labelMeter
            // 
            labelMeter.AccessibleDescription = "Die Taktart des aktuellen Patterns.";
            labelMeter.Anchor = AnchorStyles.Left;
            labelMeter.AutoSize = true;
            labelMeter.Location = new Point(784, 12);
            labelMeter.Margin = new Padding(14, 0, 0, 0);
            labelMeter.Name = "labelMeter";
            labelMeter.Size = new Size(50, 19);
            labelMeter.TabIndex = 9;
            labelMeter.Text = "Taktart";
            // 
            // comboMeter
            // 
            comboMeter.AccessibleDescription = "Die Taktart des aktuellen Patterns.";
            comboMeter.Anchor = AnchorStyles.Left;
            comboMeter.DropDownStyle = ComboBoxStyle.DropDownList;
            comboMeter.Location = new Point(837, 10);
            comboMeter.Name = "comboMeter";
            comboMeter.Size = new Size(110, 25);
            comboMeter.TabIndex = 10;
            comboMeter.SelectedIndexChanged += ComboMeter_SelectedIndexChanged;
            // 
            // lineTrainer
            // 
            lineTrainer.BackColor = SystemColors.ControlDark;
            lineTrainer.Dock = DockStyle.Top;
            lineTrainer.Location = new Point(0, 116);
            lineTrainer.Name = "lineTrainer";
            lineTrainer.Size = new Size(1260, 1);
            lineTrainer.TabIndex = 4;
            // 
            // rowTrainer
            // 
            rowTrainer.AutoSize = true;
            rowTrainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            rowTrainer.Controls.Add(checkBoxTrainer);
            rowTrainer.Controls.Add(labelTrainerStep);
            rowTrainer.Controls.Add(numericTrainerStep);
            rowTrainer.Controls.Add(labelTrainerBars);
            rowTrainer.Controls.Add(numericTrainerBars);
            rowTrainer.Controls.Add(labelTrainerTarget);
            rowTrainer.Controls.Add(numericTrainerTarget);
            rowTrainer.Controls.Add(labelTrainerBpm);
            rowTrainer.Controls.Add(labelTrainerStatus);
            rowTrainer.Dock = DockStyle.Top;
            rowTrainer.Location = new Point(0, 117);
            rowTrainer.Name = "rowTrainer";
            rowTrainer.Padding = new Padding(6, 3, 6, 3);
            rowTrainer.Size = new Size(1260, 37);
            rowTrainer.TabIndex = 5;
            // 
            // checkBoxTrainer
            // 
            checkBoxTrainer.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            checkBoxTrainer.Anchor = AnchorStyles.Left;
            checkBoxTrainer.Appearance = Appearance.ToggleSwitch;
            checkBoxTrainer.Location = new Point(12, 9);
            checkBoxTrainer.Margin = new Padding(6, 3, 3, 3);
            checkBoxTrainer.Name = "checkBoxTrainer";
            checkBoxTrainer.Size = new Size(134, 19);
            checkBoxTrainer.TabIndex = 0;
            checkBoxTrainer.Text = "Tempotrainer";
            checkBoxTrainer.UseVisualStyleBackColor = true;
            checkBoxTrainer.VisualStylesMode = VisualStylesMode.Net11;
            checkBoxTrainer.CheckedChanged += CheckBoxTrainer_CheckedChanged;
            // 
            // labelTrainerStep
            // 
            labelTrainerStep.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            labelTrainerStep.Anchor = AnchorStyles.Left;
            labelTrainerStep.AutoSize = true;
            labelTrainerStep.Location = new Point(155, 9);
            labelTrainerStep.Margin = new Padding(6, 0, 0, 0);
            labelTrainerStep.Name = "labelTrainerStep";
            labelTrainerStep.Size = new Size(19, 19);
            labelTrainerStep.TabIndex = 1;
            labelTrainerStep.Text = "+";
            // 
            // numericTrainerStep
            // 
            numericTrainerStep.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            numericTrainerStep.Anchor = AnchorStyles.Left;
            numericTrainerStep.Enabled = false;
            numericTrainerStep.Location = new Point(177, 6);
            numericTrainerStep.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            numericTrainerStep.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericTrainerStep.Name = "numericTrainerStep";
            numericTrainerStep.Size = new Size(44, 25);
            numericTrainerStep.TabIndex = 2;
            numericTrainerStep.TextAlign = HorizontalAlignment.Center;
            numericTrainerStep.Value = new decimal(new int[] { 2, 0, 0, 0 });
            numericTrainerStep.ValueChanged += NumericTrainer_ValueChanged;
            // 
            // labelTrainerBars
            // 
            labelTrainerBars.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            labelTrainerBars.Anchor = AnchorStyles.Left;
            labelTrainerBars.AutoSize = true;
            labelTrainerBars.Location = new Point(224, 9);
            labelTrainerBars.Margin = new Padding(0);
            labelTrainerBars.Name = "labelTrainerBars";
            labelTrainerBars.Size = new Size(62, 19);
            labelTrainerBars.TabIndex = 3;
            labelTrainerBars.Text = "BPM alle";
            // 
            // numericTrainerBars
            // 
            numericTrainerBars.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            numericTrainerBars.Anchor = AnchorStyles.Left;
            numericTrainerBars.Enabled = false;
            numericTrainerBars.Location = new Point(289, 6);
            numericTrainerBars.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            numericTrainerBars.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericTrainerBars.Name = "numericTrainerBars";
            numericTrainerBars.Size = new Size(44, 25);
            numericTrainerBars.TabIndex = 4;
            numericTrainerBars.TextAlign = HorizontalAlignment.Center;
            numericTrainerBars.Value = new decimal(new int[] { 4, 0, 0, 0 });
            numericTrainerBars.ValueChanged += NumericTrainer_ValueChanged;
            // 
            // labelTrainerTarget
            // 
            labelTrainerTarget.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            labelTrainerTarget.Anchor = AnchorStyles.Left;
            labelTrainerTarget.AutoSize = true;
            labelTrainerTarget.Location = new Point(336, 9);
            labelTrainerTarget.Margin = new Padding(0);
            labelTrainerTarget.Name = "labelTrainerTarget";
            labelTrainerTarget.Size = new Size(64, 19);
            labelTrainerTarget.TabIndex = 5;
            labelTrainerTarget.Text = "Takte, bis";
            // 
            // numericTrainerTarget
            // 
            numericTrainerTarget.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            numericTrainerTarget.Anchor = AnchorStyles.Left;
            numericTrainerTarget.Enabled = false;
            numericTrainerTarget.Location = new Point(403, 6);
            numericTrainerTarget.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            numericTrainerTarget.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            numericTrainerTarget.Name = "numericTrainerTarget";
            numericTrainerTarget.Size = new Size(62, 25);
            numericTrainerTarget.TabIndex = 6;
            numericTrainerTarget.TextAlign = HorizontalAlignment.Center;
            numericTrainerTarget.Value = new decimal(new int[] { 120, 0, 0, 0 });
            numericTrainerTarget.ValueChanged += NumericTrainer_ValueChanged;
            // 
            // labelTrainerBpm
            // 
            labelTrainerBpm.AccessibleDescription = "Steigert das Tempo beim Spielen schrittweise bis zum Ziel. So tastest du dich an schwierige Stellen heran.";
            labelTrainerBpm.Anchor = AnchorStyles.Left;
            labelTrainerBpm.AutoSize = true;
            labelTrainerBpm.Location = new Point(468, 9);
            labelTrainerBpm.Margin = new Padding(0);
            labelTrainerBpm.Name = "labelTrainerBpm";
            labelTrainerBpm.Size = new Size(38, 19);
            labelTrainerBpm.TabIndex = 7;
            labelTrainerBpm.Text = "BPM";
            // 
            // labelTrainerStatus
            // 
            labelTrainerStatus.Anchor = AnchorStyles.Left;
            labelTrainerStatus.AutoSize = true;
            labelTrainerStatus.Location = new Point(520, 9);
            labelTrainerStatus.Margin = new Padding(14, 0, 0, 0);
            labelTrainerStatus.Name = "labelTrainerStatus";
            labelTrainerStatus.Size = new Size(0, 19);
            labelTrainerStatus.TabIndex = 8;
            // 
            // stepGrid
            // 
            stepGrid.BackColor = SystemColors.Window;
            stepGrid.Dock = DockStyle.Fill;
            stepGrid.Location = new Point(0, 154);
            stepGrid.Name = "stepGrid";
            stepGrid.Size = new Size(1260, 476);
            stepGrid.TabIndex = 6;
            stepGrid.Edited += StepGrid_Edited;
            stepGrid.EditCompleted += StepGrid_EditCompleted;
            stepGrid.TrackNameClicked += StepGrid_TrackNameClicked;
            // 
            // statusStrip
            // 
            statusStrip.Font = new Font("Segoe UI", 10F);
            statusStrip.Items.AddRange(new ToolStripItem[] { statusHint, statusBars, statusTime });
            statusStrip.Location = new Point(0, 630);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1260, 24);
            statusStrip.TabIndex = 7;
            // 
            // statusHint
            // 
            statusHint.Name = "statusHint";
            statusHint.Size = new Size(1032, 19);
            statusHint.Spring = true;
            statusHint.TextAlign = ContentAlignment.MiddleLeft;
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
            // pendingOpenTimer
            // 
            pendingOpenTimer.Interval = 200;
            pendingOpenTimer.Tick += PendingOpenTimer_Tick;
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "drums4me-Projekte (*.d4m)|*.d4m|Alle Dateien (*.*)|*.*";
            openFileDialog.Title = "Projekt öffnen";
            // 
            // saveFileDialog
            // 
            saveFileDialog.DefaultExt = "d4m";
            saveFileDialog.Filter = "drums4me-Projekte (*.d4m)|*.d4m|Alle Dateien (*.*)|*.*";
            saveFileDialog.Title = "Projekt speichern";
            // 
            // folderBrowserDialog
            // 
            folderBrowserDialog.Description = "Wähle den Ordner für deine Projekte.";
            folderBrowserDialog.UseDescriptionForTitle = true;
            // 
            // FrmDrums4me
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1260, 654);
            Controls.Add(stepGrid);
            Controls.Add(statusStrip);
            Controls.Add(rowTrainer);
            Controls.Add(lineTrainer);
            Controls.Add(rowSequence);
            Controls.Add(lineSequence);
            Controls.Add(rowMain);
            Controls.Add(menuStrip);
            Font = new Font("Segoe UI", 10F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            MinimumSize = new Size(960, 574);
            Name = "FrmDrums4me";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "drums4me";
            FormClosing += FrmDrums4me_FormClosing;
            Load += FrmDrums4me_Load;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            rowMain.ResumeLayout(false);
            rowMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericTempo).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarSwing).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVolume).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericCountIn).EndInit();
            rowSequence.ResumeLayout(false);
            rowSequence.PerformLayout();
            rowTrainer.ResumeLayout(false);
            rowTrainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericTrainerStep).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericTrainerBars).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericTrainerTarget).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            trackNameMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem demosToolStripMenuItem;
        private ToolStripMenuItem noDemosToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripSeparator fileSeparator;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripSeparator editSeparator;
        private ToolStripMenuItem clearPatternToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem projectFolderToolStripMenuItem;
        private ToolStripMenuItem appearanceToolStripMenuItem;
        private ToolStripMenuItem appearanceSystemToolStripMenuItem;
        private ToolStripMenuItem appearanceLightToolStripMenuItem;
        private ToolStripMenuItem appearanceDarkToolStripMenuItem;
        private ToolStripSeparator settingsSeparator;
        private ToolStripMenuItem ownKitsToolStripMenuItem;
        private ToolStripMenuItem installedKitsToolStripMenuItem;
        private ToolStripMenuItem reloadKitsToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem shortcutsToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private FlowLayoutPanel rowMain;
        private Panel dividerTempo;
        private Panel dividerKit;
        private Panel lineSequence;
        private FlowLayoutPanel rowSequence;
        private Panel dividerPattern;
        private Panel lineTrainer;
        private FlowLayoutPanel rowTrainer;
        private Button buttonPlay;
        private Label labelTempo;
        private NumericUpDown numericTempo;
        private Label labelBpm;
        private Button buttonTap;
        private Label labelSwing;
        private TrackBar trackBarSwing;
        private Label labelVolume;
        private TrackBar trackBarVolume;
        private Label labelKit;
        private ComboBox comboKit;
        private Label labelPattern;
        private RadioButton radioPattern1;
        private RadioButton radioPattern2;
        private RadioButton radioPattern3;
        private RadioButton radioPattern4;
        private CheckBox checkBoxSequence;
        private TextBox textBoxSequence;
        private CheckBox checkBoxRepeat;
        private System.Windows.Forms.Timer previewTimer;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusHint;
        private ToolStripStatusLabel statusBars;
        private ToolStripStatusLabel statusTime;
        private ContextMenuStrip trackNameMenu;
        private ToolStripMenuItem previewTrackToolStripMenuItem;
        private ToolStripMenuItem renameTrackToolStripMenuItem;
        private CheckBox checkBoxTrainer;
        private Label labelTrainerStep;
        private NumericUpDown numericTrainerStep;
        private Label labelTrainerBars;
        private NumericUpDown numericTrainerBars;
        private Label labelTrainerTarget;
        private NumericUpDown numericTrainerTarget;
        private Label labelTrainerBpm;
        private Label labelTrainerStatus;
        private Label labelCountIn;
        private NumericUpDown numericCountIn;
        private CheckBox checkBoxFlash;
        private Drums4me.Controls.StepGrid stepGrid;
        private Label labelMeter;
        private ComboBox comboMeter;
        private System.Windows.Forms.Timer playheadTimer;
        private System.Windows.Forms.Timer pendingOpenTimer;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private FolderBrowserDialog folderBrowserDialog;
    }
}
