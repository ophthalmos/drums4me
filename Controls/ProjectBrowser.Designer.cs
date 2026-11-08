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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectBrowser));
            listProjects = new ListView();
            columnName = new ColumnHeader();
            columnFolder = new ColumnHeader();
            columnModified = new ColumnHeader();
            columnTempo = new ColumnHeader();
            columnMeter = new ColumnHeader();
            columnKit = new ColumnHeader();
            projectMenu = new ContextMenuStrip(components);
            menuOpen = new ToolStripMenuItem();
            menuEdit = new ToolStripMenuItem();
            menuShowInFolder = new ToolStripMenuItem();
            menuSeparatorFile = new ToolStripSeparator();
            menuRename = new ToolStripMenuItem();
            menuDuplicate = new ToolStripMenuItem();
            menuCopyPath = new ToolStripMenuItem();
            menuSeparatorDelete = new ToolStripSeparator();
            menuDelete = new ToolStripMenuItem();
            menuSeparatorProperties = new ToolStripSeparator();
            menuProperties = new ToolStripMenuItem();
            projectMenu.SuspendLayout();
            SuspendLayout();
            //
            // listProjects
            //
            listProjects.AccessibleDescription = "Doppelklick oder Enter: Projekt öffnen. F2: umbenennen. Entf: in den Papierkorb. Alt+Eingabe: Eigenschaften. Strg+E: im Editor bearbeiten. Strg+Umschalt+C: Dateipfad kopieren. Rechtsklick: weitere Befehle. Klick auf einen Spaltenkopf: sortieren.";
            listProjects.BorderStyle = BorderStyle.None;
            listProjects.Columns.AddRange(new ColumnHeader[] { columnName, columnFolder, columnModified, columnTempo, columnMeter, columnKit });
            listProjects.ContextMenuStrip = projectMenu;
            listProjects.Dock = DockStyle.Fill;
            listProjects.FullRowSelect = true;
            listProjects.HideSelection = false;
            listProjects.LabelEdit = true;
            listProjects.Location = new Point(0, 0);
            listProjects.MultiSelect = false;
            listProjects.Name = "listProjects";
            listProjects.ShowGroups = false;
            listProjects.Size = new Size(1000, 500);
            listProjects.TabIndex = 0;
            listProjects.UseCompatibleStateImageBehavior = false;
            listProjects.View = View.Details;
            listProjects.AfterLabelEdit += ListProjects_AfterLabelEdit;
            listProjects.BeforeLabelEdit += ListProjects_BeforeLabelEdit;
            listProjects.ColumnClick += ListProjects_ColumnClick;
            listProjects.ItemActivate += ListProjects_ItemActivate;
            listProjects.ColumnWidthChanging += ListProjects_ColumnWidthChanging;
            listProjects.KeyDown += ListProjects_KeyDown;
            //
            // projectMenu
            //
            projectMenu.Items.AddRange(new ToolStripItem[] { menuOpen, menuEdit, menuShowInFolder, menuSeparatorFile, menuRename, menuDuplicate, menuCopyPath, menuSeparatorDelete, menuDelete, menuSeparatorProperties, menuProperties });
            projectMenu.Name = "projectMenu";
            projectMenu.Size = new Size(261, 202);
            projectMenu.Opening += ProjectMenu_Opening;
            //
            // menuOpen
            //
            menuOpen.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            menuOpen.Image = (Image)resources.GetObject("menuOpen.Image");
            menuOpen.Name = "menuOpen";
            menuOpen.Size = new Size(260, 22);
            menuOpen.Text = "Öffnen";
            menuOpen.Click += MenuOpen_Click;
            //
            // menuEdit
            //
            menuEdit.Image = (Image)resources.GetObject("menuEdit.Image");
            menuEdit.Name = "menuEdit";
            menuEdit.ShortcutKeyDisplayString = "Strg+E";
            menuEdit.Size = new Size(260, 22);
            menuEdit.Text = "In Editor bearbeiten";
            menuEdit.Click += MenuEdit_Click;
            //
            // menuShowInFolder
            //
            menuShowInFolder.Image = (Image)resources.GetObject("menuShowInFolder.Image");
            menuShowInFolder.Name = "menuShowInFolder";
            menuShowInFolder.Size = new Size(260, 22);
            menuShowInFolder.Text = "Im Ordner anzeigen";
            menuShowInFolder.Click += MenuShowInFolder_Click;
            //
            // menuSeparatorFile
            //
            menuSeparatorFile.Name = "menuSeparatorFile";
            menuSeparatorFile.Size = new Size(257, 6);
            //
            // menuRename
            //
            menuRename.Image = (Image)resources.GetObject("menuRename.Image");
            menuRename.Name = "menuRename";
            menuRename.ShortcutKeyDisplayString = "F2";
            menuRename.Size = new Size(260, 22);
            menuRename.Text = "Umbenennen";
            menuRename.Click += MenuRename_Click;
            //
            // menuDuplicate
            //
            menuDuplicate.Image = (Image)resources.GetObject("menuDuplicate.Image");
            menuDuplicate.Name = "menuDuplicate";
            menuDuplicate.Size = new Size(260, 22);
            menuDuplicate.Text = "Duplizieren";
            menuDuplicate.Click += MenuDuplicate_Click;
            //
            // menuCopyPath
            //
            menuCopyPath.Image = (Image)resources.GetObject("menuCopyPath.Image");
            menuCopyPath.Name = "menuCopyPath";
            menuCopyPath.ShortcutKeyDisplayString = "Strg+Umschalt+C";
            menuCopyPath.Size = new Size(260, 22);
            menuCopyPath.Text = "Dateipfad kopieren";
            menuCopyPath.Click += MenuCopyPath_Click;
            //
            // menuSeparatorDelete
            //
            menuSeparatorDelete.Name = "menuSeparatorDelete";
            menuSeparatorDelete.Size = new Size(257, 6);
            //
            // menuDelete
            //
            menuDelete.Image = (Image)resources.GetObject("menuDelete.Image");
            menuDelete.Name = "menuDelete";
            menuDelete.ShortcutKeyDisplayString = "Entf";
            menuDelete.Size = new Size(260, 22);
            menuDelete.Text = "Löschen";
            menuDelete.Click += MenuDelete_Click;
            //
            // menuSeparatorProperties
            //
            menuSeparatorProperties.Name = "menuSeparatorProperties";
            menuSeparatorProperties.Size = new Size(257, 6);
            //
            // menuProperties
            //
            menuProperties.Image = (Image)resources.GetObject("menuProperties.Image");
            menuProperties.Name = "menuProperties";
            menuProperties.ShortcutKeyDisplayString = "Alt+Eingabe";
            menuProperties.Size = new Size(260, 22);
            menuProperties.Text = "Eigenschaften";
            menuProperties.Click += MenuProperties_Click;
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
            projectMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ContextMenuStrip projectMenu;
        private ToolStripMenuItem menuOpen;
        private ToolStripMenuItem menuEdit;
        private ToolStripMenuItem menuShowInFolder;
        private ToolStripSeparator menuSeparatorFile;
        private ToolStripMenuItem menuRename;
        private ToolStripMenuItem menuDuplicate;
        private ToolStripMenuItem menuCopyPath;
        private ToolStripSeparator menuSeparatorDelete;
        private ToolStripMenuItem menuDelete;
        private ToolStripSeparator menuSeparatorProperties;
        private ToolStripMenuItem menuProperties;
        private ListView listProjects;
        private ColumnHeader columnName;
        private ColumnHeader columnFolder;
        private ColumnHeader columnModified;
        private ColumnHeader columnTempo;
        private ColumnHeader columnMeter;
        private ColumnHeader columnKit;
    }
}
