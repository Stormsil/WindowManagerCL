namespace WindowManagerCL.Infrastructure;

internal static partial class WinApi
{
    internal const int SW_HIDE = 0;
    internal const int SW_SHOWNORMAL = 1;
    internal const int SW_NORMAL = 1;
    internal const int SW_SHOWMINIMIZED = 2;
    internal const int SW_SHOWMAXIMIZED = 3;
    internal const int SW_MAXIMIZE = 3;
    internal const int SW_SHOWNOACTIVATE = 4;
    internal const int SW_SHOW = 5;
    internal const int SW_MINIMIZE = 6;
    internal const int SW_SHOWMINNOACTIVE = 7;
    internal const int SW_SHOWNA = 8;
    internal const int SW_RESTORE = 9;
    internal const int SW_SHOWDEFAULT = 10;
    internal const int SW_FORCEMINIMIZE = 11;

    internal const uint SWP_NOSIZE = 0x0001;
    internal const uint SWP_NOMOVE = 0x0002;
    internal const uint SWP_NOZORDER = 0x0004;
    internal const uint SWP_NOREDRAW = 0x0008;
    internal const uint SWP_NOACTIVATE = 0x0010;
    internal const uint SWP_FRAMECHANGED = 0x0020;
    internal const uint SWP_SHOWWINDOW = 0x0040;
    internal const uint SWP_HIDEWINDOW = 0x0080;
    internal const uint SWP_NOCOPYBITS = 0x0100;
    internal const uint SWP_NOOWNERZORDER = 0x0200;
    internal const uint SWP_NOSENDCHANGING = 0x0400;

    internal const uint WM_CLOSE = 0x0010;
}
