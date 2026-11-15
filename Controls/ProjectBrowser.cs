using Drums4me.Classes;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Drums4me.Controls;

/// <summary>
/// Die Projektübersicht: eine Tabelle aller Projekte, sortierbar über die Spaltenköpfe.
/// Sie ersetzt im Hauptfenster das Raster, solange die Karte „Datei“ gewählt ist. Suche und Filter liegen auf der Karte.
/// </summary>
public sealed partial class ProjectBrowser : UserControl
{
    private const int WeekdayDays = 6;   // sonst stünde derselbe Wochentag für heute und vor einer Woche
    private const int MaxColumnWidth = 2000;
    private const int MinNameWidth = 160;
    private const int HeaderPadding = 14;
    private const string SortUp = " ▲";
    private const string SortDown = " ▼";


    private readonly Dictionary<ColumnHeader, string> _titles = [];
    private readonly Dictionary<string, int> _defaultWidths;
    private List<ProjectEntry> _entries = [];
    private ColumnHeader _sortColumn;
    private bool _sortDescending;
    private string? _selectedFile;
    private string _search = string.Empty;
    private string? _kit;
    private string? _meter;

    public ProjectBrowser()
    {
        InitializeComponent();
        Lng.Apply(this);
        Lng.Apply(projectMenu);
        foreach (ColumnHeader column in listProjects.Columns)
        {
            _titles[column] = column.Text;
        }
        _sortColumn = columnName;
        ShowSortMark();
        _defaultWidths = GetColumnWidths();
    }

    /// <summary>Wurde ein Projekt zum Öffnen gewählt? Der Pfad steht im Ereignis.</summary>
    public event EventHandler<string>? ProjectChosen;

    /// <summary>Die Anzahl der gezeigten Projekte hat sich geändert. Der Text steht in <see cref="CountText"/>.</summary>
    public event EventHandler? CountChanged;

    /// <summary>Ein Projekt soll einen neuen Namen bekommen. Das erledigt das Hauptfenster, denn es kennt das offene Projekt.</summary>
    public event EventHandler<ProjectRename>? RenameRequested;

    /// <summary>Ein Projekt soll kopiert werden. Der Pfad steht im Ereignis.</summary>
    public event EventHandler<string>? DuplicateRequested;

    /// <summary>Ein Projekt soll in den Papierkorb. Der Pfad steht im Ereignis.</summary>
    public event EventHandler<string>? DeleteRequested;

    /// <summary>Eine kurze Rückmeldung für die Statuszeile, etwa nach dem Kopieren des Pfads.</summary>
    public event EventHandler<string>? NoticeRequested;

    /// <summary>Solange ein Name bearbeitet wird, gehören Leertaste und Esc dem Eingabefeld.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsEditingName { get; private set; }

    public string CountText { get; private set; } = string.Empty;

    /// <summary>Die Kits, die in den Projekten vorkommen, für den Filter.</summary>
    public IEnumerable<string> KitNames => _entries.Where(entry => !entry.Unreadable).Select(entry => entry.KitName).Distinct(StringComparer.OrdinalIgnoreCase).Order(StringComparer.CurrentCultureIgnoreCase);

    /// <summary>Die Taktarten, die in den Projekten vorkommen, für den Filter.</summary>
    public IEnumerable<string> MeterNames => Meter.All.Select(meter => meter.Name).Where(name => _entries.Exists(entry => entry.Meters.Contains(name)));

    /// <summary>Übernimmt die Projekte und markiert das aktuelle. Sichtbar werden sie mit <see cref="SetFilter"/>.</summary>
    internal void SetEntries(List<ProjectEntry> entries, string? currentFile)
    {
        _entries = entries;
        _selectedFile = currentFile;

        var hasFolders = entries.Exists(entry => entry.Folder.Length > 0);
        if (hasFolders != listProjects.Columns.Contains(columnFolder))
        {
            if (hasFolders)
            {
                listProjects.Columns.Insert(1, columnFolder);
            }
            else
            {
                listProjects.Columns.Remove(columnFolder);
                if (_sortColumn == columnFolder)
                {
                    _sortColumn = columnName;
                    ShowSortMark();
                }
            }
        }
    }

    /// <summary>Null bei Kit und Taktart heißt: alle.</summary>
    public void SetFilter(string search, string? kit, string? meter)
    {
        _search = search.Trim();
        _kit = kit;
        _meter = meter;
        ApplyFilter();
    }

    /// <summary>Die Breiten aller Spalten, so wie du sie gezogen hast.</summary>
    public Dictionary<string, int> GetColumnWidths() => SavedColumns().ToDictionary(pair => pair.Key, pair => pair.Value.Width);

