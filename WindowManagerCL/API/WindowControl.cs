using WindowManagerCL.Infrastructure;

namespace WindowManagerCL.API;

/// <summary>
/// Represents a Windows desktop window with methods for querying and manipulating its state.
/// </summary>
public sealed partial class WindowControl
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
}
