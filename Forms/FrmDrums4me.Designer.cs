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
            kitsToolStripMenuItem = new ToolStripMenuItem();
            openKitFolderToolStripMenuItem = new ToolStripMenuItem();
            reloadKitsToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            projectFolderToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            controlPanel = new FlowLayoutPanel();
            buttonPlay = new Button();
            labelTempo = new Label();
            numericTempo = new NumericUpDown();
            labelBpm = new Label();
            buttonTap = new Button();
            labelSwing = new Label();
            trackBarSwing = new TrackBar();
            labelVolume = new Label();
            trackBarVolume = new TrackBar();
            labelKit = new Label();
            comboKit = new ComboBox();
            labelCountIn = new Label();
            numericCountIn = new NumericUpDown();
            checkBoxFlash = new CheckBox();
            labelPattern = new Label();
            radioPattern1 = new RadioButton();
            radioPattern2 = new RadioButton();
            radioPattern3 = new RadioButton();
            radioPattern4 = new RadioButton();
            labelMeter = new Label();
            comboMeter = new ComboBox();
            checkBoxSequence = new CheckBox();
            textBoxSequence = new TextBox();
            checkBoxRepeat = new CheckBox();
            labelSequenceHint = new Label();
            stepGrid = new Drums4me.Controls.StepGrid();
            toolTip = new ToolTip(components);
            playheadTimer = new System.Windows.Forms.Timer(components);
            pendingOpenTimer = new System.Windows.Forms.Timer(components);
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            folderBrowserDialog = new FolderBrowserDialog();
            menuStrip.SuspendLayout();
            controlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericTempo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarSwing).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVolume).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericCountIn).BeginInit();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.Font = new Font("Segoe UI", 10F);
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, kitsToolStripMenuItem, settingsToolStripMenuItem, helpToolStripMenuItem });
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
            noDemosToolStripMenuItem.Size = new Size(240, 24);
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
            // kitsToolStripMenuItem
            // 
            kitsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openKitFolderToolStripMenuItem, reloadKitsToolStripMenuItem });
            kitsToolStripMenuItem.Name = "kitsToolStripMenuItem";
            kitsToolStripMenuItem.Size = new Size(43, 23);
            kitsToolStripMenuItem.Text = "&Kits";
            // 
            // openKitFolderToolStripMenuItem
            // 
            openKitFolderToolStripMenuItem.Image = (Image)resources.GetObject("openKitFolderToolStripMenuItem.Image");
            openKitFolderToolStripMenuItem.Name = "openKitFolderToolStripMenuItem";
            openKitFolderToolStripMenuItem.Size = new Size(187, 24);
            openKitFolderToolStripMenuItem.Text = "Kit-Ordner &öffnen";
            openKitFolderToolStripMenuItem.Click += OpenKitFolderToolStripMenuItem_Click;
            // 
            // reloadKitsToolStripMenuItem
            // 
            reloadKitsToolStripMenuItem.Image = (Image)resources.GetObject("reloadKitsToolStripMenuItem.Image");
            reloadKitsToolStripMenuItem.Name = "reloadKitsToolStripMenuItem";
            reloadKitsToolStripMenuItem.Size = new Size(187, 24);
            reloadKitsToolStripMenuItem.Text = "Kits neu &einlesen";
            reloadKitsToolStripMenuItem.Click += ReloadKitsToolStripMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { projectFolderToolStripMenuItem });
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
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(48, 23);
            helpToolStripMenuItem.Text = "&Hilfe";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Image = (Image)resources.GetObject("aboutToolStripMenuItem.Image");
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.ShortcutKeys = Keys.F1;
            aboutToolStripMenuItem.Size = new Size(212, 24);
            aboutToolStripMenuItem.Text = "Ü&ber drums4me…";
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // controlPanel
            // 
            controlPanel.AutoSize = true;
            controlPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            controlPanel.Controls.Add(buttonPlay);
            controlPanel.Controls.Add(labelTempo);
            controlPanel.Controls.Add(numericTempo);
            controlPanel.Controls.Add(labelBpm);
            controlPanel.Controls.Add(buttonTap);
            controlPanel.Controls.Add(labelSwing);
            controlPanel.Controls.Add(trackBarSwing);
            controlPanel.Controls.Add(labelVolume);
            controlPanel.Controls.Add(trackBarVolume);
            controlPanel.Controls.Add(labelKit);
            controlPanel.Controls.Add(comboKit);
            controlPanel.Controls.Add(labelCountIn);
            controlPanel.Controls.Add(numericCountIn);
            controlPanel.Controls.Add(checkBoxFlash);
            controlPanel.Controls.Add(labelPattern);
            controlPanel.Controls.Add(radioPattern1);
            controlPanel.Controls.Add(radioPattern2);
            controlPanel.Controls.Add(radioPattern3);
            controlPanel.Controls.Add(radioPattern4);
            controlPanel.Controls.Add(labelMeter);
            controlPanel.Controls.Add(comboMeter);
            controlPanel.Controls.Add(checkBoxSequence);
            controlPanel.Controls.Add(textBoxSequence);
            controlPanel.Controls.Add(checkBoxRepeat);
            controlPanel.Controls.Add(labelSequenceHint);
            controlPanel.Dock = DockStyle.Top;
            controlPanel.Location = new Point(0, 27);
            controlPanel.Name = "controlPanel";
            controlPanel.Padding = new Padding(6, 6, 6, 2);
            controlPanel.Size = new Size(1260, 84);
            controlPanel.TabIndex = 1;
            // 
            // buttonPlay
            // 
            buttonPlay.Anchor = AnchorStyles.Left;
            buttonPlay.Location = new Point(9, 9);
            buttonPlay.Name = "buttonPlay";
            buttonPlay.Size = new Size(92, 32);
            buttonPlay.TabIndex = 0;
            buttonPlay.Text = "▶ Start";
            buttonPlay.UseVisualStyleBackColor = true;
            buttonPlay.Click += ButtonPlay_Click;
            // 
            // labelTempo
            // 
            labelTempo.Anchor = AnchorStyles.Left;
            labelTempo.AutoSize = true;
            labelTempo.Location = new Point(114, 15);
            labelTempo.Margin = new Padding(10, 0, 0, 0);
            labelTempo.Name = "labelTempo";
            labelTempo.Size = new Size(50, 19);
            labelTempo.TabIndex = 1;
            labelTempo.Text = "Tempo";
            // 
            // numericTempo
            // 
            numericTempo.Anchor = AnchorStyles.Left;
            numericTempo.Location = new Point(167, 12);
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
            labelBpm.Anchor = AnchorStyles.Left;
            labelBpm.AutoSize = true;
            labelBpm.Location = new Point(232, 15);
            labelBpm.Margin = new Padding(0);
            labelBpm.Name = "labelBpm";
            labelBpm.Size = new Size(38, 19);
            labelBpm.TabIndex = 3;
            labelBpm.Text = "BPM";
            toolTip.SetToolTip(labelBpm, "Viertelnoten pro Minute, auch bei Achtel-Takten");
            // 
            // buttonTap
            // 
            buttonTap.Anchor = AnchorStyles.Left;
            buttonTap.Location = new Point(273, 9);
            buttonTap.Name = "buttonTap";
            buttonTap.Size = new Size(58, 32);
            buttonTap.TabIndex = 4;
            buttonTap.Text = "Tap…";
            buttonTap.UseVisualStyleBackColor = true;
            buttonTap.Click += ButtonTap_Click;
            // 
            // labelSwing
            // 
            labelSwing.Anchor = AnchorStyles.Left;
            labelSwing.Location = new Point(338, 15);
            labelSwing.Margin = new Padding(4, 0, 0, 0);
            labelSwing.Name = "labelSwing";
            labelSwing.Size = new Size(86, 19);
            labelSwing.TabIndex = 5;
            labelSwing.Text = "Swing 0 %";
            labelSwing.TextAlign = ContentAlignment.MiddleRight;
            // 
            // trackBarSwing
            // 
            trackBarSwing.Anchor = AnchorStyles.Left;
            trackBarSwing.AutoSize = false;
            trackBarSwing.Location = new Point(427, 10);
            trackBarSwing.Maximum = 100;
            trackBarSwing.Name = "trackBarSwing";
            trackBarSwing.Size = new Size(100, 30);
            trackBarSwing.TabIndex = 6;
            trackBarSwing.TickStyle = TickStyle.None;
            trackBarSwing.ValueChanged += TrackBarSwing_ValueChanged;
            // 
            // labelVolume
            // 
            labelVolume.Anchor = AnchorStyles.Left;
            labelVolume.Location = new Point(530, 15);
            labelVolume.Margin = new Padding(0);
            labelVolume.Name = "labelVolume";
            labelVolume.Size = new Size(118, 19);
            labelVolume.TabIndex = 7;
            labelVolume.Text = "Lautstärke 80 %";
            labelVolume.TextAlign = ContentAlignment.MiddleRight;
            // 
            // trackBarVolume
            // 
            trackBarVolume.Anchor = AnchorStyles.Left;
            trackBarVolume.AutoSize = false;
            trackBarVolume.Location = new Point(651, 10);
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
            labelKit.Anchor = AnchorStyles.Left;
            labelKit.AutoSize = true;
            labelKit.Location = new Point(758, 15);
            labelKit.Margin = new Padding(4, 0, 0, 0);
            labelKit.Name = "labelKit";
            labelKit.Size = new Size(25, 19);
            labelKit.TabIndex = 9;
            labelKit.Text = "Kit";
            // 
            // comboKit
            // 
            comboKit.Anchor = AnchorStyles.Left;
            comboKit.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKit.Location = new Point(786, 12);
            comboKit.MaxDropDownItems = 21;
            comboKit.Name = "comboKit";
            comboKit.Size = new Size(120, 25);
            comboKit.TabIndex = 10;
            comboKit.SelectedIndexChanged += ComboKit_SelectedIndexChanged;
            // 
            // labelCountIn
            // 
            labelCountIn.Anchor = AnchorStyles.Left;
            labelCountIn.Location = new Point(919, 15);
            labelCountIn.Margin = new Padding(10, 0, 0, 0);
            labelCountIn.Name = "labelCountIn";
            labelCountIn.Size = new Size(72, 19);
            labelCountIn.TabIndex = 11;
            labelCountIn.Text = "Vorzählen";
            labelCountIn.TextAlign = ContentAlignment.MiddleRight;
            // 
            // numericCountIn
            // 
            numericCountIn.Anchor = AnchorStyles.Left;
            numericCountIn.Location = new Point(994, 12);
            numericCountIn.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
            numericCountIn.Name = "numericCountIn";
            numericCountIn.Size = new Size(44, 25);
            numericCountIn.TabIndex = 12;
            numericCountIn.TextAlign = HorizontalAlignment.Center;
            numericCountIn.ValueChanged += NumericCountIn_ValueChanged;
            // 
            // checkBoxFlash
            // 
            checkBoxFlash.Anchor = AnchorStyles.Left;
            checkBoxFlash.Appearance = Appearance.ToggleSwitch;
            checkBoxFlash.AutoSize = true;
            controlPanel.SetFlowBreak(checkBoxFlash, true);
            checkBoxFlash.Location = new Point(1053, 15);
            checkBoxFlash.Margin = new Padding(12, 3, 3, 3);
            checkBoxFlash.Name = "checkBoxFlash";
            checkBoxFlash.Size = new Size(175, 19);
            checkBoxFlash.TabIndex = 13;
            checkBoxFlash.Text = "Taktwechsel anzeigen";
            checkBoxFlash.UseVisualStyleBackColor = true;
            checkBoxFlash.VisualStylesMode = VisualStylesMode.Net11;
            checkBoxFlash.CheckedChanged += CheckBoxFlash_CheckedChanged;
            // 
            // labelPattern
            // 
            labelPattern.Anchor = AnchorStyles.Left;
            labelPattern.AutoSize = true;
            labelPattern.Location = new Point(12, 53);
            labelPattern.Margin = new Padding(6, 0, 0, 0);
            labelPattern.Name = "labelPattern";
            labelPattern.Size = new Size(54, 19);
            labelPattern.TabIndex = 14;
            labelPattern.Text = "Pattern";
            // 
            // radioPattern1
            // 
            radioPattern1.Anchor = AnchorStyles.Left;
            radioPattern1.Appearance = Appearance.Button;
            radioPattern1.Checked = true;
            radioPattern1.Location = new Point(69, 47);
            radioPattern1.Margin = new Padding(3, 3, 0, 3);
            radioPattern1.Name = "radioPattern1";
            radioPattern1.Size = new Size(34, 32);
            radioPattern1.TabIndex = 15;
            radioPattern1.TabStop = true;
            radioPattern1.Text = "1";
            radioPattern1.TextAlign = ContentAlignment.MiddleCenter;
            radioPattern1.UseVisualStyleBackColor = true;
            radioPattern1.CheckedChanged += RadioPattern_CheckedChanged;
            // 
            // radioPattern2
            // 
            radioPattern2.Anchor = AnchorStyles.Left;
            radioPattern2.Appearance = Appearance.Button;
            radioPattern2.Location = new Point(103, 47);
            radioPattern2.Margin = new Padding(0, 3, 0, 3);
            radioPattern2.Name = "radioPattern2";
            radioPattern2.Size = new Size(34, 32);
            radioPattern2.TabIndex = 16;
            radioPattern2.Text = "2";
            radioPattern2.TextAlign = ContentAlignment.MiddleCenter;
            radioPattern2.UseVisualStyleBackColor = true;
            radioPattern2.CheckedChanged += RadioPattern_CheckedChanged;
            // 
            // radioPattern3
            // 
            radioPattern3.Anchor = AnchorStyles.Left;
            radioPattern3.Appearance = Appearance.Button;
            radioPattern3.Location = new Point(137, 47);
            radioPattern3.Margin = new Padding(0, 3, 0, 3);
            radioPattern3.Name = "radioPattern3";
            radioPattern3.Size = new Size(34, 32);
            radioPattern3.TabIndex = 17;
            radioPattern3.Text = "3";
            radioPattern3.TextAlign = ContentAlignment.MiddleCenter;
            radioPattern3.UseVisualStyleBackColor = true;
            radioPattern3.CheckedChanged += RadioPattern_CheckedChanged;
            // 
            // radioPattern4
            // 
            radioPattern4.Anchor = AnchorStyles.Left;
            radioPattern4.Appearance = Appearance.Button;
            radioPattern4.Location = new Point(171, 47);
            radioPattern4.Margin = new Padding(0, 3, 0, 3);
            radioPattern4.Name = "radioPattern4";
            radioPattern4.Size = new Size(34, 32);
            radioPattern4.TabIndex = 18;
            radioPattern4.Text = "4";
            radioPattern4.TextAlign = ContentAlignment.MiddleCenter;
            radioPattern4.UseVisualStyleBackColor = true;
            radioPattern4.CheckedChanged += RadioPattern_CheckedChanged;
            // 
            // labelMeter
            // 
            labelMeter.Anchor = AnchorStyles.Left;
            labelMeter.AutoSize = true;
            labelMeter.Location = new Point(219, 53);
            labelMeter.Margin = new Padding(14, 0, 0, 0);
            labelMeter.Name = "labelMeter";
            labelMeter.Size = new Size(50, 19);
            labelMeter.TabIndex = 19;
            labelMeter.Text = "Taktart";
            // 
            // comboMeter
            // 
            comboMeter.Anchor = AnchorStyles.Left;
            comboMeter.DropDownStyle = ComboBoxStyle.DropDownList;
            comboMeter.Location = new Point(272, 51);
            comboMeter.Name = "comboMeter";
            comboMeter.Size = new Size(110, 25);
            comboMeter.TabIndex = 20;
            comboMeter.SelectedIndexChanged += ComboMeter_SelectedIndexChanged;
            // 
            // checkBoxSequence
            // 
            checkBoxSequence.Anchor = AnchorStyles.Left;
            checkBoxSequence.Appearance = Appearance.ToggleSwitch;
            checkBoxSequence.AutoSize = true;
            checkBoxSequence.Location = new Point(399, 53);
            checkBoxSequence.Margin = new Padding(14, 3, 3, 3);
            checkBoxSequence.Name = "checkBoxSequence";
            checkBoxSequence.Size = new Size(93, 19);
            checkBoxSequence.TabIndex = 21;
            checkBoxSequence.Text = "Abfolge";
            checkBoxSequence.UseVisualStyleBackColor = true;
            checkBoxSequence.VisualStylesMode = VisualStylesMode.Net11;
            checkBoxSequence.CheckedChanged += CheckBoxSequence_CheckedChanged;
            // 
            // textBoxSequence
            // 
            textBoxSequence.Anchor = AnchorStyles.Left;
            textBoxSequence.Enabled = false;
            textBoxSequence.Location = new Point(498, 50);
            textBoxSequence.Name = "textBoxSequence";
            textBoxSequence.Size = new Size(260, 25);
            textBoxSequence.TabIndex = 22;
            textBoxSequence.TextChanged += TextBoxSequence_TextChanged;
            textBoxSequence.Leave += TextBoxSequence_Leave;
            // 
            // checkBoxRepeat
            // 
            checkBoxRepeat.Anchor = AnchorStyles.Left;
            checkBoxRepeat.Appearance = Appearance.ToggleSwitch;
            checkBoxRepeat.AutoSize = true;
            checkBoxRepeat.Checked = true;
            checkBoxRepeat.CheckState = CheckState.Checked;
            checkBoxRepeat.Enabled = false;
            checkBoxRepeat.Location = new Point(767, 53);
            checkBoxRepeat.Margin = new Padding(6, 3, 3, 3);
            checkBoxRepeat.Name = "checkBoxRepeat";
            checkBoxRepeat.Size = new Size(123, 19);
            checkBoxRepeat.TabIndex = 23;
            checkBoxRepeat.Text = "Wiederholen";
            checkBoxRepeat.UseVisualStyleBackColor = true;
            checkBoxRepeat.VisualStylesMode = VisualStylesMode.Net11;
            checkBoxRepeat.CheckedChanged += CheckBoxRepeat_CheckedChanged;
            // 
            // labelSequenceHint
            // 
            labelSequenceHint.Anchor = AnchorStyles.Left;
            labelSequenceHint.AutoSize = true;
            labelSequenceHint.ForeColor = SystemColors.GrayText;
            labelSequenceHint.Location = new Point(896, 53);
            labelSequenceHint.Name = "labelSequenceHint";
            labelSequenceHint.Size = new Size(208, 19);
            labelSequenceHint.TabIndex = 24;
            labelSequenceHint.Text = "Ein Pattern pro Takt, z. B. 1 1 1 2";
            // 
            // stepGrid
            // 
            stepGrid.BackColor = SystemColors.Window;
            stepGrid.Dock = DockStyle.Fill;
            stepGrid.Location = new Point(0, 111);
            stepGrid.Name = "stepGrid";
            stepGrid.Size = new Size(1260, 469);
            stepGrid.TabIndex = 2;
            stepGrid.Edited += StepGrid_Edited;
            stepGrid.EditCompleted += StepGrid_EditCompleted;
            // 
            // playheadTimer
            // 
            playheadTimer.Interval = 15;
            playheadTimer.Tick += PlayheadTimer_Tick;
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
            ClientSize = new Size(1260, 580);
            Controls.Add(stepGrid);
            Controls.Add(controlPanel);
            Controls.Add(menuStrip);
            Font = new Font("Segoe UI", 10F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            MinimumSize = new Size(960, 500);
            Name = "FrmDrums4me";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "drums4me";
            FormClosing += FrmDrums4me_FormClosing;
            Load += FrmDrums4me_Load;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            controlPanel.ResumeLayout(false);
            controlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericTempo).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarSwing).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVolume).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericCountIn).EndInit();
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
        private ToolStripMenuItem kitsToolStripMenuItem;
        private ToolStripMenuItem openKitFolderToolStripMenuItem;
        private ToolStripMenuItem reloadKitsToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem projectFolderToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private FlowLayoutPanel controlPanel;
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
        private Label labelSequenceHint;
        private Label labelCountIn;
        private NumericUpDown numericCountIn;
        private CheckBox checkBoxFlash;
        private Drums4me.Controls.StepGrid stepGrid;
        private Label labelMeter;
        private ComboBox comboMeter;
        private ToolTip toolTip;
        private System.Windows.Forms.Timer playheadTimer;
        private System.Windows.Forms.Timer pendingOpenTimer;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private FolderBrowserDialog folderBrowserDialog;
    }
}
