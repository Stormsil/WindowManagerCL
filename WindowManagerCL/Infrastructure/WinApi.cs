namespace WindowManagerCL.Infrastructure;

/// <summary>
/// P/Invoke declarations for Win32 User32.dll and Kernel32.dll window management APIs.
/// </summary>
internal static partial class WinApi
{
    /// <summary>
    /// Callback function for EnumWindows.
    /// </summary>
    /// <param name="hWnd">Window handle.</param>
    /// <param name="lParam">Application-defined value.</param>
    /// <returns>True to continue enumeration, false to stop.</returns>
    internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
}
