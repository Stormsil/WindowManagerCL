namespace WindowManagerCL.API;

public static partial class Window
{
    public static WindowControl FromHandle(IntPtr handle)
    {
        return new WindowControl(handle);
    }

    public static WindowControl? GetForegroundWindow()
    {
        var handle = Infrastructure.WinApi.GetForegroundWindow();

        if (handle == IntPtr.Zero)
            return null;

        try
        {
            return new WindowControl(handle);
        }
        catch (InvalidWindowHandleException)
        {
            return null;
        }
    }
}
