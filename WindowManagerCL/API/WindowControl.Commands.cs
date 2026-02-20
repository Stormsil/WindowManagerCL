using System.Runtime.InteropServices;
using WindowManagerCL.Infrastructure;

namespace WindowManagerCL.API;

public sealed partial class WindowControl
{
    /// <summary>
    /// Activates the window (brings to foreground and gives keyboard focus).
    /// </summary>
    public void Activate()
    {
        if (!WinApi.IsWindow(Handle))
            throw new InvalidWindowHandleException(Handle, "Window no longer exists");

        if (State == WindowState.Minimized)
        {
            WinApi.ShowWindow(Handle, WinApi.SW_RESTORE);
        }

        if (!WinApi.SetForegroundWindow(Handle))
        {
            int error = Marshal.GetLastWin32Error();
            if (error != 0)
                throw new WindowOperationException(Handle, "Activate", error);
        }
    }

    /// <summary>
    /// Minimizes the window to the taskbar.
    /// </summary>
    public void Minimize()
    {
        if (!WinApi.IsWindow(Handle))
            throw new InvalidWindowHandleException(Handle, "Window no longer exists");

        if (!WinApi.ShowWindow(Handle, WinApi.SW_MINIMIZE))
        {
            int error = Marshal.GetLastWin32Error();
            if (error != 0)
                throw new WindowOperationException(Handle, "Minimize", error);
        }
    }

    /// <summary>
    /// Maximizes the window to fill the screen.
    /// </summary>
    public void Maximize()
    {
        if (!WinApi.IsWindow(Handle))
            throw new InvalidWindowHandleException(Handle, "Window no longer exists");

        if (!WinApi.ShowWindow(Handle, WinApi.SW_MAXIMIZE))
        {
            int error = Marshal.GetLastWin32Error();
            if (error != 0)
                throw new WindowOperationException(Handle, "Maximize", error);
        }
    }

    /// <summary>
    /// Restores the window to normal state (neither minimized nor maximized).
    /// </summary>
    public void Restore()
    {
        if (!WinApi.IsWindow(Handle))
            throw new InvalidWindowHandleException(Handle, "Window no longer exists");

        if (!WinApi.ShowWindow(Handle, WinApi.SW_RESTORE))
        {
            int error = Marshal.GetLastWin32Error();
            if (error != 0)
                throw new WindowOperationException(Handle, "Restore", error);
        }
    }

    /// <summary>
    /// Closes the window by sending WM_CLOSE message.
    /// </summary>
    public void Close()
    {
        if (!WinApi.IsWindow(Handle))
            throw new InvalidWindowHandleException(Handle, "Window no longer exists");

        WinApi.SendMessage(Handle, WinApi.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
    }

    /// <summary>
    /// Moves the window to the specified screen coordinates.
    /// </summary>
    public void MoveTo(int x, int y)
    {
        if (!WinApi.IsWindow(Handle))
            throw new InvalidWindowHandleException(Handle, "Window no longer exists");

        if (!WinApi.SetWindowPos(Handle, IntPtr.Zero, x, y, 0, 0,
            WinApi.SWP_NOSIZE | WinApi.SWP_NOZORDER))
        {
            int error = Marshal.GetLastWin32Error();
            throw new WindowOperationException(Handle, "MoveTo", error);
        }
    }

    /// <summary>
    /// Resizes the window to the specified dimensions.
    /// </summary>
    public void Resize(int width, int height)
    {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be greater than 0");

        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be greater than 0");

        if (!WinApi.IsWindow(Handle))
            throw new InvalidWindowHandleException(Handle, "Window no longer exists");

        if (!WinApi.SetWindowPos(Handle, IntPtr.Zero, 0, 0, width, height,
            WinApi.SWP_NOMOVE | WinApi.SWP_NOZORDER))
        {
            int error = Marshal.GetLastWin32Error();
            throw new WindowOperationException(Handle, "Resize", error);
        }
    }

    /// <summary>
    /// Sets both position and size of the window in one operation.
    /// </summary>
    public void SetBounds(WindowBounds bounds)
    {
        if (!WinApi.IsWindow(Handle))
            throw new InvalidWindowHandleException(Handle, "Window no longer exists");

        if (!WinApi.SetWindowPos(Handle, IntPtr.Zero, bounds.X, bounds.Y, bounds.Width, bounds.Height,
            WinApi.SWP_NOZORDER))
        {
            int error = Marshal.GetLastWin32Error();
            throw new WindowOperationException(Handle, "SetBounds", error);
        }
    }
}
