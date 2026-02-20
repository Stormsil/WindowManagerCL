using System.Runtime.InteropServices;
using System.Text;
using WindowManagerCL.Infrastructure;

namespace WindowManagerCL.API;

public sealed partial class WindowControl
{
    /// <summary>
    /// Gets the window title (caption text).
    /// </summary>
    public string Title
    {
        get
        {
            var sb = new StringBuilder(256);
            int length = WinApi.GetWindowText(Handle, sb, sb.Capacity);

            if (length == 0)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                    throw new WindowOperationException(Handle, "GetTitle", error);
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Gets the window class name.
    /// </summary>
    public string ClassName
    {
        get
        {
            var sb = new StringBuilder(256);
            int length = WinApi.GetClassName(Handle, sb, sb.Capacity);

            if (length == 0)
            {
                int error = Marshal.GetLastWin32Error();
                throw new WindowOperationException(Handle, "GetClassName", error);
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Gets the process ID of the window's owning process.
    /// </summary>
    public uint ProcessId
    {
        get
        {
            if (!WinApi.GetWindowThreadProcessId(Handle, out uint processId))
            {
                int error = Marshal.GetLastWin32Error();
                throw new WindowOperationException(Handle, "GetProcessId", error);
            }

            return processId;
        }
    }

    /// <summary>
    /// Gets whether the window is currently visible.
    /// </summary>
    public bool IsVisible => WinApi.IsWindowVisible(Handle);

    /// <summary>
    /// Gets whether the window handle is still valid.
    /// </summary>
    public bool IsValid => WinApi.IsWindow(Handle);

    /// <summary>
    /// Gets the window position and size.
    /// </summary>
    public WindowBounds Bounds
    {
        get
        {
            if (!WinApi.GetWindowRect(Handle, out var rect))
            {
                int error = Marshal.GetLastWin32Error();
                throw new WindowOperationException(Handle, "GetBounds", error);
            }

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            return new WindowBounds(rect.Left, rect.Top, width, height);
        }
    }

    /// <summary>
    /// Gets the window state (Normal, Minimized, or Maximized).
    /// </summary>
    public WindowState State
    {
        get
        {
            var placement = new WinApi.WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);

            if (!WinApi.GetWindowPlacement(Handle, ref placement))
            {
                int error = Marshal.GetLastWin32Error();
                throw new WindowOperationException(Handle, "GetState", error);
            }

            return placement.showCmd switch
            {
                WinApi.SW_SHOWMINIMIZED => WindowState.Minimized,
                WinApi.SW_SHOWMAXIMIZED => WindowState.Maximized,
                _ => WindowState.Normal
            };
        }
    }

    /// <summary>
    /// Gets the parent window, or null if this is a top-level window.
    /// </summary>
    public WindowControl? Parent
    {
        get
        {
            IntPtr parentHandle = WinApi.GetParent(Handle);

            if (parentHandle == IntPtr.Zero)
                return null;

            try
            {
                return new WindowControl(parentHandle);
            }
            catch
            {
                return null;
            }
        }
    }
}
