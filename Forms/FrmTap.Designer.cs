namespace Drums4me.Forms
{
    partial class FrmTap
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
            buttonReset = new Button();
            buttonApply = new Button();
            buttonCancel = new Button();
            flashTimer = new System.Windows.Forms.Timer(components);
            pauseTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            //
            // labelHint
            //
            labelHint.Location = new Point(12, 12);
            labelHint.Name = "labelHint";
            labelHint.Size = new Size(366, 22);
            labelHint.TabIndex = 0;
            labelHint.Text = "Tippe im Takt auf die Leertaste.";
            labelHint.TextAlign = ContentAlignment.MiddleCenter;
            //
            // labelBpm
            //
            labelBpm.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            labelBpm.Location = new Point(12, 42);
            labelBpm.Name = "labelBpm";
            labelBpm.Size = new Size(366, 84);
            labelBpm.TabIndex = 1;
            labelBpm.Text = "– BPM";
            labelBpm.TextAlign = ContentAlignment.MiddleCenter;
            //
            // labelStatus
            //
            labelStatus.ForeColor = SystemColors.GrayText;
            labelStatus.Location = new Point(12, 132);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(366, 22);
            labelStatus.TabIndex = 2;
            labelStatus.Text = "Noch kein Schlag";
            labelStatus.TextAlign = ContentAlignment.MiddleCenter;
            //
            // buttonReset
            //
            buttonReset.Location = new Point(12, 168);
            buttonReset.Name = "buttonReset";
            buttonReset.Size = new Size(116, 32);
            buttonReset.TabIndex = 3;
            buttonReset.Text = "Neu beginnen";
            buttonReset.UseVisualStyleBackColor = true;
            buttonReset.Click += ButtonReset_Click;
            //
            // buttonApply
            //
            buttonApply.DialogResult = DialogResult.OK;
            buttonApply.Enabled = false;
            buttonApply.Location = new Point(174, 168);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new Size(108, 32);
            buttonApply.TabIndex = 4;
            buttonApply.Text = "Übernehmen";
            buttonApply.UseVisualStyleBackColor = true;
            //
            // buttonCancel
            //
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(288, 168);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(90, 32);
            buttonCancel.TabIndex = 5;
            buttonCancel.Text = "Abbrechen";
            buttonCancel.UseVisualStyleBackColor = true;
            //
            // flashTimer
            //
            flashTimer.Interval = 90;
            flashTimer.Tick += FlashTimer_Tick;
            //
            // pauseTimer
            //
            pauseTimer.Enabled = true;
            pauseTimer.Interval = 200;
            pauseTimer.Tick += PauseTimer_Tick;
            //
            // FrmTap
            //
            AcceptButton = buttonApply;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(390, 212);
            Controls.Add(labelHint);
            Controls.Add(labelBpm);
            Controls.Add(labelStatus);
            Controls.Add(buttonReset);
            Controls.Add(buttonApply);
            Controls.Add(buttonCancel);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmTap";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Tempo tippen";
            ResumeLayout(false);
        }

        #endregion

        private Label labelHint;
        private Label labelBpm;
        private Label labelStatus;
        private Button buttonReset;
        private Button buttonApply;
        private Button buttonCancel;
        private System.Windows.Forms.Timer flashTimer;
        private System.Windows.Forms.Timer pauseTimer;
    }
}
