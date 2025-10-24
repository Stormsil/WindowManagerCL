namespace WindowManagerCL.API;

/// <summary>
/// Represents the possible states of a window.
/// </summary>
public enum WindowState
{
    /// <summary>
    /// Window is in normal state (neither minimized nor maximized).
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Window is minimized to the taskbar.
    /// </summary>
    Minimized = 1,

    /// <summary>
    /// Window is maximized to fill the screen.
    /// </summary>
    Maximized = 2
}
