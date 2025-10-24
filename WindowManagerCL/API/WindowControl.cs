using System.Runtime.InteropServices;
using System.Text;
using WindowManagerCL.Infrastructure;

namespace WindowManagerCL.API;

/// <summary>
/// Represents a Windows desktop window with methods for querying and manipulating its state.
/// </summary>
public sealed class WindowControl
{
    /// <summary>
    /// Gets the native window handle (HWND).
    /// </summary>
    /// <remarks>
    /// Exposed for advanced interop scenarios. Handle may become invalid if window closes.
    /// Use IsValid property to check handle validity before advanced operations.
    /// </remarks>
    public IntPtr Handle { get; }

    /// <summary>
    /// Internal constructor accepting IntPtr handle with validation.
    /// </summary>
    /// <param name="handle">Window handle to wrap.</param>
    /// <exception cref="InvalidWindowHandleException">Handle is IntPtr.Zero or invalid.</exception>
    internal WindowControl(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            throw new InvalidWindowHandleException(handle, "Handle is IntPtr.Zero");

        if (!WinApi.IsWindow(handle))
            throw new InvalidWindowHandleException(handle, "Handle does not reference a valid window");

        Handle = handle;
    }

    // ====================================================================
    // Properties (implemented as methods - always query Win32 for freshness)
    // ====================================================================

    /// <summary>
    /// Gets the window title (caption text).
    /// </summary>
    /// <returns>Current window title, or empty string if window has no title.</returns>
    /// <exception cref="WindowOperationException">
    /// Failed to retrieve title (e.g., access denied, window closed during operation).
    /// </exception>
    /// <remarks>
    /// Queries Win32 GetWindowText on each call - no caching.
    /// Maximum title length is 256 characters (Win32 limitation).
    /// </remarks>
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
    /// <returns>Window class name (e.g., "Notepad", "Chrome_WidgetWin_1").</returns>
    /// <exception cref="WindowOperationException">
    /// Failed to retrieve class name (e.g., access denied, window closed).
    /// </exception>
    /// <remarks>
    /// Queries Win32 GetClassName on each call.
    /// Class names are typically stable identifiers for window types.
    /// </remarks>
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
    /// <returns>Process ID (PID) as unsigned integer.</returns>
    /// <exception cref="WindowOperationException">
    /// Failed to retrieve process ID (window closed during operation).
    /// </exception>
    /// <remarks>
    /// Queries Win32 GetWindowThreadProcessId on each call.
    /// </remarks>
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
    /// <returns>True if window is visible; otherwise, false.</returns>
    /// <remarks>
    /// Queries Win32 IsWindowVisible on each call.
    /// Does not throw exceptions (returns false if window closed).
    /// </remarks>
    public bool IsVisible
    {
        get
        {
            return WinApi.IsWindowVisible(Handle);
        }
    }

    /// <summary>
    /// Gets whether the window handle is still valid.
    /// </summary>
    /// <returns>True if window exists; false if window closed or handle invalid.</returns>
    /// <remarks>
    /// Queries Win32 IsWindow on each call.
    /// Does not throw exceptions - safe to call on any handle.
    /// </remarks>
    public bool IsValid
    {
        get
        {
            return WinApi.IsWindow(Handle);
        }
    }

    /// <summary>
    /// Gets the window position and size.
    /// </summary>
    /// <returns>WindowBounds struct with X, Y, Width, Height.</returns>
    /// <exception cref="WindowOperationException">
    /// Failed to retrieve bounds (e.g., window closed, access denied).
    /// </exception>
    /// <remarks>
    /// Queries Win32 GetWindowRect on each call - always fresh data.
    /// Coordinates are in screen space (may be negative on multi-monitor setups).
    /// </remarks>
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
    /// <returns>Current WindowState enum value.</returns>
    /// <exception cref="WindowOperationException">
    /// Failed to retrieve state (window closed during operation).
    /// </exception>
    /// <remarks>
    /// Queries Win32 GetWindowPlacement on each call.
    /// </remarks>
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
    /// <returns>Parent WindowControl or null if no parent.</returns>
    /// <exception cref="WindowOperationException">
    /// Failed to retrieve parent (window closed during operation).
    /// </exception>
    /// <remarks>
    /// Queries Win32 GetParent on each call.
    /// Returns null for top-level windows and windows with no parent.
    /// </remarks>
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

