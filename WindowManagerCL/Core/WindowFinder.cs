using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using WindowManagerCL.API;
using WindowManagerCL.Infrastructure;

namespace WindowManagerCL.Core;

/// <summary>
/// Internal helper class for finding and filtering windows.
/// </summary>
internal static class WindowFinder
{
    // Cache for compiled regex patterns
    private static readonly ConcurrentDictionary<string, Regex> _regexCache = new();

    /// <summary>
    /// Finds all top-level windows.
    /// </summary>
    /// <returns>Collection of all windows.</returns>
    internal static IEnumerable<WindowControl> FindAll()
    {
        var handles = WindowEnumerator.EnumerateWindows();

        foreach (var handle in handles)
        {
            WindowControl? window = null;
            try
            {
                window = new WindowControl(handle);
            }
            catch
            {
                // Skip invalid handles
                continue;
            }

            yield return window;
        }
    }

    /// <summary>
    /// Filters windows by title (exact or partial match).
    /// </summary>
    /// <param name="windows">Windows to filter.</param>
    /// <param name="title">Title to search for.</param>
    /// <param name="exact">If true, exact match; if false, partial match (substring).</param>
    /// <returns>Filtered windows.</returns>
    internal static IEnumerable<WindowControl> FilterByTitle(
        IEnumerable<WindowControl> windows,
        string title,
        bool exact)
    {
        if (title == null)
            throw new ArgumentNullException(nameof(title));

        foreach (var window in windows)
        {
            string windowTitle;
            try
            {
                windowTitle = window.Title;
            }
            catch
            {
                // Skip windows where we can't get title
                continue;
            }

            bool matches = exact
                ? string.Equals(windowTitle, title, StringComparison.OrdinalIgnoreCase)
                : windowTitle.Contains(title, StringComparison.OrdinalIgnoreCase);

            if (matches)
                yield return window;
        }
    }

    /// <summary>
    /// Filters windows by title using regular expression.
    /// </summary>
    /// <param name="windows">Windows to filter.</param>
    /// <param name="pattern">Regex pattern.</param>
    /// <returns>Filtered windows.</returns>
    internal static IEnumerable<WindowControl> FilterByTitleRegex(
        IEnumerable<WindowControl> windows,
        string pattern)
    {
        if (pattern == null)
            throw new ArgumentNullException(nameof(pattern));

        // Get or create compiled regex (cached)
        Regex regex;
        try
        {
            regex = _regexCache.GetOrAdd(pattern, p =>
                new Regex(p, RegexOptions.Compiled | RegexOptions.IgnoreCase));
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Invalid regex pattern: {pattern}", nameof(pattern), ex);
        }

        foreach (var window in windows)
        {
            string windowTitle;
            try
            {
                windowTitle = window.Title;
            }
            catch
            {
                // Skip windows where we can't get title
                continue;
            }

            if (regex.IsMatch(windowTitle))
                yield return window;
        }
    }

    /// <summary>
    /// Filters windows by class name.
    /// </summary>
    /// <param name="windows">Windows to filter.</param>
    /// <param name="className">Class name to search for.</param>
    /// <returns>Filtered windows.</returns>
    internal static IEnumerable<WindowControl> FilterByClassName(
        IEnumerable<WindowControl> windows,
        string className)
    {
        if (className == null)
            throw new ArgumentNullException(nameof(className));

        foreach (var window in windows)
        {
            string windowClassName;
            try
            {
                windowClassName = window.ClassName;
            }
            catch
            {
                // Skip windows where we can't get class name
                continue;
            }

            if (string.Equals(windowClassName, className, StringComparison.OrdinalIgnoreCase))
                yield return window;
        }
    }

    /// <summary>
    /// Filters windows by process ID.
    /// </summary>
    /// <param name="windows">Windows to filter.</param>
    /// <param name="processId">Process ID to search for.</param>
    /// <returns>Filtered windows.</returns>
    internal static IEnumerable<WindowControl> FilterByProcessId(
        IEnumerable<WindowControl> windows,
        uint processId)
    {
        foreach (var window in windows)
        {
            uint windowProcessId;
            try
            {
                windowProcessId = window.ProcessId;
            }
            catch
            {
                // Skip windows where we can't get process ID
                continue;
            }

            if (windowProcessId == processId)
                yield return window;
        }
    }
}
