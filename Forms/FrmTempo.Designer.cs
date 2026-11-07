namespace Drums4me.Forms
{
    partial class FrmTempo
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
            labelHint = new Label();
            labelBpm = new Label();
            labelStatus = new Label();
            buttonHalf = new Button();
            buttonDouble = new Button();
            buttonReset = new Button();
            buttonApply = new Button();
            buttonCancel = new Button();
            flashTimer = new System.Windows.Forms.Timer(components);
            updateTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            //
            // labelHint
            //
            labelHint.Location = new Point(12, 12);
            labelHint.Name = "labelHint";
            labelHint.Size = new Size(546, 44);
            labelHint.TabIndex = 0;
            labelHint.Text = "Tippe im Takt auf die Leertaste oder spiele den Song in einem beliebigen Programm ab: drums4me hört den Ton von Windows mit. Nichts davon wird gespeichert.";
            labelHint.TextAlign = ContentAlignment.MiddleCenter;
            //
            // labelBpm
            //
            labelBpm.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            labelBpm.Location = new Point(12, 62);
            labelBpm.Name = "labelBpm";
            labelBpm.Size = new Size(546, 84);
            labelBpm.TabIndex = 1;
            labelBpm.Text = "– BPM";
            labelBpm.TextAlign = ContentAlignment.MiddleCenter;
            //
            // labelStatus
            //
            labelStatus.AutoEllipsis = true;
            labelStatus.ForeColor = SystemColors.GrayText;
            labelStatus.Location = new Point(12, 150);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(546, 22);
            labelStatus.TabIndex = 2;
            labelStatus.TextAlign = ContentAlignment.MiddleCenter;
            //
            // buttonHalf
            //
            buttonHalf.Enabled = false;
            buttonHalf.Location = new Point(12, 186);
            buttonHalf.Name = "buttonHalf";
            buttonHalf.Size = new Size(70, 32);
            buttonHalf.TabIndex = 3;
            buttonHalf.Text = "Halb";
            buttonHalf.UseVisualStyleBackColor = true;
            buttonHalf.Click += ButtonHalf_Click;
            //
            // buttonDouble
            //
            buttonDouble.Enabled = false;
            buttonDouble.Location = new Point(88, 186);
            buttonDouble.Name = "buttonDouble";
            buttonDouble.Size = new Size(70, 32);
            buttonDouble.TabIndex = 4;
            buttonDouble.Text = "Doppelt";
            buttonDouble.UseVisualStyleBackColor = true;
            buttonDouble.Click += ButtonDouble_Click;
            //
            // buttonReset
            //
            buttonReset.Location = new Point(164, 186);
            buttonReset.Name = "buttonReset";
            buttonReset.Size = new Size(116, 32);
            buttonReset.TabIndex = 5;
            buttonReset.Text = "Neu beginnen";
            buttonReset.UseVisualStyleBackColor = true;
            buttonReset.Click += ButtonReset_Click;
            //
            // buttonApply
            //
            buttonApply.DialogResult = DialogResult.OK;
            buttonApply.Enabled = false;
            buttonApply.Location = new Point(354, 186);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new Size(108, 32);
            buttonApply.TabIndex = 6;
            buttonApply.Text = "Übernehmen";
            buttonApply.UseVisualStyleBackColor = true;
            //
            // buttonCancel
            //
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(468, 186);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(90, 32);
            buttonCancel.TabIndex = 7;
            buttonCancel.Text = "Abbrechen";
            buttonCancel.UseVisualStyleBackColor = true;
            //
            // flashTimer
            //
            flashTimer.Interval = 90;
            flashTimer.Tick += FlashTimer_Tick;
            //
            // updateTimer
            //
            updateTimer.Enabled = true;
            updateTimer.Interval = 500;
            updateTimer.Tick += UpdateTimer_Tick;
            //
            // FrmTempo
            //
            AcceptButton = buttonApply;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(570, 230);
            Controls.Add(labelHint);
            Controls.Add(labelBpm);
            Controls.Add(labelStatus);
            Controls.Add(buttonHalf);
            Controls.Add(buttonDouble);
            Controls.Add(buttonReset);
            Controls.Add(buttonApply);
            Controls.Add(buttonCancel);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmTempo";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Tempo ermitteln";
            FormClosed += FrmTempo_FormClosed;
            Shown += FrmTempo_Shown;
            ResumeLayout(false);
        }

        #endregion

        private Label labelHint;
        private Label labelBpm;
        private Label labelStatus;
        private Button buttonHalf;
        private Button buttonDouble;
        private Button buttonReset;
        private Button buttonApply;
        private Button buttonCancel;
        private System.Windows.Forms.Timer flashTimer;
        private System.Windows.Forms.Timer updateTimer;
    }
}
