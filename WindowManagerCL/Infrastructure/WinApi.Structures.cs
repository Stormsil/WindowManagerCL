using System.Runtime.InteropServices;

namespace WindowManagerCL.Infrastructure;

internal static partial class WinApi
{
    /// <summary>
    /// RECT structure for window rectangles.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    /// <summary>
    /// WINDOWPLACEMENT structure for window placement information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public POINT ptMinPosition;
        public POINT ptMaxPosition;
        public RECT rcNormalPosition;
    }

    /// <summary>
    /// POINT structure for coordinates.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int X;
        public int Y;
    }
}
