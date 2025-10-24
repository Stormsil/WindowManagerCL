using System.Runtime.InteropServices;

namespace WindowManagerCL.Infrastructure;

/// <summary>
/// Internal helper for enumerating windows using Win32 EnumWindows API.
/// </summary>
internal static class WindowEnumerator
{
    /// <summary>
    /// Enumerates all top-level windows currently open on the desktop.
    /// </summary>
    /// <returns>Collection of window handles (HWND).</returns>
    internal static List<IntPtr> EnumerateWindows()
    {
        var windows = new List<IntPtr>();
        var gcHandle = GCHandle.Alloc(windows);

        try
        {
            WinApi.EnumWindows((hWnd, lParam) =>
            {
                var handle = GCHandle.FromIntPtr(lParam);
                var list = (List<IntPtr>)handle.Target!;
                list.Add(hWnd);
                return true; // Continue enumeration
            }, GCHandle.ToIntPtr(gcHandle));

            // Check for enumeration errors
            if (Marshal.GetLastWin32Error() != 0)
            {
                throw new InvalidOperationException(
                    $"EnumWindows failed with error code: {Marshal.GetLastWin32Error()}");
            }
        }
        finally
        {
            if (gcHandle.IsAllocated)
                gcHandle.Free();
        }

        return windows;
    }

    /// <summary>
    /// Enumerates all child windows of the specified parent window.
    /// </summary>
    /// <param name="hWndParent">Handle to parent window.</param>
    /// <returns>Collection of child window handles.</returns>
    internal static List<IntPtr> EnumerateChildWindows(IntPtr hWndParent)
    {
        var children = new List<IntPtr>();
        var gcHandle = GCHandle.Alloc(children);

        try
        {
            WinApi.EnumChildWindows(hWndParent, (hWnd, lParam) =>
            {
                var handle = GCHandle.FromIntPtr(lParam);
                var list = (List<IntPtr>)handle.Target!;
                list.Add(hWnd);
                return true; // Continue enumeration
            }, GCHandle.ToIntPtr(gcHandle));

            // Check for enumeration errors
            int error = Marshal.GetLastWin32Error();
            if (error != 0 && error != 127) // 127 = ERROR_PROC_NOT_FOUND (can occur for some windows)
            {
                throw new InvalidOperationException(
                    $"EnumChildWindows failed with error code: {error}");
            }
        }
        finally
        {
            if (gcHandle.IsAllocated)
                gcHandle.Free();
        }

        return children;
    }
}
