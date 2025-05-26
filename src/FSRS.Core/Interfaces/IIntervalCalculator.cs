namespace FSRS.Core.Interfaces;

/// <summary>
/// Calculates optimal review intervals based on stability and desired retention
/// </summary>
public interface IIntervalCalculator
{
    /// <summary>
    /// Calculates the next review interval in days
    /// </summary>
    /// <param name="stability">Current card stability</param>
    /// <param name="desiredRetention">Target retention rate (0-1)</param>
    /// <param name="parameters">FSRS algorithm parameters</param>
    /// <param name="maximumInterval">Maximum allowed interval in days</param>
    /// <returns>Next review interval in days</returns>
    int CalculateNextInterval(double stability, double desiredRetention, double[] parameters, int maximumInterval);
}