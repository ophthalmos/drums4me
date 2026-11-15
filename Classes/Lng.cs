using System.Globalization;
using System.Resources;

namespace Drums4me.Classes;

/// <summary>
/// Mehrsprachigkeit (Deutsch und Englisch), übernommen aus PDFlight: Deutsch ist einkompiliert und bleibt der Rückfall
/// für jeden fehlenden Eintrag; Englisch liegt als Languages\lng.en.resx daneben, wobei der SCHLÜSSEL jedes Eintrags
/// der deutsche Text selbst ist. Ändert sich ein deutscher Text, verfällt seine Übersetzung dadurch automatisch, bis
/// die resx nachgezogen ist. Bis dahin erscheint der Text auf Deutsch, es bricht also nichts. check-lng.ps1 meldet das.
/// </summary>
internal static class Lng
{
    private static readonly ResourceManager Resources = new("Drums4me.Languages.lng", typeof(Lng).Assembly);
    private static CultureInfo? _culture;   // null = Deutsch, keine Übersetzung nötig

    /// <summary>Der gewählte Kultur-Code, "de" oder "en".</summary>
    public static string CultureCode { get; private set; } = "de";

    /// <summary>Ist Englisch eingestellt?</summary>
    public static bool IsEnglish => _culture is not null;

    /// <summary>Kultur für Datumsangaben und Wochentage in der Programmsprache.</summary>
    public static CultureInfo FormatCulture => _culture is null ? CultureInfo.GetCultureInfo("de-DE") : CultureInfo.GetCultureInfo("en-US");

    public static void Initialize(string? cultureCode)
    {
        CultureCode = cultureCode == "en" ? "en" : "de";
        _culture = CultureCode == "de" ? null : CultureInfo.GetCultureInfo(CultureCode);
    }

    /// <summary>Die Sprache für eine Erstinstallation ohne Vorgabe des Setups: Deutsch nur bei deutschem Windows.</summary>
    public static string DefaultCultureCode => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "de" : "en";

    /// <summary>Übersetzt einen deutschen Text. Ohne Eintrag oder auf Deutsch kommt er unverändert zurück.</summary>
    [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(nameof(german))]
    public static string? T(string? german)
    {
        if (_culture is null || string.IsNullOrEmpty(german))
        {
            return german;
        }

        try
        {
            return Resources.GetString(german, _culture) ?? german;
        }
        catch (MissingManifestResourceException)
        {
            return german;
        }
    }

    /// <summary>
    /// Für mehrzeilige Texte: Nachschlag über einen festen Schlüssel, denn Zeilenumbrüche taugen nicht als resx-Schlüssel.
    /// </summary>
    [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(nameof(german))]
    public static string? T(string key, string? german)
    {
        if (_culture is null)
        {
            return german;
        }

        try
        {
            return Resources.GetString(key, _culture) ?? german;
        }
        catch (MissingManifestResourceException)
        {
            return german;
        }
    }

    /// <summary>
    /// Übersetzt alle Texte eines Formulars oder Steuerelements: Beschriftungen, Statuszeilen-Hinweise
    /// (AccessibleDescription), Namen für Screenreader, Tooltips, Menüs, Spaltenköpfe und Reiter-Tooltips.
    /// Einmal direkt nach InitializeComponent aufrufen.
    /// </summary>
    public static void Apply(Control root, ToolTip? toolTip = null)
    {
        if (_culture is null)
        {
            return;
        }

        TranslateControl(root, toolTip);
        TranslateChildren(root, toolTip);
    }

    /// <summary>Übersetzt ein einzelnes Menü. Kontextmenüs hängen nicht im Steuerelement-Baum des Formulars.</summary>
    public static void Apply(ToolStrip strip)
    {
        if (_culture is null)
        {
            return;
        }

        foreach (ToolStripItem item in strip.Items)
        {
            TranslateItem(item);
        }
    }

    private static void TranslateChildren(Control parent, ToolTip? toolTip)
    {
        foreach (Control child in parent.Controls)
        {
            TranslateControl(child, toolTip);
            if (child is ToolStrip strip)   // deckt auch die Statuszeile ab
            {
                foreach (ToolStripItem item in strip.Items)
                {
                    TranslateItem(item);
                }
            }
            if (child is ListView listView)   // Spaltenköpfe sind keine Steuerelemente
            {
                foreach (ColumnHeader column in listView.Columns)
                {
                    column.Text = T(column.Text);
                }
            }
            if (child is ComboBox { DataSource: null, Items.Count: > 0 } combo && combo.Items[0] is string)
            {
                for (var index = 0; index < combo.Items.Count; index++)
                {
                    if (combo.Items[index] is string item)
                    {
                        combo.Items[index] = T(item);
                    }
                }
            }
            TranslateChildren(child, toolTip);
        }
    }

    private static void TranslateControl(Control control, ToolTip? toolTip)
    {
        control.Text = T(control.Text);
        control.AccessibleDescription = T(control.AccessibleDescription);
        control.AccessibleName = T(control.AccessibleName);
        if (control is TabPage page)
        {
            page.ToolTipText = T(page.ToolTipText);
        }
        if (control is TextBox textBox)
        {
            textBox.PlaceholderText = T(textBox.PlaceholderText);
        }
        if (toolTip?.GetToolTip(control) is { Length: > 0 } tip)
        {
            toolTip.SetToolTip(control, T(tip));
        }
    }

    private static void TranslateItem(ToolStripItem item)
    {
        item.Text = T(item.Text);
        item.ToolTipText = T(item.ToolTipText);
        item.AccessibleDescription = T(item.AccessibleDescription);
        if (item is ToolStripMenuItem menuItem)
        {
            menuItem.ShortcutKeyDisplayString = T(menuItem.ShortcutKeyDisplayString);
        }
        if (item is ToolStripDropDownItem dropDown)
        {
            foreach (ToolStripItem child in dropDown.DropDownItems)
            {
                TranslateItem(child);
            }
        }
    }
}
