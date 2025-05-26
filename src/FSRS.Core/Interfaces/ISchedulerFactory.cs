namespace FSRS.Core.Interfaces;

/// <summary>
/// Factory interface for creating IScheduler instances with different configurations.
/// </summary>
public interface ISchedulerFactory
{
    /// <summary>
    /// Creates a scheduler with default configuration.
    /// </summary>
    /// <returns>A new IScheduler instance with default settings.</returns>
    IScheduler CreateScheduler();

    /// <summary>
    /// Creates a scheduler with custom parameters and retention settings.
    /// </summary>
    /// <param name="parameters">Custom FSRS parameters. If null, uses default parameters.</param>
    /// <param name="desiredRetention">Custom desired retention rate. If null, uses default retention.</param>
    /// <returns>A new IScheduler instance with the specified configuration.</returns>
    IScheduler CreateScheduler(double[]? parameters = null, double? desiredRetention = null);
}