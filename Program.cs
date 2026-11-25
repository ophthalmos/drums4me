using System.Diagnostics;
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
        var settings = AppSettings.Load();
        Application.SetColorMode(settings.ColorMode);
        Lng.Initialize(settings.Language);   // vor dem ersten Fenster, denn die Formulare übersetzen sich beim Erstellen

        var startFile = StartFileFrom(args);
        FrmDrums4me form;
        using (var instance = new SingleInstance())
        {
            if (!instance.IsFirst && SingleInstance.TrySendToFirst(startFile))
            {
                return;
            }

            form = new FrmDrums4me(startFile);
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

        // Erst jetzt, ohne Sperre und Pipe: Sonst hielte sich der neue Start für einen zweiten und gäbe an diesen ab.
        if (form.RestartRequested)
        {
            Restart(form.OpenFile);
        }
    }

    private static void Restart(string? file)
    {
        var start = new ProcessStartInfo(Environment.ProcessPath!) { UseShellExecute = false };
        if (file is not null && File.Exists(file))
        {
            start.ArgumentList.Add(file);
        }
        Process.Start(start);
    }

    /// <summary>Der Dateipfad aus der Befehlszeile, wie ihn Windows beim Doppelklick auf eine .d4mx- oder .d4mt-Datei übergibt.</summary>
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
