namespace WindowManagerCL.API;

/// <summary>
/// Exception thrown when a window search operation finds no matching windows.
/// </summary>
public sealed class WindowNotFoundException : WindowManagerException
{
    public string SearchCriteria { get; }

    public WindowNotFoundException(string searchCriteria)
        : base($"No window found matching criteria: {searchCriteria}")
    {
        SearchCriteria = searchCriteria;
    }

    public WindowNotFoundException(string searchCriteria, Exception innerException)
        : base($"No window found matching criteria: {searchCriteria}", IntPtr.Zero, null, innerException)
    {
        SearchCriteria = searchCriteria;
    }
}
