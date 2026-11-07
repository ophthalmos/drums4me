namespace Drums4me.Classes;

/// <summary>Farben, für die es keine passende Systemfarbe gibt und die im Dunkelmodus anders ausfallen müssen.</summary>
internal static class Theme
{
    /// <summary>Für Eingabefehler. Das helle Rot bleibt auch auf dunklem Grund gut lesbar.</summary>
    public static Color Error => Application.IsDarkModeEnabled ? Color.FromArgb(255, 150, 140) : Color.Firebrick;
}
