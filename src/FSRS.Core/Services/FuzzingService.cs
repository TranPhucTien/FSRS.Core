using FSRS.Core.Constants;
using FSRS.Core.Interfaces;

namespace FSRS.Core.Services;

/// <inheritdoc/>
public class FuzzingService : IFuzzingService
{
    private readonly Random _random = new();

    /// <inheritdoc/>
    public TimeSpan ApplyFuzzing(TimeSpan interval, int maximumInterval)
    {
        var intervalDays = interval.Days;

        if (intervalDays < 2.5)
            return interval;

        var (minInterval, maxInterval) = GetFuzzRange(intervalDays, maximumInterval);
        var fuzzedDays = _random.NextDouble() * (maxInterval - minInterval + 1) + minInterval;
        var clampedDays = Math.Min(Math.Round(fuzzedDays), maximumInterval);

        return TimeSpan.FromDays(clampedDays);
    }

    private static (int Min, int Max) GetFuzzRange(int intervalDays, int maximumInterval)
    {
        var delta = 1.0;

        foreach (var fuzzRange in FsrsConstants.FuzzRanges)
        {
            delta += fuzzRange.Factor * Math.Max(
                Math.Min(intervalDays, fuzzRange.End) - fuzzRange.Start, 0.0);
        }

        var minInterval = Math.Max(2, (int)Math.Round(intervalDays - delta));
        var maxInterval = Math.Min((int)Math.Round(intervalDays + delta), maximumInterval);
        minInterval = Math.Min(minInterval, maxInterval);

        return (minInterval, maxInterval);
    }
}