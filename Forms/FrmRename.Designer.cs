namespace Drums4me.Forms
{
    partial class FrmRename
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
            labelName = new Label();
            textBoxName = new TextBox();
            labelHint = new Label();
            buttonOk = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            //
            // labelName
            //
            labelName.AutoSize = true;
            labelName.Location = new Point(12, 12);
            labelName.Name = "labelName";
            labelName.Size = new Size(144, 19);
            labelName.TabIndex = 0;
            labelName.Text = "Neuer Name der Spur:";
            //
            // textBoxName
            //
            textBoxName.Location = new Point(12, 36);
            textBoxName.MaxLength = 40;
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(366, 25);
            textBoxName.TabIndex = 1;
            textBoxName.VisualStylesMode = VisualStylesMode.Net11;
            textBoxName.TextChanged += TextBoxName_TextChanged;
            //
            // labelHint
            //
            labelHint.ForeColor = SystemColors.GrayText;
            labelHint.Location = new Point(12, 66);
            labelHint.Name = "labelHint";
            labelHint.Size = new Size(366, 22);
            labelHint.TabIndex = 2;
            labelHint.Text = "Auch die Datei im Kit-Ordner bekommt diesen Namen.";
            //
            // buttonOk
            //
            buttonOk.DialogResult = DialogResult.OK;
            buttonOk.Location = new Point(166, 98);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(116, 32);
            buttonOk.TabIndex = 3;
            buttonOk.Text = "Umbenennen";
            buttonOk.UseVisualStyleBackColor = true;
            //
            // buttonCancel
            //
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(288, 98);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(90, 32);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "Abbrechen";
            buttonCancel.UseVisualStyleBackColor = true;
            //
            // FrmRename
            //
            AcceptButton = buttonOk;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(390, 142);
            Controls.Add(labelName);
            Controls.Add(textBoxName);
            Controls.Add(labelHint);
            Controls.Add(buttonOk);
            Controls.Add(buttonCancel);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmRename";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Spur umbenennen";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelName;
        private TextBox textBoxName;
        private Label labelHint;
        private Button buttonOk;
        private Button buttonCancel;
    }
}
