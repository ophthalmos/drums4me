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

    /// <summary>Der Wunsch gilt, bis er zurückgenommen wird.</summary>
    internal const uint ExecutionContinuous = 0x80000000;

    /// <summary>Windows soll den Bildschirm nicht abschalten.</summary>
    internal const uint ExecutionDisplayRequired = 0x00000002;

    /// <summary>Sagt Windows, was der aufrufende Thread gerade braucht. Der Wunsch endet mit dem Thread.</summary>
    [LibraryImport("kernel32.dll")]
    internal static partial uint SetThreadExecutionState(uint flags);
}