    /// <summary>Übernimmt gespeicherte Breiten. Unbekannte oder unsinnige Werte bleiben unbeachtet.</summary>
    public void SetColumnWidths(IReadOnlyDictionary<string, int>? widths)
    {
        if (widths is null)
        {
            return;
        }

        foreach (var (key, column) in SavedColumns())
        {
            // Zu schmale Werte stammen von einer Spalte, die früher ganz zugeschoben werden konnte. Dann gilt die Standardbreite.
            if (widths.TryGetValue(key, out var width) && width >= MinWidth(column) && width <= MaxColumnWidth)
            {
                column.Width = width;
            }
        }
    }

    /// <summary>Stellt die Breiten wieder her, mit denen die Liste ohne gespeicherte Einstellungen beginnt.</summary>
    public void ResetColumnWidths()
    {
        foreach (var (key, column) in SavedColumns())
        {
            column.Width = _defaultWidths[key];
        }
    }

    /// <summary>Die Schlüssel stehen in der Einstellungsdatei und dürfen sich deshalb nicht ändern.</summary>
    private Dictionary<string, ColumnHeader> SavedColumns() => new()
    {
        ["Name"] = columnName,
        ["Folder"] = columnFolder,
        ["Modified"] = columnModified,
        ["Tempo"] = columnTempo,
        ["Meter"] = columnMeter,
        ["Kit"] = columnKit,
    };

    /// <summary>Liefert false, wenn die Liste leer ist.</summary>
    public bool FocusList() => listProjects.Items.Count > 0 && listProjects.Focus();

    private void ApplyFilter()
    {
        var shown = _entries.Where(entry =>
                (_search.Length == 0 || entry.Name.Contains(_search, StringComparison.CurrentCultureIgnoreCase) || entry.Folder.Contains(_search, StringComparison.CurrentCultureIgnoreCase))
                && (_kit is null || string.Equals(entry.KitName, _kit, StringComparison.OrdinalIgnoreCase))
                && (_meter is null || entry.Meters.Contains(_meter)))
            .ToList();
        shown.Sort(Compare);

        listProjects.BeginUpdate();
        listProjects.Items.Clear();
        ListViewItem? select = null;
        foreach (var entry in shown)
        {
            var item = CreateItem(entry);
            listProjects.Items.Add(item);
            if (string.Equals(entry.FilePath, _selectedFile, StringComparison.OrdinalIgnoreCase))
            {
                select = item;
            }
        }
        select ??= listProjects.Items.Count > 0 ? listProjects.Items[0] : null;
        if (select is not null)
        {
            select.Selected = true;
            select.Focused = true;
            select.EnsureVisible();
        }
        listProjects.EndUpdate();

        CountText = shown.Count == _entries.Count
            ? (_entries.Count == 1 ? Lng.T("1 Projekt") : string.Format(Lng.T("{0} Projekte"), _entries.Count))
            : string.Format(Lng.T("{0} von {1} Projekten"), shown.Count, _entries.Count);
        CountChanged?.Invoke(this, EventArgs.Empty);
    }

    private ListViewItem CreateItem(ProjectEntry entry)
    {
        var item = new ListViewItem(entry.Name) { Tag = entry, UseItemStyleForSubItems = false };
        foreach (ColumnHeader column in listProjects.Columns)
        {
            if (column == columnName)
            {
                continue;
            }

            var text = column == columnFolder ? entry.Folder
                : column == columnModified ? ModifiedText(entry.Modified, DateTime.Now)
                : column == columnTempo ? (entry.Unreadable ? string.Empty : entry.Bpm.ToString())
                : column == columnMeter ? entry.MeterText
                : entry.Unreadable ? Lng.T("(nicht lesbar)")
                : entry.KitMissing ? string.Format(Lng.T("{0} (fehlt)"), entry.KitName)
                : entry.KitName;
            var subItem = item.SubItems.Add(text);
            if (column == columnKit && (entry.KitMissing || entry.Unreadable))
            {
                subItem.ForeColor = Theme.Error;
            }
        }
        return item;
    }

    /// <summary>Die letzte Woche liest man schneller als Wort. Sortiert wird weiterhin nach dem echten Zeitpunkt.</summary>
    private static string ModifiedText(DateTime modified, DateTime now)
    {
        var daysAgo = (now.Date - modified.Date).Days;
        var day = daysAgo switch
        {
            0 => Lng.T("Heute"),
            1 => Lng.T("Gestern"),
            >= 2 and <= WeekdayDays => modified.ToString("dddd", Lng.FormatCulture),
            _ => Lng.IsEnglish ? modified.ToString("d", CultureInfo.CurrentCulture) : modified.ToString("dd.MM.yyyy"),
        };
        return $"{day} {modified:HH:mm}";
    }

