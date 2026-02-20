using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using WindowManagerCL.API;

namespace WindowManagerCL.Core;

/// <summary>
/// Internal helper class for finding and filtering windows.
/// </summary>
internal static partial class WindowFinder
{
    private static readonly ConcurrentDictionary<string, Regex> _regexCache = new();
}
