using System.ComponentModel;
using System.Text;

namespace WindowManagerCL.API;

/// <summary>
/// Base exception for all WindowManagerCL library errors.
/// </summary>
public abstract class WindowManagerException : Exception
{
    public IntPtr Handle { get; }
    public int? Win32ErrorCode { get; }
    public string? Win32ErrorMessage { get; }

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
