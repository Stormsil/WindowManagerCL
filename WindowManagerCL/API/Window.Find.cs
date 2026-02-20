using WindowManagerCL.Core;

namespace WindowManagerCL.API;

public static partial class Window
{
    public static IEnumerable<WindowControl> FindAll()
    {
        return WindowFinder.FindAll();
    }

    public static WindowControl FindByTitle(string title, bool exact = true)
    {
        if (title == null)
            throw new ArgumentNullException(nameof(title));

        var windows = WindowFinder.FindAll();
        var filtered = WindowFinder.FilterByTitle(windows, title, exact);
        var result = filtered.FirstOrDefault();

        if (result == null)
        {
            var criteria = exact
                ? $"Title (exact): '{title}'"
                : $"Title (partial): '{title}'";
            throw new WindowNotFoundException(criteria);
        }

        return result;
    }

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

    public static IEnumerable<WindowControl> FindByTitleRegex(string pattern)
    {
        if (pattern == null)
            throw new ArgumentNullException(nameof(pattern));

        var windows = WindowFinder.FindAll();
        return WindowFinder.FilterByTitleRegex(windows, pattern);
    }

    public static IEnumerable<WindowControl> FindByClassName(string className)
    {
        if (className == null)
            throw new ArgumentNullException(nameof(className));

        var windows = WindowFinder.FindAll();
        return WindowFinder.FilterByClassName(windows, className);
    }

    public static IEnumerable<WindowControl> FindByProcessId(uint processId)
    {
        var windows = WindowFinder.FindAll();
        return WindowFinder.FilterByProcessId(windows, processId);
    }
}
