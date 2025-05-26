using FSRS.Core.Models;

namespace FSRS.Core.Constants;

/// <summary>
/// Contains constants and default parameters for the FSRS algorithm
/// </summary>
public static class FsrsConstants
{
    /// <summary>
    /// Minimum allowed stability value to prevent division by zero
    /// </summary>
    public const double StabilityMin = 0.001;

    /// <summary>
    /// Default FSRS algorithm parameters optimized for general use
    /// </summary>
    public static readonly double[] DefaultParameters =
    [
        0.2172, 1.1771, 3.2602, 16.1507, 7.0114, 0.57,
        2.0966, 0.0069, 1.5261, 0.112, 1.0178,
        1.849, 0.1133, 0.3127, 2.2934, 0.2191,
        3.0004, 0.7536, 0.3332, 0.1437, 0.2,
    ];

    /// <summary>
    /// Fuzzing ranges for interval randomization
    /// </summary>
    public static readonly FuzzRange[] FuzzRanges =
    [
        new(2.5, 7.0, 0.15),
        new(7.0, 20.0, 0.1),
        new(20.0, double.PositiveInfinity, 0.05)
    ];
}