namespace FSRS.Core.Interfaces;

/// <summary>
/// Applies randomization to review intervals to prevent synchronized reviews
/// </summary>
public interface IFuzzingService
{
    /// <summary>
    /// Applies fuzzing to a review interval
    /// </summary>
    /// <param name="interval">Base interval to fuzz</param>
    /// <param name="maximumInterval">Maximum allowed interval</param>
    /// <returns>Fuzzed interval</returns>
    TimeSpan ApplyFuzzing(TimeSpan interval, int maximumInterval);
}