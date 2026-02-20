namespace WindowManagerCL.API;

public sealed partial class WindowControl
{
    /// <summary>
    /// Returns string representation of window (handle and title).
    /// </summary>
    public override string ToString()
    {
        try
        {
            return $"WindowControl[0x{Handle.ToString("X")}]: {Title}";
        }
        catch
        {
            return $"WindowControl[0x{Handle.ToString("X")}]: <invalid>";
        }
    }

    /// <summary>
    /// Determines whether two WindowControl instances refer to the same window.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is WindowControl other && Handle == other.Handle;
    }

    /// <summary>
    /// Returns hash code based on window handle.
    /// </summary>
    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }
}
