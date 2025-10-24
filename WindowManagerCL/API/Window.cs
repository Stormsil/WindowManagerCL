using WindowManagerCL.Core;

namespace WindowManagerCL.API;

/// <summary>
/// Provides static methods for discovering and searching Windows desktop windows.
/// This is the primary entry point for window management operations.
/// </summary>
public static class Window
{
    /// <summary>
    /// Enumerates all top-level windows currently open on the desktop.
    /// </summary>
    /// <returns>Collection of all top-level windows. May be empty if no windows exist.</returns>
    /// <remarks>
    /// This method uses EnumWindows internally and returns all windows including hidden ones.
    /// Use LINQ filtering or WindowControl.IsVisible property to filter visible windows only.
    /// Performance: O(n) where n is number of top-level windows (typically &lt; 100).
    /// </remarks>
    public static IEnumerable<WindowControl> FindAll()
    {
        return WindowFinder.FindAll();
    }

    /// <summary>
    /// Finds a window by its title with exact or partial matching.
    /// </summary>
    /// <param name="title">Window title to search for. Empty string matches windows with no title.</param>
    /// <param name="exact">
    /// If true, title must match exactly (case-insensitive).
    /// If false, title can be a substring (case-insensitive).
    /// </param>
    /// <returns>The first window matching the criteria.</returns>
    /// <exception cref="WindowNotFoundException">No window found with the specified title.</exception>
    /// <exception cref="ArgumentNullException">Title is null.</exception>
    /// <remarks>
    /// If multiple windows match, returns the first one found (order is undefined).
    /// Use FindByTitleRegex for pattern matching or FindAll().Where() for custom filtering.
    /// </remarks>
    public static WindowControl FindByTitle(string title, bool exact = true)
    {
        if (title == null)
            throw new ArgumentNullException(nameof(title));

        var windows = WindowFinder.FindAll();
        var filtered = WindowFinder.FilterByTitle(windows, title, exact);
        var result = filtered.FirstOrDefault();

        if (result == null)
        {
            string criteria = exact
                ? $"Title (exact): '{title}'"
                : $"Title (partial): '{title}'";
            throw new WindowNotFoundException(criteria);
        }

        return result;
    }

    /// <summary>
    /// Attempts to find a window by title without throwing exceptions.
    /// </summary>
    /// <param name="title">Window title to search for.</param>
    /// <param name="exact">If true, requires exact match; if false, allows partial match.</param>
    /// <param name="window">
    /// When this method returns, contains the window if found; otherwise, null.
    /// </param>
    /// <returns>True if window found; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Title is null.</exception>
    /// <remarks>
    /// Preferred method for conditional logic where window may not exist.
    /// Does not throw WindowNotFoundException - check return value instead.
    /// </remarks>
    public static bool TryFindByTitle(string title, bool exact, out WindowControl? window)
    {
        if (title == null)
            throw new ArgumentNullException(nameof(title));

        try
        {
            var windows = WindowFinder.FindAll();
            var filtered = WindowFinder.FilterByTitle(windows, title, exact);
            window = filtered.FirstOrDefault();
            return window != null;
        }
        catch
        {
            window = null;
            return false;
        }
    }

    /// <summary>
    /// Finds all windows whose titles match the specified regular expression pattern.
    /// </summary>
    /// <param name="pattern">
    /// .NET regular expression pattern (case-insensitive by default).
    /// Pattern is compiled and cached for performance.
    /// </param>
    /// <returns>Collection of windows with titles matching the pattern. May be empty.</returns>
    /// <exception cref="ArgumentNullException">Pattern is null.</exception>
    /// <exception cref="ArgumentException">Pattern is not a valid regular expression.</exception>
    /// <remarks>
    /// Examples:
    /// - ".*Chrome$" matches titles ending with "Chrome"
    /// - "^Notepad" matches titles starting with "Notepad"
    /// - "File.*Explorer" matches titles containing "File" followed by "Explorer"
    /// Compiled regex instances are cached per pattern for performance.
    /// </remarks>
    public static IEnumerable<WindowControl> FindByTitleRegex(string pattern)
    {
        if (pattern == null)
            throw new ArgumentNullException(nameof(pattern));

        var windows = WindowFinder.FindAll();
        return WindowFinder.FilterByTitleRegex(windows, pattern);
    }

    /// <summary>
    /// Finds all windows with the specified class name.
    /// </summary>
    /// <param name="className">
    /// Window class name (e.g., "Notepad", "Chrome_WidgetWin_1").
    /// Case-insensitive comparison.
    /// </param>
    /// <returns>Collection of windows with matching class name. May be empty.</returns>
    /// <exception cref="ArgumentNullException">ClassName is null.</exception>
    /// <remarks>
    /// Window class names are more stable than titles for identifying window types.
    /// Use Spy++ or similar tools to discover class names.
    /// Empty string matches windows with no class name (rare but valid).
    /// </remarks>
    public static IEnumerable<WindowControl> FindByClassName(string className)
    {
        if (className == null)
            throw new ArgumentNullException(nameof(className));

        var windows = WindowFinder.FindAll();
        return WindowFinder.FilterByClassName(windows, className);
    }

    /// <summary>
    /// Finds all windows owned by the specified process.
    /// </summary>
    /// <param name="processId">Process ID (PID) of the owning process.</param>
    /// <returns>
    /// Collection of windows owned by the process. May be empty if process has no windows.
    /// </returns>
    /// <remarks>
    /// Useful for scenarios with multiple instances of the same application.
    /// Returns all windows (main + dialogs) owned by the process.
    /// If process ID does not exist, returns empty collection (does not throw).
    /// </remarks>
    public static IEnumerable<WindowControl> FindByProcessId(uint processId)
    {
        var windows = WindowFinder.FindAll();
        return WindowFinder.FilterByProcessId(windows, processId);
    }

    /// <summary>
    /// Creates a WindowControl wrapper for an existing window handle.
    /// </summary>
    /// <param name="handle">Native HWND pointer (IntPtr) to wrap.</param>
    /// <returns>WindowControl instance wrapping the handle.</returns>
    /// <exception cref="InvalidWindowHandleException">
    /// Handle is IntPtr.Zero or does not reference a valid window.
    /// </exception>
    /// <remarks>
    /// Use this method to wrap window handles obtained from other sources
    /// (e.g., Win32 API calls, interop with other libraries).
    /// Handle is validated with IsWindow() before returning.
    /// </remarks>
    public static WindowControl FromHandle(IntPtr handle)
    {
        return new WindowControl(handle);
    }

    /// <summary>
    /// Gets the currently active (foreground) window.
    /// </summary>
    /// <returns>
    /// The currently active window, or null if no window is in the foreground.
    /// </returns>
    /// <remarks>
    /// Returns the window that has keyboard focus and is displayed at the top of the Z-order.
    /// May return null if no window is currently active (rare).
    /// The returned window may be owned by any process.
    /// </remarks>
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
