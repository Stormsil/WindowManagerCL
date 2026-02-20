using WindowManagerCL.Infrastructure;

namespace WindowManagerCL.API;

public sealed partial class WindowControl
{
    /// <summary>
    /// Enumerates all direct child windows (controls) of this window.
    /// </summary>
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
                continue;
            }

            yield return child;
        }
    }

    /// <summary>
    /// Finds a child window by class name and optional text.
    /// </summary>
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
}
