using FSRS.Core.Interfaces;
using FSRS.Core.Services;

namespace Services;

public class FuzzingServiceTests
{
    private readonly IFuzzingService _fuzzingService;

    public FuzzingServiceTests()
    {
        _fuzzingService = new FuzzingService();
    }

    [Fact]
    public void ApplyFuzzing_ShortInterval_ShouldReturnUnchanged()
    {
        var shortInterval = TimeSpan.FromDays(2);
        var maximumInterval = 36500;

        var fuzzedInterval = _fuzzingService.ApplyFuzzing(shortInterval, maximumInterval);

        Assert.Equal(shortInterval, fuzzedInterval);
    }

    [Fact]
    public void ApplyFuzzing_LongInterval_ShouldReturnFuzzedValue()
    {
        // Arrange
        var longInterval = TimeSpan.FromDays(30);
        var maximumInterval = 36500;

        var results = new HashSet<double>();

        for (int i = 0; i < 100; i++)
        {
            var fuzzedInterval = _fuzzingService.ApplyFuzzing(longInterval, maximumInterval);
            results.Add(fuzzedInterval.TotalDays);
        }

        Assert.True(results.Count > 1, "Fuzzing did not produce varied results");
    }

    [Theory]
    [InlineData(3)]
    [InlineData(10)]
    [InlineData(30)]
    [InlineData(100)]
    public void ApplyFuzzing_VariousIntervals_ShouldRespectMaximumInterval(int intervalDays)
    {
        var interval = TimeSpan.FromDays(intervalDays);
        var maximumInterval = 36500;

        var fuzzedInterval = _fuzzingService.ApplyFuzzing(interval, maximumInterval);

        Assert.True(fuzzedInterval.Days <= maximumInterval);
    }

    [Fact]
    public void ApplyFuzzing_LargeInterval_ShouldProduceReasonableRange()
    {
        var interval = TimeSpan.FromDays(50);
        var maximumInterval = 36500;
        var results = new List<int>();

        // Collect multiple results
        for (int i = 0; i < 1000; i++)
        {
            var fuzzedInterval = _fuzzingService.ApplyFuzzing(interval, maximumInterval);
            results.Add(fuzzedInterval.Days);
        }

        var min = results.Min();
        var max = results.Max();
        var originalDays = interval.Days;

        // Fuzzed values should be within reasonable range of original
        Assert.True(min >= originalDays * 0.8); // At least 80% of original
        Assert.True(max <= originalDays * 1.2); // At most 120% of original
        Assert.True(max > min); // Should have variation
    }

    [Fact]
    public void ApplyFuzzing_MinimumIntervalThreshold_ShouldRespectMinimum()
    {
        var interval = TimeSpan.FromDays(10);
        var maximumInterval = 36500;
        var results = new List<int>();

        // Collect multiple results
        for (int i = 0; i < 100; i++)
        {
            var fuzzedInterval = _fuzzingService.ApplyFuzzing(interval, maximumInterval);
            results.Add(fuzzedInterval.Days);
        }

        // All results should be at least 2 days (minimum enforced in fuzzing)
        Assert.All(results, days => Assert.True(days >= 2));
    }

    [Fact]
    public void ApplyFuzzing_ExactThreshold_ShouldHandleBoundaryCorrectly()
    {
        var thresholdInterval = TimeSpan.FromDays(2.5);
        var maximumInterval = 36500;

        var fuzzedInterval = _fuzzingService.ApplyFuzzing(thresholdInterval, maximumInterval);

        // At exactly 2.5 days, should not be fuzzed
        Assert.Equal(thresholdInterval, fuzzedInterval);
    }
}