    private int Compare(ProjectEntry a, ProjectEntry b)
    {
        var result = 0;
        if (_sortColumn == columnFolder)
        {
            result = string.Compare(a.Folder, b.Folder, StringComparison.CurrentCultureIgnoreCase);
        }
        else if (_sortColumn == columnModified)
        {
            result = a.Modified.CompareTo(b.Modified);
        }
        else if (_sortColumn == columnTempo)
        {
            result = a.Bpm.CompareTo(b.Bpm);
        }
        else if (_sortColumn == columnMeter)
        {
            result = string.Compare(a.MeterText, b.MeterText, StringComparison.CurrentCultureIgnoreCase);
        }
        else if (_sortColumn == columnKit)
        {
            result = string.Compare(a.KitName, b.KitName, StringComparison.CurrentCultureIgnoreCase);
        }

        if (result == 0)
        {
            result = string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase);
            if (_sortColumn != columnName)
            {
                return result;   // gleiche Werte stehen immer alphabetisch
            }
        }
        return _sortDescending ? -result : result;
    }

    private void ShowSortMark()
    {
        foreach (var (column, title) in _titles)
        {
            column.Text = column == _sortColumn ? title + (_sortDescending ? SortDown : SortUp) : title;
        }
    }

    /// <summary>Jede Spalte bleibt so breit, dass ihr Titel samt Sortierpfeil lesbar ist. Eine Spalte mit Breite null wäre unauffindbar.</summary>
    private int MinWidth(ColumnHeader column) => column == columnName
        ? LogicalToDeviceUnits(MinNameWidth)
        : TextRenderer.MeasureText(_titles[column] + SortDown, listProjects.Font).Width + LogicalToDeviceUnits(HeaderPadding);

    public void OpenSelected()
    {
        if (SelectedEntry is { } entry)
        {
            ProjectChosen?.Invoke(this, entry.FilePath);
        }
    }

    /// <summary>Die Zeile darunter, in der letzten Zeile die darüber. Dorthin wandert die Markierung, wenn ein Projekt verschwindet.</summary>
    public string? NeighborOf(string path)
    {
        var paths = listProjects.Items.Cast<ListViewItem>().Select(item => ((ProjectEntry)item.Tag!).FilePath).ToList();
        var index = paths.FindIndex(other => string.Equals(other, path, StringComparison.OrdinalIgnoreCase));
        return index < 0 ? null
            : index + 1 < paths.Count ? paths[index + 1]
            : index > 0 ? paths[index - 1]
            : null;
    }

    private ProjectEntry? SelectedEntry => listProjects.SelectedItems.Count > 0 ? listProjects.SelectedItems[0].Tag as ProjectEntry : null;

    // ==================== Kontextmenü und Tasten ====================

    /// <summary>Ein Rechtsklick auf die freie Fläche unter den Zeilen hebt die Markierung auf. Ohne Projekt bleibt das Menü zu.</summary>
    private void ProjectMenu_Opening(object? sender, CancelEventArgs e) => e.Cancel = SelectedEntry is null;

    private void MenuOpen_Click(object? sender, EventArgs e) => OpenSelected();

    private void MenuShowInFolder_Click(object? sender, EventArgs e)
    {
        if (SelectedEntry is not { } entry)
        {
            return;
        }

        try
        {
            ProjectFiles.ShowInFileManager(entry.FilePath);
        }
        catch (Win32Exception ex)
        {
            Debug.WriteLine($"Dateimanager nicht gestartet: {ex.Message}");
        }
    }

    private void MenuEdit_Click(object? sender, EventArgs e) => OpenInEditor();

    private void OpenInEditor()
    {
        if (SelectedEntry is not { } entry)
        {
            return;
        }

        try
        {
            ProjectFiles.OpenInTextEditor((IWin32Window?)FindForm() ?? this, entry.FilePath);
        }
        catch (Win32Exception ex)
        {
            Dialogs.Error((IWin32Window?)FindForm() ?? this, Lng.T("Der Editor ließ sich nicht starten."), ex.Message);
        }
    }

    private void MenuCopyPath_Click(object? sender, EventArgs e) => CopyPath();

    /// <summary>Ohne Anführungszeichen, wie in PDFlight.</summary>
    private void CopyPath()
    {
        if (SelectedEntry is not { } entry)
        {
            return;
        }

        try
        {
            Clipboard.SetText(entry.FilePath);
            NoticeRequested?.Invoke(this, Lng.T("Der Dateipfad steht in der Zwischenablage."));
        }
        catch (ExternalException ex)
        {
            // Ein anderes Programm hält die Zwischenablage gerade fest.
            Dialogs.Error((IWin32Window?)FindForm() ?? this, Lng.T("Der Dateipfad ließ sich nicht kopieren."), ex.Message);
        }
    }

    private void MenuProperties_Click(object? sender, EventArgs e) => ShowProperties();

    private void ShowProperties()
    {
        if (SelectedEntry is { } entry)
        {
            ProjectFiles.ShowProperties((IWin32Window?)FindForm() ?? this, entry.FilePath);
        }
    }

    private void MenuRename_Click(object? sender, EventArgs e) => BeginRename();

    private void MenuDuplicate_Click(object? sender, EventArgs e)
    {
        if (SelectedEntry is { } entry)
        {
            DuplicateRequested?.Invoke(this, entry.FilePath);
        }
    }

    private void MenuDelete_Click(object? sender, EventArgs e)
    {
        if (SelectedEntry is { } entry)
        {
            DeleteRequested?.Invoke(this, entry.FilePath);
        }
    }

    private void ListProjects_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyData == Keys.F2)
        {
            BeginRename();
            e.Handled = true;
        }
        else if (e.KeyData == Keys.Delete && SelectedEntry is { } entry)
        {
            DeleteRequested?.Invoke(this, entry.FilePath);
            e.Handled = true;
        }
        else if (e.KeyData == (Keys.Control | Keys.E))
        {
            OpenInEditor();
            e.Handled = true;
        }
        else if (e.KeyData == (Keys.Control | Keys.Shift | Keys.C))
        {
            CopyPath();
            e.Handled = true;
        }
        else if (e.KeyData == (Keys.Alt | Keys.Enter))
        {
            ShowProperties();
            e.Handled = true;
            e.SuppressKeyPress = true;   // sonst piept Windows über die unbenutzte Alt-Taste
        }
    }

    private void BeginRename()
    {
        if (listProjects.SelectedItems.Count > 0)
        {
            listProjects.SelectedItems[0].BeginEdit();
        }
    }

    private void ListProjects_BeforeLabelEdit(object? sender, LabelEditEventArgs e) => IsEditingName = true;

    /// <summary>
    /// Umbenannt wird erst nach dem Ende der Bearbeitung, denn danach baut sich die Liste neu auf.
    /// Bis dahin zeigt die Zeile den alten Namen, bei einem Fehler bleibt es dabei.
    /// </summary>
    private void ListProjects_AfterLabelEdit(object? sender, LabelEditEventArgs e)
    {
        IsEditingName = false;
        e.CancelEdit = true;
        if (e.Label is { } label && listProjects.Items[e.Item].Tag is ProjectEntry entry && label != entry.Name)
        {
            BeginInvoke(() => RenameRequested?.Invoke(this, new ProjectRename(entry.FilePath, label)));
        }
    }

    private void ListProjects_ColumnClick(object? sender, ColumnClickEventArgs e)
    {
        var column = listProjects.Columns[e.Column];
        if (column == _sortColumn)
        {
            _sortDescending = !_sortDescending;
        }
        else
        {
            _sortColumn = column;

            // Bei Datum und Tempo interessiert zuerst das Neueste und Schnellste
            _sortDescending = column == columnModified || column == columnTempo;
        }

        if (listProjects.SelectedItems.Count > 0 && listProjects.SelectedItems[0].Tag is ProjectEntry entry)
        {
            _selectedFile = entry.FilePath;
        }
        ShowSortMark();
        ApplyFilter();
    }

    private void ListProjects_ItemActivate(object? sender, EventArgs e) => OpenSelected();

    private void ListProjects_ColumnWidthChanging(object? sender, ColumnWidthChangingEventArgs e)
    {
        var column = listProjects.Columns[e.ColumnIndex];
        var min = MinWidth(column);
        if (e.NewWidth < min)
        {
            // Abbrechen hält die Kante beim Ziehen an. Nur NewWidth zu setzen, würde der Spaltenkopf nicht übernehmen.
            e.Cancel = true;
            e.NewWidth = Math.Max(column.Width, min);
        }
    }
}

/// <summary>Der neue Name ist so, wie er eingegeben wurde. Geprüft wird beim Umbenennen.</summary>
public sealed record ProjectRename(string FilePath, string NewName);
