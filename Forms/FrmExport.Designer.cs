namespace Drums4me.Forms
{
    partial class FrmExport
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
            labelHint = new Label();
            labelBars = new Label();
            numericBars = new NumericUpDown();
            labelUnit = new Label();
            labelDuration = new Label();
            labelFormat = new Label();
            radioWave = new RadioButton();
            radioFlac = new RadioButton();
            radioMp3 = new RadioButton();
            labelNote = new Label();
            buttonSave = new Button();
            buttonCancel = new Button();
            exportFileDialog = new SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)numericBars).BeginInit();
            SuspendLayout();
            //
            // labelHint
            //
            labelHint.Location = new Point(12, 12);
            labelHint.Name = "labelHint";
            labelHint.Size = new Size(536, 84);
            labelHint.TabIndex = 0;
            labelHint.Text = "Gespeichert wird, was die Wiedergabe spielt.";
            //
            // labelBars
            //
            labelBars.AutoSize = true;
            labelBars.Location = new Point(12, 108);
            labelBars.Name = "labelBars";
            labelBars.Size = new Size(48, 19);
            labelBars.TabIndex = 1;
            labelBars.Text = "Länge:";
            //
            // numericBars
            //
            numericBars.Location = new Point(84, 105);
            numericBars.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            numericBars.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericBars.Name = "numericBars";
            numericBars.Size = new Size(70, 25);
            numericBars.TabIndex = 2;
            numericBars.TextAlign = HorizontalAlignment.Center;
            numericBars.Value = new decimal(new int[] { 4, 0, 0, 0 });
            numericBars.ValueChanged += NumericBars_ValueChanged;
            //
            // labelUnit
            //
            labelUnit.AutoSize = true;
            labelUnit.Location = new Point(160, 108);
            labelUnit.Name = "labelUnit";
            labelUnit.Size = new Size(40, 19);
            labelUnit.TabIndex = 3;
            labelUnit.Text = "Takte";
            //
            // labelDuration
            //
            labelDuration.AutoSize = true;
            labelDuration.ForeColor = SystemColors.GrayText;
            labelDuration.Location = new Point(224, 108);
            labelDuration.Name = "labelDuration";
            labelDuration.Size = new Size(82, 19);
            labelDuration.TabIndex = 4;
            labelDuration.Text = "Dauer: 0:00";
            //
            // labelFormat
            //
            labelFormat.AutoSize = true;
            labelFormat.Location = new Point(12, 144);
            labelFormat.Name = "labelFormat";
            labelFormat.Size = new Size(55, 19);
            labelFormat.TabIndex = 5;
            labelFormat.Text = "Format:";
            //
            // radioWave
            //
            radioWave.AutoSize = true;
            radioWave.Checked = true;
            radioWave.Location = new Point(84, 142);
            radioWave.Name = "radioWave";
            radioWave.Size = new Size(380, 23);
            radioWave.TabIndex = 6;
            radioWave.TabStop = true;
            radioWave.Text = "WAV: unkomprimiert, für Looper und Aufnahme-Software";
            radioWave.UseVisualStyleBackColor = true;
            //
            // radioFlac
            //
            radioFlac.AutoSize = true;
            radioFlac.Location = new Point(84, 168);
            radioFlac.Name = "radioFlac";
            radioFlac.Size = new Size(330, 23);
            radioFlac.TabIndex = 7;
            radioFlac.Text = "FLAC: verlustfrei komprimiert, deutlich kleiner";
            radioFlac.UseVisualStyleBackColor = true;
            //
            // radioMp3
            //
            radioMp3.AutoSize = true;
            radioMp3.Location = new Point(84, 194);
            radioMp3.Name = "radioMp3";
            radioMp3.Size = new Size(450, 23);
            radioMp3.TabIndex = 8;
            radioMp3.Text = "MP3: klein und überall abspielbar, aber nicht nahtlos wiederholbar";
            radioMp3.UseVisualStyleBackColor = true;
            //
            // labelNote
            //
            labelNote.AutoSize = true;
            labelNote.ForeColor = SystemColors.GrayText;
            labelNote.Location = new Point(12, 232);
            labelNote.Name = "labelNote";
            labelNote.Size = new Size(360, 19);
            labelNote.TabIndex = 9;
            labelNote.Text = "Vorzählen und Tempotrainer kommen nicht in die Datei.";
            //
            // buttonSave
            //
            buttonSave.Location = new Point(354, 270);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(108, 32);
            buttonSave.TabIndex = 10;
            buttonSave.Text = "Speichern…";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += ButtonSave_Click;
            //
            // buttonCancel
            //
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(468, 270);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(90, 32);
            buttonCancel.TabIndex = 11;
            buttonCancel.Text = "Abbrechen";
            buttonCancel.UseVisualStyleBackColor = true;
            //
            // exportFileDialog
            //
            exportFileDialog.Title = "Als Audiodatei speichern";
            //
            // FrmExport
            //
            AcceptButton = buttonSave;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(570, 314);
            Controls.Add(labelHint);
            Controls.Add(labelBars);
            Controls.Add(numericBars);
            Controls.Add(labelUnit);
            Controls.Add(labelDuration);
            Controls.Add(labelFormat);
            Controls.Add(radioWave);
            Controls.Add(radioFlac);
            Controls.Add(radioMp3);
            Controls.Add(labelNote);
            Controls.Add(buttonSave);
            Controls.Add(buttonCancel);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmExport";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Als Audiodatei speichern";
            ((System.ComponentModel.ISupportInitialize)numericBars).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelHint;
        private Label labelBars;
        private NumericUpDown numericBars;
        private Label labelUnit;
        private Label labelDuration;
        private Label labelFormat;
        private RadioButton radioWave;
        private RadioButton radioFlac;
        private RadioButton radioMp3;
        private Label labelNote;
        private Button buttonSave;
        private Button buttonCancel;
        private SaveFileDialog exportFileDialog;
    }
}
