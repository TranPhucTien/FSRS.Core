namespace FSRS.Core.Models;

/// <summary>
/// Represents a range for interval fuzzing with start, end, and factor values
/// </summary>
/// <param name="Start">Start of the range in days</param>
/// <param name="End">End of the range in days</param>
/// <param name="Factor">Fuzzing factor to apply</param>
public record FuzzRange(double Start, double End, double Factor);
