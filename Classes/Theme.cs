using Microsoft.Win32;

namespace Drums4me.Classes;

/// <summary>Farben, für die es keine passende Systemfarbe gibt und die im Dunkelmodus anders ausfallen müssen.</summary>
internal static class Theme
{
    /// <summary>Für Eingabefehler. Das helle Rot bleibt auch auf dunklem Grund gut lesbar.</summary>
    public static Color Error => Application.IsDarkModeEnabled ? Color.FromArgb(255, 150, 140) : Color.Firebrick;

    /// <summary>
    /// Die Akzentfarbe von Windows, in der auch die Schalter erscheinen. Dafür gibt es keine Systemfarbe:
    /// SystemColors.Highlight ist ein anderes Blau.
    /// </summary>
    public static Color Accent
    {
        get
        {
            // Windows legt sie hier als 0xAABBGGRR ab. Die Abfrage über DwmGetColorizationColor liefert einen leicht
            // dunkleren Ton als den der Schalter (gemessen 0,91,163 statt 0,99,177).
            try
            {
                if (Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "AccentColor", null) is int stored)
                {
                    return Color.FromArgb(stored & 0xFF, (stored >> 8) & 0xFF, (stored >> 16) & 0xFF);
                }
            }
            catch (Exception ex) when (ex is System.Security.SecurityException or IOException or UnauthorizedAccessException)
            {
                // Dann eben das Blau, das Windows für Markierungen nimmt.
            }
            return SystemColors.Highlight;
        }
    }
}
