using System.Text.RegularExpressions;
using WindowManagerCL.API;

namespace WindowManagerCL.Core;

internal static partial class WindowFinder
{
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
                continue;
            }

            var matches = exact
                ? string.Equals(windowTitle, title, StringComparison.OrdinalIgnoreCase)
                : windowTitle.Contains(title, StringComparison.OrdinalIgnoreCase);

            if (matches)
                yield return window;
        }
    }

    internal static IEnumerable<WindowControl> FilterByTitleRegex(
        IEnumerable<WindowControl> windows,
        string pattern)
    {
        if (pattern == null)
            throw new ArgumentNullException(nameof(pattern));

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
                continue;
            }

            if (regex.IsMatch(windowTitle))
                yield return window;
        }
    }

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
                continue;
            }

            if (string.Equals(windowClassName, className, StringComparison.OrdinalIgnoreCase))
                yield return window;
        }
    }

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
                continue;
            }

            if (windowProcessId == processId)
                yield return window;
        }
    }
}
