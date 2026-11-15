using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Drums4me.Classes;

/// <summary>Was die Übersicht mit einer Projektdatei tun kann: umbenennen, kopieren, löschen, im Ordner zeigen, Eigenschaften.</summary>
internal static partial class ProjectFiles
{
    public const string Extension = ".d4m";

    private const int SwShow = 5;
    private const int SwRestore = 9;
    private const uint SeeMaskClassName = 0x01;
    private const uint SeeMaskInvokeIdList = 0x0C;
    private const int ListerWaitSteps = 20;
    private const int ListerWaitMs = 100;

    private const uint FoDelete = 3;
    private const ushort FofSilent = 0x0004;
    private const ushort FofNoConfirmation = 0x0010;
    private const ushort FofAllowUndo = 0x0040;
    private const ushort FofNoErrorUi = 0x0400;
    private const ushort FofWantNukeWarning = 0x4000;

    /// <summary>Prüft den neuen Namen. Bei false steht in <paramref name="problem"/>, was nicht passt.</summary>
    public static bool TryGetRenameTarget(string path, string newName, out string target, out string problem)
    {
        target = path;
        problem = string.Empty;
        var name = newName.Trim();
        if (name.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
        {
            name = name[..^Extension.Length].TrimEnd();
        }

        if (name.Length == 0)
        {
            problem = Lng.T("Der Name darf nicht leer sein.");
            return false;
        }
        if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            problem = Lng.T("Ein Name darf keines dieser Zeichen enthalten:") + " \\ / : * ? \" < > |";
            return false;
        }
        if (name.EndsWith('.'))
        {
            problem = Lng.T("Ein Name darf nicht mit einem Punkt enden.");
            return false;
        }

        target = Path.Combine(Path.GetDirectoryName(path)!, name + Extension);

        // Nur die Groß- und Kleinschreibung zu ändern, ist erlaubt.
        if (File.Exists(target) && !string.Equals(target, path, StringComparison.OrdinalIgnoreCase))
        {
            problem = string.Format(Lng.T("In diesem Ordner gibt es schon ein Projekt „{0}“."), name);
            return false;
        }
        return true;
    }

    /// <summary>Ein freier Name nach dem Muster des Explorers: „Name - Kopie“, dann „Name - Kopie (2)“ und so weiter.</summary>
    public static string DuplicateTarget(string path)
    {
        var folder = Path.GetDirectoryName(path)!;
        var name = Path.GetFileNameWithoutExtension(path) + Lng.T(" - Kopie");
        var target = Path.Combine(folder, name + Extension);
        for (var number = 2; File.Exists(target); number++)
        {
            target = Path.Combine(folder, $"{name} ({number}){Extension}");
        }
        return target;
    }

    /// <summary>
    /// Verschiebt die Datei in den Papierkorb. Gefragt hat das Programm vorher selbst.
    /// Hat das Laufwerk keinen Papierkorb, warnt Windows, bevor die Datei endgültig verschwindet.
    /// </summary>
    /// <exception cref="OperationCanceledException">Diese Warnung wurde abgelehnt.</exception>
    /// <exception cref="IOException">Windows konnte die Datei nicht verschieben.</exception>
    public static void MoveToRecycleBin(IWin32Window owner, string path)
    {
        var from = Marshal.StringToHGlobalUni(path + '\0');   // die Liste der Dateien endet mit einer doppelten Null
        try
        {
            const ushort flags = FofSilent | FofNoConfirmation | FofAllowUndo | FofNoErrorUi | FofWantNukeWarning;
            int result;
            bool aborted;
            if (Environment.Is64BitProcess)
            {
                var operation = new FileOperation64 { Window = owner.Handle, Function = FoDelete, From = from, Flags = flags };
                result = SHFileOperation64(ref operation);
                aborted = operation.AnyOperationsAborted != 0;
            }
            else
            {
                var operation = new FileOperation32 { Window = owner.Handle, Function = FoDelete, From = from, Flags = flags };
                result = SHFileOperation32(ref operation);
                aborted = operation.AnyOperationsAborted != 0;
            }

            if (aborted)
            {
                throw new OperationCanceledException();
            }
            if (result != 0)
            {
                throw new IOException(string.Format(Lng.T("Windows meldet den Fehler 0x{0:X}. Vielleicht ist die Datei gerade in einem anderen Programm geöffnet."), result));
            }
        }
        finally
        {
            Marshal.FreeHGlobal(from);
        }
    }

