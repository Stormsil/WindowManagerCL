namespace WindowManagerCL.API;

/// <summary>
/// Exception thrown when a window operation fails (move, resize, state change, etc.).
/// </summary>
public sealed class WindowOperationException : WindowManagerException
{
    public string Operation { get; }

    public WindowOperationException(
        IntPtr handle,
        string operation,
        int win32ErrorCode)
        : base($"Operation '{operation}' failed", handle, win32ErrorCode)
    {
        Operation = operation;
    }

    public WindowOperationException(
        IntPtr handle,
        string operation,
        string reason)
        : base($"Operation '{operation}' failed: {reason}", handle)
    {
        Operation = operation;
    }

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
