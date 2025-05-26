using FSRS.Core.Configurations;
using FSRS.Core.Interfaces;
using FSRS.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FSRS.Core.Extensions;

/// <summary>
/// Extension methods for configuring FSRS services in dependency injection container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds FSRS services to the service collection with default configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddFSRS(this IServiceCollection services)
    {
        return services.AddFSRS(_ => { });
    }

    /// <summary>
    /// Adds FSRS services to the service collection with custom configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Configuration action</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddFSRS(this IServiceCollection services, Action<SchedulerOptions> configure)
    {
        var options = new SchedulerOptions();
        configure(options);

        // Register core calculation services
        services.TryAddTransient<IRetrievabilityCalculator, RetrievabilityCalculator>();
        services.TryAddTransient<IStabilityCalculator, StabilityCalculator>();
        services.TryAddTransient<IDifficultyCalculator, DifficultyCalculator>();
        services.TryAddTransient<IIntervalCalculator, IntervalCalculator>();
        services.TryAddTransient<IFuzzingService, FuzzingService>();

        // Register scheduler with configured options
        services.TryAddSingleton<IScheduler>(provider =>
        {
            var retrievabilityCalculator = provider.GetRequiredService<IRetrievabilityCalculator>();
            var stabilityCalculator = provider.GetRequiredService<IStabilityCalculator>();
            var difficultyCalculator = provider.GetRequiredService<IDifficultyCalculator>();
            var intervalCalculator = provider.GetRequiredService<IIntervalCalculator>();
            var fuzzingService = provider.GetRequiredService<IFuzzingService>();

            return new Scheduler(
                parameters: options.Parameters,
                desiredRetention: options.DesiredRetention,
                learningSteps: options.LearningSteps,
                relearningSteps: options.RelearningSteps,
                maximumInterval: options.MaximumInterval,
                enableFuzzing: options.EnableFuzzing,
                retrievabilityCalculator: retrievabilityCalculator,
                stabilityCalculator: stabilityCalculator,
                difficultyCalculator: difficultyCalculator,
                intervalCalculator: intervalCalculator,
                fuzzingService: fuzzingService
            );
        });

        return services;
    }

    /// <summary>
    /// Adds FSRS scheduler as a singleton with specific configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="scheduler">Pre-configured scheduler instance</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddFSRS(this IServiceCollection services, IScheduler scheduler)
    {
        services.TryAddSingleton(scheduler);
        return services;
    }
}