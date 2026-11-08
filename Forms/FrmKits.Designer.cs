namespace Drums4me.Forms
{
    partial class FrmKits
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmKits));
            labelHint = new Label();
            buttonOpenFolder = new Button();
            listKits = new ListView();
            columnKit = new ColumnHeader();
            columnCollection = new ColumnHeader();
            columnTracks = new ColumnHeader();
            buttonAll = new Button();
            buttonNone = new Button();
            labelCount = new Label();
            linkKitGuide = new LinkLabel();
            buttonOk = new Button();
            buttonCancel = new Button();
            toolTip = new ToolTip(components);
            SuspendLayout();
            // 
            // labelHint
            // 
            labelHint.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelHint.Location = new Point(12, 10);
            labelHint.Name = "labelHint";
            labelHint.Size = new Size(460, 40);
            labelHint.TabIndex = 0;
            labelHint.Text = "Nur angehakte Kits erscheinen in der Auswahl auf der Start-Registerkarte.\r\nProjekte mit einem ausgeblendeten Kit klingen weiterhin wie gewohnt.";
            // 
            // buttonOpenFolder
            // 
            buttonOpenFolder.AccessibleDescription = "Öffnet den Ordner des markierten Kits, etwa um Klänge auszutauschen.";
            buttonOpenFolder.AccessibleName = "Ordner des Kits öffnen";
            buttonOpenFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonOpenFolder.Enabled = false;
            buttonOpenFolder.Image = (Image)resources.GetObject("buttonOpenFolder.Image");
            buttonOpenFolder.ImageAlign = ContentAlignment.MiddleRight;
            buttonOpenFolder.Location = new Point(478, 12);
            buttonOpenFolder.Name = "buttonOpenFolder";
            buttonOpenFolder.Size = new Size(290, 29);
            buttonOpenFolder.TabIndex = 2;
            buttonOpenFolder.TextAlign = ContentAlignment.MiddleRight;
            buttonOpenFolder.TextImageRelation = TextImageRelation.TextBeforeImage;
            toolTip.SetToolTip(buttonOpenFolder, "Ordner des markierten Kits öffnen");
            buttonOpenFolder.UseVisualStyleBackColor = true;
            buttonOpenFolder.SizeChanged += ButtonOpenFolder_SizeChanged;
            buttonOpenFolder.Click += ButtonOpenFolder_Click;
            // 
            // listKits
            // 
            listKits.AccessibleDescription = "Haken: Das Kit erscheint in der Auswahl. Leertaste oder Doppelklick schaltet den Haken um.";
            listKits.AccessibleName = "Kits";
            listKits.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listKits.CheckBoxes = true;
            listKits.Columns.AddRange(new ColumnHeader[] { columnKit, columnCollection, columnTracks });
            listKits.FullRowSelect = true;
            listKits.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listKits.Location = new Point(12, 56);
            listKits.MultiSelect = false;
            listKits.Name = "listKits";
            listKits.ShowGroups = false;
            listKits.ShowItemToolTips = true;
            listKits.Size = new Size(756, 410);
            listKits.TabIndex = 1;
            listKits.UseCompatibleStateImageBehavior = false;
            listKits.View = View.Details;
            listKits.ItemChecked += ListKits_ItemChecked;
            listKits.SelectedIndexChanged += ListKits_SelectedIndexChanged;
            // 
            // columnKit
            // 
            columnKit.Text = "Kit";
            columnKit.Width = 130;
            // 
            // columnCollection
            // 
            columnCollection.Text = "Sammlung";
            columnCollection.Width = 100;
            // 
            // columnTracks
            // 
            columnTracks.Text = "Klänge";
            columnTracks.Width = 495;
            // 
            // buttonAll
            // 
            buttonAll.AccessibleDescription = "Hakt alle Kits an.";
            buttonAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonAll.Location = new Point(12, 476);
            buttonAll.Name = "buttonAll";
            buttonAll.Size = new Size(80, 29);
            buttonAll.TabIndex = 3;
            buttonAll.Text = "Alle";
            buttonAll.UseVisualStyleBackColor = true;
            buttonAll.Click += ButtonAll_Click;
            // 
            // buttonNone
            // 
            buttonNone.AccessibleDescription = "Entfernt alle Haken, damit du danach nur die gewünschten Kits anhakst.";
            buttonNone.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonNone.Location = new Point(98, 476);
            buttonNone.Name = "buttonNone";
            buttonNone.Size = new Size(80, 29);
            buttonNone.TabIndex = 4;
            buttonNone.Text = "Keine";
            buttonNone.UseVisualStyleBackColor = true;
            buttonNone.Click += ButtonNone_Click;
            // 
            // labelCount
            // 
            labelCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelCount.ForeColor = SystemColors.GrayText;
            labelCount.Location = new Point(190, 476);
            labelCount.Name = "labelCount";
            labelCount.Size = new Size(240, 29);
            labelCount.TabIndex = 5;
            labelCount.Text = "0 von 0 ausgewählt";
            labelCount.TextAlign = ContentAlignment.MiddleLeft;
            //
            // linkKitGuide
            //
            linkKitGuide.AccessibleDescription = "Erklärt, wie du eigene Kits anlegst und ihre Klänge aufbereitest.";
            linkKitGuide.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            linkKitGuide.Location = new Point(436, 476);
            linkKitGuide.Name = "linkKitGuide";
            linkKitGuide.Size = new Size(150, 29);
            linkKitGuide.TabIndex = 6;
            linkKitGuide.TabStop = true;
            linkKitGuide.Text = "Eigene Kits anlegen…";
            linkKitGuide.TextAlign = ContentAlignment.MiddleRight;
            linkKitGuide.LinkClicked += LinkKitGuide_LinkClicked;
            // 
            // buttonOk
            // 
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOk.DialogResult = DialogResult.OK;
            buttonOk.Location = new Point(592, 476);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(85, 29);
            buttonOk.TabIndex = 7;
            buttonOk.Text = "OK";
            buttonOk.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(683, 476);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(85, 29);
            buttonCancel.TabIndex = 8;
            buttonCancel.Text = "Abbrechen";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // FrmKits
            // 
            AcceptButton = buttonOk;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(780, 517);
            Controls.Add(labelHint);
            Controls.Add(buttonOpenFolder);
            Controls.Add(listKits);
            Controls.Add(buttonAll);
            Controls.Add(buttonNone);
            Controls.Add(labelCount);
            Controls.Add(linkKitGuide);
            Controls.Add(buttonOk);
            Controls.Add(buttonCancel);
            Font = new Font("Segoe UI", 10F);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(480, 360);
            Name = "FrmKits";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Kits auswählen";
            ResumeLayout(false);
        }

        #endregion

        private Label labelHint;
        private Button buttonOpenFolder;
        private ListView listKits;
        private ColumnHeader columnKit;
        private ColumnHeader columnCollection;
        private ColumnHeader columnTracks;
        private Button buttonAll;
        private Button buttonNone;
        private Label labelCount;
        private LinkLabel linkKitGuide;
        private Button buttonOk;
        private Button buttonCancel;
        private ToolTip toolTip;
    }
}
