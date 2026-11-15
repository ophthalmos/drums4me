namespace Drums4me.Forms
{
    partial class FrmMidi
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
            labelInfo = new Label();
            listBars = new ListView();
            columnBar = new ColumnHeader();
            columnMeter = new ColumnHeader();
            columnCount = new ColumnHeader();
            panelPreview = new Panel();
            labelChoice = new Label();
            labelMapping = new Label();
            buttonImport = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // labelInfo
            // 
            labelInfo.Location = new Point(12, 12);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(676, 62);
            labelInfo.TabIndex = 0;
            labelInfo.Text = "Wähle bis zu vier Takte, sie werden zu den Patterns 1 bis 4 eines neuen Projekts.";
            // 
            // listBars
            // 
            listBars.CheckBoxes = true;
            listBars.Columns.AddRange(new ColumnHeader[] { columnBar, columnMeter, columnCount });
            listBars.FullRowSelect = true;
            listBars.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listBars.HideSelection = false;
            listBars.Location = new Point(12, 80);
            listBars.MultiSelect = false;
            listBars.Name = "listBars";
            listBars.Size = new Size(268, 200);
            listBars.TabIndex = 1;
            listBars.UseCompatibleStateImageBehavior = false;
            listBars.View = View.Details;
            listBars.ItemCheck += ListBars_ItemCheck;
            listBars.ItemChecked += ListBars_ItemChecked;
            listBars.SelectedIndexChanged += ListBars_SelectedIndexChanged;
            // 
            // columnBar
            // 
            columnBar.Text = "Takt";
            columnBar.Width = 96;
            // 
            // columnMeter
            // 
            columnMeter.Text = "Taktart";
            columnMeter.Width = 72;
            // 
            // columnCount
            // 
            columnCount.Text = "kommt vor";
            columnCount.TextAlign = HorizontalAlignment.Right;
            columnCount.Width = 76;
            // 
            // panelPreview
            // 
            panelPreview.Location = new Point(292, 80);
            panelPreview.Name = "panelPreview";
            panelPreview.Size = new Size(396, 200);
            panelPreview.TabIndex = 2;
            panelPreview.Paint += PanelPreview_Paint;
            // 
            // labelChoice
            // 
            labelChoice.Location = new Point(12, 288);
            labelChoice.Name = "labelChoice";
            labelChoice.Size = new Size(676, 22);
            labelChoice.TabIndex = 3;
            labelChoice.Text = "";
            // 
            // labelMapping
            // 
            labelMapping.AutoEllipsis = true;
            labelMapping.ForeColor = SystemColors.GrayText;
            labelMapping.Location = new Point(12, 318);
            labelMapping.Name = "labelMapping";
            labelMapping.Size = new Size(676, 98);
            labelMapping.TabIndex = 4;
            labelMapping.Text = "";
            // 
            // buttonImport
            // 
            buttonImport.DialogResult = DialogResult.OK;
            buttonImport.Location = new Point(482, 424);
            buttonImport.Name = "buttonImport";
            buttonImport.Size = new Size(108, 32);
            buttonImport.TabIndex = 5;
            buttonImport.Text = "Importieren";
            buttonImport.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(598, 424);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(90, 32);
            buttonCancel.TabIndex = 6;
            buttonCancel.Text = "Abbrechen";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // FrmMidi
            // 
            AcceptButton = buttonImport;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(700, 468);
            Controls.Add(labelInfo);
            Controls.Add(listBars);
            Controls.Add(panelPreview);
            Controls.Add(labelChoice);
            Controls.Add(labelMapping);
            Controls.Add(buttonImport);
            Controls.Add(buttonCancel);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmMidi";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "MIDI-Groove importieren";
            Shown += FrmMidi_Shown;
            ResumeLayout(false);
        }

        #endregion

        private Label labelInfo;
        private ListView listBars;
        private ColumnHeader columnBar;
        private ColumnHeader columnMeter;
        private ColumnHeader columnCount;
        private Panel panelPreview;
        private Label labelChoice;
        private Label labelMapping;
        private Button buttonImport;
        private Button buttonCancel;
    }
}
