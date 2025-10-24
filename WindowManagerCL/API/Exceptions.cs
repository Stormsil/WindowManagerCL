using System.ComponentModel;
using System.Text;

namespace WindowManagerCL.API;

/// <summary>
/// Base exception for all WindowManagerCL library errors.
/// </summary>
public abstract class WindowManagerException : Exception
{
    /// <summary>
    /// Gets the window handle (HWND) associated with this exception.
    /// May be IntPtr.Zero if exception is not associated with a specific window.
    /// </summary>
    public IntPtr Handle { get; }

    /// <summary>
    /// Gets the Win32 error code from GetLastWin32Error(), if applicable.
    /// Null if no Win32 error occurred or error code not captured.
    /// </summary>
    public int? Win32ErrorCode { get; }

    /// <summary>
    /// Gets the Win32 error message corresponding to Win32ErrorCode, if available.
    /// Null if Win32ErrorCode is null.
    /// </summary>
    public string? Win32ErrorMessage { get; }

    /// <summary>
    /// Initializes a new instance of WindowManagerException.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="handle">Window handle associated with exception.</param>
    /// <param name="win32ErrorCode">Win32 error code, if applicable.</param>
    protected WindowManagerException(
        string message,
        IntPtr handle = default,
        int? win32ErrorCode = null)
        : base(FormatMessage(message, handle, win32ErrorCode))
    {
        Handle = handle;
        Win32ErrorCode = win32ErrorCode;
        Win32ErrorMessage = win32ErrorCode.HasValue ? new Win32Exception(win32ErrorCode.Value).Message : null;
    }

    /// <summary>
    /// Initializes a new instance with inner exception.
    /// </summary>
    protected WindowManagerException(
        string message,
        IntPtr handle,
        int? win32ErrorCode,
        Exception innerException)
        : base(FormatMessage(message, handle, win32ErrorCode), innerException)
    {
        Handle = handle;
        Win32ErrorCode = win32ErrorCode;
        Win32ErrorMessage = win32ErrorCode.HasValue ? new Win32Exception(win32ErrorCode.Value).Message : null;
    }

    private static string FormatMessage(string message, IntPtr handle, int? win32ErrorCode)
    {
        var sb = new StringBuilder(message);

        if (handle != IntPtr.Zero)
            sb.Append($" [HWND: 0x{handle.ToString("X")}]");

        if (win32ErrorCode.HasValue)
        {
            var win32Msg = new Win32Exception(win32ErrorCode.Value).Message;
            sb.Append($" [Win32: {win32ErrorCode.Value} - {win32Msg}]");
        }

        return sb.ToString();
    }
}

/// <summary>
/// Exception thrown when a window search operation finds no matching windows.
/// </summary>
public sealed class WindowNotFoundException : WindowManagerException
{
    /// <summary>
    /// Gets the search criteria that produced no results.
    /// </summary>
    public string SearchCriteria { get; }

    /// <summary>
    /// Initializes a new instance of WindowNotFoundException.
    /// </summary>
    /// <param name="searchCriteria">Description of what was searched (e.g., "Title: Notepad").</param>
    public WindowNotFoundException(string searchCriteria)
        : base($"No window found matching criteria: {searchCriteria}")
    {
        SearchCriteria = searchCriteria;
    }

    /// <summary>
    /// Initializes a new instance with inner exception.
    /// </summary>
    public WindowNotFoundException(string searchCriteria, Exception innerException)
        : base($"No window found matching criteria: {searchCriteria}",
               IntPtr.Zero, null, innerException)
    {
        SearchCriteria = searchCriteria;
    }
}

/// <summary>
/// Exception thrown when a window operation fails (move, resize, state change, etc.).
/// </summary>
public sealed class WindowOperationException : WindowManagerException
{
    /// <summary>
    /// Gets the name of the operation that failed.
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Initializes a new instance of WindowOperationException.
    /// </summary>
    /// <param name="handle">Window handle on which operation failed.</param>
    /// <param name="operation">Name of the operation (e.g., "Activate", "Resize").</param>
    /// <param name="win32ErrorCode">Win32 error code from GetLastWin32Error().</param>
    public WindowOperationException(
        IntPtr handle,
        string operation,
        int win32ErrorCode)
        : base($"Operation '{operation}' failed", handle, win32ErrorCode)
    {
        Operation = operation;
    }

    /// <summary>
    /// Initializes a new instance without Win32 error code.
    /// </summary>
    /// <param name="handle">Window handle on which operation failed.</param>
    /// <param name="operation">Name of the operation.</param>
    /// <param name="reason">Human-readable reason for failure.</param>
    public WindowOperationException(
        IntPtr handle,
        string operation,
        string reason)
        : base($"Operation '{operation}' failed: {reason}", handle)
    {
        Operation = operation;
    }

    /// <summary>
    /// Initializes a new instance with inner exception.
    /// </summary>
    public WindowOperationException(
        IntPtr handle,
        string operation,
        int win32ErrorCode,
        Exception innerException)
        : base($"Operation '{operation}' failed", handle, win32ErrorCode, innerException)
    {
        Operation = operation;
    }
}

/// <summary>
/// Exception thrown when a window handle is invalid or window has been closed.
/// </summary>
public sealed class InvalidWindowHandleException : WindowManagerException
{
    /// <summary>
    /// Gets the reason why the handle is invalid.
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// Initializes a new instance of InvalidWindowHandleException.
    /// </summary>
    /// <param name="handle">The invalid window handle.</param>
    /// <param name="reason">Reason why handle is invalid (e.g., "Window closed", "Handle is zero").</param>
    public InvalidWindowHandleException(IntPtr handle, string reason)
        : base($"Invalid window handle: {reason}", handle)
    {
        Reason = reason;
    }

    /// <summary>
    /// Initializes a new instance with Win32 error code.
    /// </summary>
    public InvalidWindowHandleException(
        IntPtr handle,
        string reason,
        int win32ErrorCode)
        : base($"Invalid window handle: {reason}", handle, win32ErrorCode)
    {
        Reason = reason;
    }

    /// <summary>
    /// Initializes a new instance with inner exception.
    /// </summary>
    public InvalidWindowHandleException(
        IntPtr handle,
        string reason,
        Exception innerException)
        : base($"Invalid window handle: {reason}", handle, null, innerException)
    {
        Reason = reason;
    }
}
