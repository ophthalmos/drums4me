using Drums4me.Classes;
using System.Globalization;

namespace Drums4me.Controls;

/// <summary>
/// Die Projektübersicht: eine Tabelle aller Projekte, sortierbar über die Spaltenköpfe.
/// Sie ersetzt im Hauptfenster das Raster, solange die Karte „Datei“ gewählt ist. Suche und Filter liegen auf der Karte.
/// </summary>
public sealed partial class ProjectBrowser : UserControl
{
    private const int WeekdayDays = 6;   // sonst stünde derselbe Wochentag für heute und vor einer Woche
    private const string SortUp = " ▲";
    private const string SortDown = " ▼";

    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");

    private readonly Dictionary<ColumnHeader, string> _titles = [];
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
        foreach (ColumnHeader column in listProjects.Columns)
        {
            _titles[column] = column.Text;
        }
        _sortColumn = columnName;
        ShowSortMark();
    }

    /// <summary>Wurde ein Projekt zum Öffnen gewählt? Der Pfad steht im Ereignis.</summary>
    public event EventHandler<string>? ProjectChosen;

    /// <summary>Die Anzahl der gezeigten Projekte hat sich geändert. Der Text steht in <see cref="CountText"/>.</summary>
    public event EventHandler? CountChanged;

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
            ? (_entries.Count == 1 ? "1 Projekt" : $"{_entries.Count} Projekte")
            : $"{shown.Count} von {_entries.Count} Projekten";
        CountChanged?.Invoke(this, EventArgs.Empty);
        SizeNameColumn();
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
                : entry.Unreadable ? "(nicht lesbar)"
                : entry.KitMissing ? $"{entry.KitName} (fehlt)"
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
            0 => "Heute",
            1 => "Gestern",
            >= 2 and <= WeekdayDays => modified.ToString("dddd", German),
            _ => modified.ToString("dd.MM.yyyy"),
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

    private void SizeNameColumn()
    {
        var others = 0;
        foreach (ColumnHeader column in listProjects.Columns)
        {
            others += column == columnName ? 0 : column.Width;
        }
        // Platz für die Bildlaufleiste bleibt immer frei, sonst käme durch sie eine waagerechte hinzu.
        var free = listProjects.Width - SystemInformation.VerticalScrollBarWidth - others - 1;
        columnName.Width = Math.Max(LogicalToDeviceUnits(160), free);
    }

    public void OpenSelected()
    {
        if (listProjects.SelectedItems.Count > 0 && listProjects.SelectedItems[0].Tag is ProjectEntry entry)
        {
            ProjectChosen?.Invoke(this, entry.FilePath);
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

    private void ListProjects_ClientSizeChanged(object? sender, EventArgs e) => SizeNameColumn();
}
