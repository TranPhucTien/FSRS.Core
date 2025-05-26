using FSRS.Core.Configurations;
using FSRS.Core.Interfaces;

namespace FSRS.Core.Services;

/// <summary>
/// Factory implementation for creating IScheduler instances with different configurations.
/// </summary>
public class SchedulerFactory : ISchedulerFactory
{
    private readonly SchedulerOptions _defaultOptions;

    /// <summary>
    /// Initializes a new instance of the SchedulerFactory class.
    /// </summary>
    /// <param name="defaultOptions">The default FSRS configuration options.</param>
    public SchedulerFactory(SchedulerOptions defaultOptions)
    {
        _defaultOptions = defaultOptions;
    }

    /// <summary>
    /// Creates a scheduler with default configuration.
    /// </summary>
    /// <returns>A new IScheduler instance with default settings.</returns>
    public IScheduler CreateScheduler()
    {
        return new Scheduler(
            parameters: _defaultOptions.Parameters,
            desiredRetention: _defaultOptions.DesiredRetention,
            learningSteps: _defaultOptions.LearningSteps,
            relearningSteps: _defaultOptions.RelearningSteps,
            maximumInterval: _defaultOptions.MaximumInterval,
            enableFuzzing: _defaultOptions.EnableFuzzing);
        // Không cần inject services - Scheduler sẽ tạo mới
    }

    /// <summary>
    /// Creates a scheduler with custom parameters and retention settings.
    /// </summary>
    /// <param name="parameters">Custom FSRS parameters. If null, uses default parameters.</param>
    /// <param name="desiredRetention">Custom desired retention rate. If null, uses default retention.</param>
    /// <returns>A new IScheduler instance with the specified configuration.</returns>
    public IScheduler CreateScheduler(double[]? parameters = null, double? desiredRetention = null)
    {
        return new Scheduler(
            parameters: parameters ?? _defaultOptions.Parameters,
            desiredRetention: desiredRetention ?? _defaultOptions.DesiredRetention,
            learningSteps: _defaultOptions.LearningSteps,
            relearningSteps: _defaultOptions.RelearningSteps,
            maximumInterval: _defaultOptions.MaximumInterval,
            enableFuzzing: _defaultOptions.EnableFuzzing);
    }
}