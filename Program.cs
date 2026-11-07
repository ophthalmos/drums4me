using Drums4me.Classes;
using Drums4me.Forms;

namespace Drums4me;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.SetColorMode(AppSettings.Load().ColorMode);

        var startFile = StartFileFrom(args);
        using var instance = new SingleInstance();
        if (!instance.IsFirst && SingleInstance.TrySendToFirst(startFile))
        {
            return;
        }

        var form = new FrmDrums4me(startFile);
        if (instance.IsFirst)
        {
            form.Shown += (_, _) => instance.Listen(path =>
            {
                if (form.IsHandleCreated && !form.IsDisposed)
                {
                    form.BeginInvoke(() => form.HandleExternalOpen(path));
                }
            });
        }
        Application.Run(form);
    }

    /// <summary>Der Dateipfad aus der Befehlszeile, wie ihn Windows beim Doppelklick auf eine .d4m-Datei übergibt.</summary>
    private static string? StartFileFrom(string[] args)
    {
        if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
        {
            return null;
        }

        try
        {
            return Path.GetFullPath(args[0]);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return args[0];
        }
    }
}
