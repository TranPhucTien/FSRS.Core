namespace FSRS.Core.Configurations;

/// <summary>
/// Configuration options for Scheduler services
/// </summary>
public class SchedulerOptions
{
    /// <summary>
    /// FSRS algorithm parameters (uses defaults if null)
    /// </summary>
    public double[]? Parameters { get; set; }

    /// <summary>
    /// Target retention rate (default: 0.9)
    /// </summary>
    public double DesiredRetention { get; set; } = 0.9;

    /// <summary>
    /// Learning step intervals for new cards (default: 1min, 10min)
    /// </summary>
    public TimeSpan[] LearningSteps { get; set; } =
    [
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(10)
    ];

    /// <summary>
    /// Relearning step intervals for forgotten cards (default: 10min)
    /// </summary>
    public TimeSpan[] RelearningSteps { get; set; } =
    [
        TimeSpan.FromMinutes(10)
    ];

    /// <summary>
    /// Maximum review interval in days (default: 36500)
    /// </summary>
    public int MaximumInterval { get; set; } = 36500;

    /// <summary>
    /// Whether to apply interval fuzzing (default: true)
    /// </summary>
    public bool EnableFuzzing { get; set; } = true;
}