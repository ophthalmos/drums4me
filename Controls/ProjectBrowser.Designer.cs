namespace Drums4me.Controls
{
    partial class ProjectBrowser
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

        #region Component Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listProjects = new ListView();
            columnName = new ColumnHeader();
            columnFolder = new ColumnHeader();
            columnModified = new ColumnHeader();
            columnTempo = new ColumnHeader();
            columnMeter = new ColumnHeader();
            columnKit = new ColumnHeader();
            SuspendLayout();
            //
            // listProjects
            //
            listProjects.AccessibleDescription = "Doppelklick oder Enter: Projekt öffnen. Klick auf einen Spaltenkopf: sortieren.";
            listProjects.BorderStyle = BorderStyle.None;
            listProjects.Columns.AddRange(new ColumnHeader[] { columnName, columnFolder, columnModified, columnTempo, columnMeter, columnKit });
            listProjects.Dock = DockStyle.Fill;
            listProjects.FullRowSelect = true;
            listProjects.HideSelection = false;
            listProjects.Location = new Point(0, 0);
            listProjects.MultiSelect = false;
            listProjects.Name = "listProjects";
            listProjects.ShowGroups = false;
            listProjects.Size = new Size(1000, 500);
            listProjects.TabIndex = 0;
            listProjects.UseCompatibleStateImageBehavior = false;
            listProjects.View = View.Details;
            listProjects.ColumnClick += ListProjects_ColumnClick;
            listProjects.ItemActivate += ListProjects_ItemActivate;
            listProjects.ClientSizeChanged += ListProjects_ClientSizeChanged;
            //
            // columnName
            //
            columnName.Text = "Name";
            columnName.Width = 400;
            //
            // columnFolder
            //
            columnFolder.Text = "Ordner";
            columnFolder.Width = 150;
            //
            // columnModified
            //
            columnModified.Text = "Geändert";
            columnModified.TextAlign = HorizontalAlignment.Right;
            columnModified.Width = 135;
            //
            // columnTempo
            //
            columnTempo.Text = "Tempo";
            columnTempo.TextAlign = HorizontalAlignment.Right;
            columnTempo.Width = 75;
            //
            // columnMeter
            //
            columnMeter.Text = "Taktart";
            columnMeter.Width = 130;
            //
            // columnKit
            //
            columnKit.Text = "Kit";
            columnKit.Width = 180;
            //
            // ProjectBrowser
            //
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(listProjects);
            Font = new Font("Segoe UI", 10F);
            Name = "ProjectBrowser";
            Size = new Size(1000, 500);
            ResumeLayout(false);
        }

        #endregion

        private ListView listProjects;
        private ColumnHeader columnName;
        private ColumnHeader columnFolder;
        private ColumnHeader columnModified;
        private ColumnHeader columnTempo;
        private ColumnHeader columnMeter;
        private ColumnHeader columnKit;
    }
}
