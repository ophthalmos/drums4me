using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Drums4me.Classes;

/// <summary>Die wenigen Windows-Funktionen, für die es in .NET keine Entsprechung gibt.</summary>
internal static partial class NativeMethods
{
    /// <summary>Ein offener Dialog sperrt sein Besitzerfenster nur hier, Control.Enabled bleibt davon unberührt.</summary>
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool IsWindowEnabled(IntPtr window);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetNamedPipeServerProcessId(SafePipeHandle pipe, out uint serverProcessId);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool AllowSetForegroundWindow(uint processId);
}