    /// <summary>Zeigt die Datei im Dateimanager: in Directory Opus, falls installiert, sonst im Explorer. Wie in PDFlight.</summary>
    /// <exception cref="System.ComponentModel.Win32Exception">Der Dateimanager ließ sich nicht starten.</exception>
    public static void ShowInFileManager(string path)
    {
        var dopus = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"GPSoftware\Directory Opus\dopusrt.exe");
        if (File.Exists(dopus))
        {
            // „Go“ mit einem Dateipfad öffnet den Ordner und markiert die Datei.
            Process.Start(new ProcessStartInfo(dopus, $"/cmd Go \"{path}\""));
            BringDopusListerToFront();
        }
        else
        {
            Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{path}\""));
        }
    }

    /// <summary>
    /// dopusrt reicht den Befehl nur an das laufende Opus weiter, ein schon offenes Lister-Fenster bliebe dahinter.
    /// Kurz warten, bis es einen Lister gibt (Opus öffnet bei Bedarf erst einen), dann nach vorn holen.
    /// Das darf drums4me, solange es selbst vorn ist.
    /// </summary>
    private static void BringDopusListerToFront() => Task.Run(() =>
    {
        for (var step = 0; step < ListerWaitSteps; step++)
        {
            var lister = FindWindow("dopus.lister", null);
            if (lister != IntPtr.Zero)
            {
                if (IsIconic(lister))
                {
                    ShowWindow(lister, SwRestore);
                }
                SetForegroundWindow(lister);
                return;
            }
            Thread.Sleep(ListerWaitMs);
        }
    });

    /// <summary>
    /// Öffnet die Datei in dem Programm, das Windows für Textdateien (.txt) eingestellt hat, etwa Editor oder Notepad++.
    /// Für .d4m selbst ist drums4me zuständig, deshalb leiht sich der Aufruf die Zuordnung von .txt.
    /// </summary>
    /// <exception cref="System.ComponentModel.Win32Exception">Auch der Windows-Editor ließ sich nicht starten.</exception>
    public static void OpenInTextEditor(IWin32Window owner, string path)
    {
        var verb = Marshal.StringToHGlobalUni("open");
        var file = Marshal.StringToHGlobalUni(path);
        var textClass = Marshal.StringToHGlobalUni(".txt");
        try
        {
            var info = new ShellExecuteInfo
            {
                Size = Marshal.SizeOf<ShellExecuteInfo>(),
                Mask = SeeMaskClassName,
                Window = owner.Handle,
                Verb = verb,
                File = file,
                Class = textClass,
                Show = SwShow,
            };
            if (ShellExecuteEx(ref info))
            {
                return;
            }
            Debug.WriteLine($"Textzuordnung nicht nutzbar: Fehler {Marshal.GetLastPInvokeError()}");
        }
        finally
        {
            Marshal.FreeHGlobal(verb);
            Marshal.FreeHGlobal(file);
            Marshal.FreeHGlobal(textClass);
        }
        Process.Start(new ProcessStartInfo("notepad.exe", $"\"{path}\""));
    }

    /// <summary>Öffnet den Eigenschaften-Dialog von Windows für die Datei.</summary>
    public static void ShowProperties(IWin32Window owner, string path)
    {
        var verb = Marshal.StringToHGlobalUni("properties");
        var file = Marshal.StringToHGlobalUni(path);
        try
        {
            var info = new ShellExecuteInfo
            {
                Size = Marshal.SizeOf<ShellExecuteInfo>(),
                Mask = SeeMaskInvokeIdList,
                Window = owner.Handle,
                Verb = verb,
                File = file,
                Show = SwShow,
            };
            if (!ShellExecuteEx(ref info))
            {
                Debug.WriteLine($"Eigenschaften nicht geöffnet: Fehler {Marshal.GetLastPInvokeError()}");
            }
        }
        finally
        {
            Marshal.FreeHGlobal(verb);
            Marshal.FreeHGlobal(file);
        }
    }

    // Die Zeichenketten stehen als Zeiger in der Struktur, so bleibt sie für LibraryImport ohne Umwandlung übergebbar.
    [StructLayout(LayoutKind.Sequential)]
    private struct ShellExecuteInfo
    {
        public int Size;
        public uint Mask;
        public IntPtr Window;
        public IntPtr Verb;
        public IntPtr File;
        public IntPtr Parameters;
        public IntPtr Directory;
        public int Show;
        public IntPtr InstApp;
        public IntPtr IdList;
        public IntPtr Class;
        public IntPtr ClassKey;
        public uint HotKey;
        public IntPtr Icon;
        public IntPtr Process;
    }

    [LibraryImport("shell32.dll", EntryPoint = "ShellExecuteExW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShellExecuteEx(ref ShellExecuteInfo info);

    [LibraryImport("user32.dll", EntryPoint = "FindWindowW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr FindWindow(string className, string? windowName);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsIconic(IntPtr window);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(IntPtr window, int command);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetForegroundWindow(IntPtr window);

    // SHFILEOPSTRUCTW ist in 64 Bit natürlich ausgerichtet, in 32 Bit dicht gepackt.
    [StructLayout(LayoutKind.Sequential)]
    private struct FileOperation64
    {
        public IntPtr Window;
        public uint Function;
        public IntPtr From;
        public IntPtr To;
        public ushort Flags;
        public int AnyOperationsAborted;
        public IntPtr NameMappings;
        public IntPtr ProgressTitle;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct FileOperation32
    {
        public IntPtr Window;
        public uint Function;
        public IntPtr From;
        public IntPtr To;
        public ushort Flags;
        public int AnyOperationsAborted;
        public IntPtr NameMappings;
        public IntPtr ProgressTitle;
    }

    [LibraryImport("shell32.dll", EntryPoint = "SHFileOperationW")]
    private static partial int SHFileOperation64(ref FileOperation64 operation);

    [LibraryImport("shell32.dll", EntryPoint = "SHFileOperationW")]
    private static partial int SHFileOperation32(ref FileOperation32 operation);
}
