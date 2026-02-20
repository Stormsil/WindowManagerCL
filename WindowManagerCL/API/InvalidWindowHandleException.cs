namespace WindowManagerCL.API;

/// <summary>
/// Exception thrown when a window handle is invalid or window has been closed.
/// </summary>
public sealed class InvalidWindowHandleException : WindowManagerException
{
    public string Reason { get; }

    public InvalidWindowHandleException(IntPtr handle, string reason)
        : base($"Invalid window handle: {reason}", handle)
    {
        Reason = reason;
    }

    public InvalidWindowHandleException(
        IntPtr handle,
        string reason,
        int win32ErrorCode)
        : base($"Invalid window handle: {reason}", handle, win32ErrorCode)
    {
        Reason = reason;
    }

    public InvalidWindowHandleException(
        IntPtr handle,
        string reason,
        Exception innerException)
        : base($"Invalid window handle: {reason}", handle, null, innerException)
    {
        Reason = reason;
    }
}
