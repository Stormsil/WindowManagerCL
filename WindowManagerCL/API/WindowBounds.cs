namespace WindowManagerCL.API;

/// <summary>
/// Represents window position and size as an immutable struct.
/// </summary>
public readonly struct WindowBounds : IEquatable<WindowBounds>
{
    /// <summary>
    /// Gets the left edge coordinate (screen space). Can be negative for multi-monitor setups.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Gets the top edge coordinate (screen space). Can be negative for multi-monitor setups.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Gets the window width in pixels. Must be greater than 0.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the window height in pixels. Must be greater than 0.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the right edge coordinate (X + Width).
    /// </summary>
    public int Right => X + Width;

    /// <summary>
    /// Gets the bottom edge coordinate (Y + Height).
    /// </summary>
    public int Bottom => Y + Height;

    /// <summary>
    /// Initializes a new instance of WindowBounds.
    /// </summary>
    /// <param name="x">Left edge coordinate.</param>
    /// <param name="y">Top edge coordinate.</param>
    /// <param name="width">Window width. Must be greater than 0.</param>
    /// <param name="height">Window height. Must be greater than 0.</param>
    /// <exception cref="ArgumentOutOfRangeException">Width or height is less than or equal to 0.</exception>
    public WindowBounds(int x, int y, int width, int height)
    {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be greater than 0");

        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be greater than 0");

        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Returns a string representation of the window bounds.
    /// </summary>
    /// <returns>String in format "(X, Y, Width, Height)".</returns>
    public override string ToString()
    {
        return $"({X}, {Y}, {Width}, {Height})";
    }

    /// <summary>
    /// Determines whether two WindowBounds instances are equal.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is WindowBounds other && Equals(other);
    }

    /// <summary>
    /// Determines whether two WindowBounds instances are equal.
    /// </summary>
    public bool Equals(WindowBounds other)
    {
        return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Width, Height);
    }

    /// <summary>
    /// Determines whether two WindowBounds instances are equal.
    /// </summary>
    public static bool operator ==(WindowBounds left, WindowBounds right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two WindowBounds instances are not equal.
    /// </summary>
    public static bool operator !=(WindowBounds left, WindowBounds right)
    {
        return !left.Equals(right);
    }
}
