namespace FSRS.Core.Extensions;

/// <summary>
/// Mathematical utility extensions
/// </summary>
public static class MathExtensions
{
    /// <summary>
    /// Clamps a double value between min and max bounds
    /// </summary>
    public static double Clamp(this double value, double min, double max)
    {
        return Math.Min(Math.Max(value, min), max);
    }

    /// <summary>
    /// Clamps an integer value between min and max bounds
    /// </summary>
    public static int Clamp(this int value, int min, int max)
    {
        return Math.Min(Math.Max(value, min), max);
    }
}