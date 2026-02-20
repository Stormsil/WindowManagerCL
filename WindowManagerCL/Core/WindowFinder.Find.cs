using WindowManagerCL.API;
using WindowManagerCL.Infrastructure;

namespace WindowManagerCL.Core;

internal static partial class WindowFinder
{
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
                continue;
            }

            yield return window;
        }
    }
}
