using FSRS.Core.Constants;
using FSRS.Core.Interfaces;
using FSRS.Core.Services;

namespace Services;

public class IntervalCalculatorTests
{
    private readonly IIntervalCalculator _intervalCalculator;
    private readonly double[] _defaultParameters;

    public IntervalCalculatorTests()
    {
        _intervalCalculator = new IntervalCalculator();
        _defaultParameters = FsrsConstants.DefaultParameters;
    }

    [Theory]
    [InlineData(1.0, 0.9, 36500)]
    [InlineData(10.0, 0.9, 36500)]
    [InlineData(100.0, 0.9, 36500)]
    public void CalculateNextInterval_ValidInputs_ShouldReturnPositiveInterval(double stability, double desiredRetention, int maximumInterval)
    {
        var interval = _intervalCalculator.CalculateNextInterval(stability, desiredRetention, _defaultParameters, maximumInterval);

        Assert.True(interval > 0);
        Assert.True(interval <= maximumInterval);
    }

    [Fact]
    public void CalculateNextInterval_HigherStability_ShouldProduceLongerInterval()
    {
        var lowStability = 5.0;
        var highStability = 50.0;
        var desiredRetention = 0.9;
        var maximumInterval = 36500;

        var lowInterval = _intervalCalculator.CalculateNextInterval(lowStability, desiredRetention, _defaultParameters, maximumInterval);
        var highInterval = _intervalCalculator.CalculateNextInterval(highStability, desiredRetention, _defaultParameters, maximumInterval);

        Assert.True(highInterval > lowInterval);
    }

    [Fact]
    public void CalculateNextInterval_HigherDesiredRetention_ShouldProduceShorterInterval()
    {
        var stability = 10.0;
        var lowRetention = 0.8;
        var highRetention = 0.95;
        var maximumInterval = 36500;

        var lowRetentionInterval = _intervalCalculator.CalculateNextInterval(stability, lowRetention, _defaultParameters, maximumInterval);
        var highRetentionInterval = _intervalCalculator.CalculateNextInterval(stability, highRetention, _defaultParameters, maximumInterval);

        Assert.True(highRetentionInterval < lowRetentionInterval);
    }

    [Fact]
    public void CalculateNextInterval_MaximumInterval_ShouldEnforceLimit()
    {
        var stability = 1000.0; // Very high stability
        var desiredRetention = 0.9;
        var maximumInterval = 100;

        var interval = _intervalCalculator.CalculateNextInterval(stability, desiredRetention, _defaultParameters, maximumInterval);

        Assert.Equal(maximumInterval, interval);
    }

    [Fact]
    public void CalculateNextInterval_MinimumInterval_ShouldBeAtLeastOne()
    {
        var stability = 0.1; // Very low stability
        var desiredRetention = 0.99; // Very high retention requirement
        var maximumInterval = 36500;

        var interval = _intervalCalculator.CalculateNextInterval(stability, desiredRetention, _defaultParameters, maximumInterval);

        Assert.True(interval >= 1);
    }

    [Theory]
    [InlineData(0.1)]
    [InlineData(0.5)]
    [InlineData(0.8)]
    [InlineData(0.9)]
    [InlineData(0.95)]
    [InlineData(0.99)]
    public void CalculateNextInterval_VariousRetentions_ShouldProduceValidIntervals(double desiredRetention)
    {
        var stability = 10.0;
        var maximumInterval = 36500;

        var interval = _intervalCalculator.CalculateNextInterval(stability, desiredRetention, _defaultParameters, maximumInterval);

        Assert.InRange(interval, 1, maximumInterval);
    }

    [Fact]
    public void CalculateNextInterval_SameInputs_ShouldProduceSameResults()
    {
        var stability = 15.0;
        var desiredRetention = 0.9;
        var maximumInterval = 36500;

        var interval1 = _intervalCalculator.CalculateNextInterval(stability, desiredRetention, _defaultParameters, maximumInterval);
        var interval2 = _intervalCalculator.CalculateNextInterval(stability, desiredRetention, _defaultParameters, maximumInterval);

        Assert.Equal(interval1, interval2);
    }
}