    // ====================================================================
    // State Management Methods
    // ====================================================================

    /// <summary>
    /// Activates the window (brings to foreground and gives keyboard focus).
    /// </summary>
    /// <exception cref="InvalidWindowHandleException">Window is closed or handle invalid.</exception>
    /// <exception cref="WindowOperationException">
    /// Failed to activate window (e.g., access denied, window not responding).
    /// </exception>
    /// <remarks>
    /// Calls SetForegroundWindow internally.
    /// If window is minimized, restores it before activating.
    /// May fail if another application is in foreground and hasn't released focus.
    /// </remarks>
    public void Activate()
    {
        if (!WinApi.IsWindow(Handle))
            throw new InvalidWindowHandleException(Handle, "Window no longer exists");

        // If window is minimized, restore it first
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
    /// <exception cref="InvalidWindowHandleException">Window is closed or handle invalid.</exception>
    /// <exception cref="WindowOperationException">
    /// Failed to minimize window (e.g., access denied, window style prevents minimization).
    /// </exception>
    /// <remarks>
    /// Calls ShowWindow with SW_MINIMIZE internally.
    /// </remarks>
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
    /// <exception cref="InvalidWindowHandleException">Window is closed or handle invalid.</exception>
    /// <exception cref="WindowOperationException">
    /// Failed to maximize window (e.g., access denied, window style prevents maximization).
    /// </exception>
    /// <remarks>
    /// Calls ShowWindow with SW_MAXIMIZE internally.
    /// </remarks>
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
    /// <exception cref="InvalidWindowHandleException">Window is closed or handle invalid.</exception>
    /// <exception cref="WindowOperationException">Failed to restore window.</exception>
    /// <remarks>
    /// Calls ShowWindow with SW_RESTORE internally.
    /// Returns window to its previous size and position before minimize/maximize.
    /// </remarks>
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
    /// <exception cref="InvalidWindowHandleException">Window is closed or handle invalid.</exception>
    /// <exception cref="WindowOperationException">
    /// Failed to send close message (access denied).
    /// </exception>
    /// <remarks>
    /// Sends WM_CLOSE message via SendMessage.
    /// The window's application can choose to ignore or delay close (e.g., unsaved changes prompt).
    /// Method returns immediately - does not wait for window to actually close.
    /// Use IsValid property afterward to verify closure.
    /// </remarks>
    public void Close()
    {
        if (!WinApi.IsWindow(Handle))
            throw new InvalidWindowHandleException(Handle, "Window no longer exists");

        WinApi.SendMessage(Handle, WinApi.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
    }

    // ====================================================================
    // Position and Size Methods
    // ====================================================================

    /// <summary>
    /// Moves the window to the specified screen coordinates.
    /// </summary>
    /// <param name="x">New X coordinate (left edge) in screen space.</param>
    /// <param name="y">New Y coordinate (top edge) in screen space.</param>
    /// <exception cref="InvalidWindowHandleException">Window is closed or handle invalid.</exception>
    /// <exception cref="WindowOperationException">
    /// Failed to move window (e.g., access denied, window style prevents moving).
    /// </exception>
    /// <remarks>
    /// Calls SetWindowPos internally with SWP_NOSIZE flag.
    /// Coordinates can be negative for multi-monitor setups.
    /// Window size remains unchanged.
    /// </remarks>
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
    /// <param name="width">New width in pixels. Must be &gt; 0.</param>
    /// <param name="height">New height in pixels. Must be &gt; 0.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Width or height is less than or equal to 0.
    /// </exception>
    /// <exception cref="InvalidWindowHandleException">Window is closed or handle invalid.</exception>
    /// <exception cref="WindowOperationException">
    /// Failed to resize window (e.g., access denied, window style prevents resizing).
    /// </exception>
    /// <remarks>
    /// Calls SetWindowPos internally with SWP_NOMOVE flag.
    /// Window position remains unchanged.
    /// </remarks>
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
    /// <param name="bounds">New bounds (position and size).</param>
    /// <exception cref="InvalidWindowHandleException">Window is closed or handle invalid.</exception>
    /// <exception cref="WindowOperationException">
    /// Failed to set bounds (e.g., access denied).
    /// </exception>
    /// <remarks>
    /// Calls SetWindowPos internally without SWP_NOMOVE or SWP_NOSIZE flags.
    /// More efficient than calling MoveTo and Resize separately.
    /// </remarks>
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

    // ====================================================================
    // Hierarchy Navigation Methods
    // ====================================================================

    /// <summary>
    /// Enumerates all direct child windows (controls) of this window.
    /// </summary>
    /// <returns>
    /// Collection of child windows. Empty if window has no children.
    /// </returns>
    /// <exception cref="WindowOperationException">
    /// Failed to enumerate children (window closed during enumeration).
    /// </exception>
    /// <remarks>
    /// Uses EnumChildWindows internally.
    /// Returns only direct children, not grandchildren (non-recursive).
    /// For recursive enumeration, call GetChildren on each returned child.
    /// </remarks>
    public IEnumerable<WindowControl> GetChildren()
    {
        if (!WinApi.IsWindow(Handle))
            throw new WindowOperationException(Handle, "GetChildren", "Window no longer exists");

        var childHandles = WindowEnumerator.EnumerateChildWindows(Handle);

        foreach (var childHandle in childHandles)
        {
            WindowControl? child = null;
            try
            {
                child = new WindowControl(childHandle);
            }
            catch
            {
                // Skip invalid child handles
                continue;
            }

            yield return child;
        }
    }

    /// <summary>
    /// Finds a child window by class name and optional text.
    /// </summary>
    /// <param name="className">
    /// Child window class name. Empty string matches any class.
    /// </param>
    /// <param name="text">
    /// Child window text/caption. Null matches any text.
    /// </param>
    /// <returns>
    /// First child matching criteria, or null if no match found.
    /// </returns>
    /// <exception cref="ArgumentNullException">ClassName is null.</exception>
    /// <exception cref="WindowOperationException">
    /// Failed to search children (window closed during search).
    /// </exception>
    /// <remarks>
    /// Uses FindWindowEx internally.
    /// Search is case-insensitive.
    /// If multiple children match, returns first one found (order undefined).
    /// </remarks>
    public WindowControl? FindChild(string className, string? text = null)
    {
        if (className == null)
            throw new ArgumentNullException(nameof(className));

        if (!WinApi.IsWindow(Handle))
            throw new WindowOperationException(Handle, "FindChild", "Window no longer exists");

        IntPtr childHandle = WinApi.FindWindowEx(Handle, IntPtr.Zero, className, text);

        if (childHandle == IntPtr.Zero)
            return null;

        try
        {
            return new WindowControl(childHandle);
        }
        catch
        {
            return null;
        }
    }

    // ====================================================================
    // Object Overrides
    // ====================================================================

    /// <summary>
    /// Returns string representation of window (handle and title).
    /// </summary>
    /// <returns>String in format "WindowControl[0x12345678]: Title"</returns>
    public override string ToString()
    {
        try
        {
            return $"WindowControl[0x{Handle.ToString("X")}]: {Title}";
        }
        catch
        {
            return $"WindowControl[0x{Handle.ToString("X")}]: <invalid>";
        }
    }

    /// <summary>
    /// Determines whether two WindowControl instances refer to the same window.
    /// </summary>
    /// <param name="obj">Object to compare.</param>
    /// <returns>True if both refer to same HWND; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is WindowControl other && Handle == other.Handle;
    }

    /// <summary>
    /// Returns hash code based on window handle.
    /// </summary>
    /// <returns>Hash code derived from Handle property.</returns>
    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }
}
