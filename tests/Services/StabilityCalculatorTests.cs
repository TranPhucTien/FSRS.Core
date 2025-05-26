using FSRS.Core.Constants;
using FSRS.Core.Enums;
using FSRS.Core.Interfaces;
using FSRS.Core.Services;

namespace Services;

public class StabilityCalculatorTests
{
    private readonly IStabilityCalculator _stabilityCalculator;
    private readonly double[] _defaultParameters;

    public StabilityCalculatorTests()
    {
        _stabilityCalculator = new StabilityCalculator();
        _defaultParameters = FsrsConstants.DefaultParameters;
    }

    [Theory]
    [InlineData(Rating.Again, 1)]
    [InlineData(Rating.Hard, 2)]
    [InlineData(Rating.Good, 3)]
    [InlineData(Rating.Easy, 4)]
    public void CalculateInitialStability_AllRatings_ShouldReturnValidStability(Rating rating, int expectedParameterIndex)
    {
        var stability = _stabilityCalculator.CalculateInitialStability(rating, _defaultParameters);

        Assert.True(stability >= FsrsConstants.StabilityMin);
        Assert.Equal(_defaultParameters[expectedParameterIndex - 1], stability, 6);
    }

    [Fact]
    public void CalculateInitialStability_ShouldClampToMinimum()
    {
        var parametersWithZero = new double[21];
        parametersWithZero[0] = 0.0; // Rating.Again parameter

        var stability = _stabilityCalculator.CalculateInitialStability(Rating.Again, parametersWithZero);

        Assert.Equal(FsrsConstants.StabilityMin, stability);
    }

    [Theory]
    [InlineData(10.0, Rating.Good, 1.0)]
    [InlineData(10.0, Rating.Easy, 1.0)]
    [InlineData(5.0, Rating.Hard, 0.5)]
    [InlineData(5.0, Rating.Again, 0.5)]
    public void CalculateShortTermStability_ShouldProduceReasonableResults(double currentStability, Rating rating, double minExpectedMultiplier)
    {
        var newStability = _stabilityCalculator.CalculateShortTermStability(currentStability, rating, _defaultParameters);

        Assert.True(newStability >= FsrsConstants.StabilityMin);

        if (rating is Rating.Good or Rating.Easy)
        {
            Assert.True(newStability >= currentStability * minExpectedMultiplier);
        }
    }

    [Fact]
    public void CalculateNextStability_AgainRating_ShouldCalculateForgetStability()
    {
        var difficulty = 5.0;
        var stability = 10.0;
        var retrievability = 0.8;

        var nextStability = _stabilityCalculator.CalculateNextStability(
            difficulty, stability, retrievability, Rating.Again, _defaultParameters);

        Assert.True(nextStability >= FsrsConstants.StabilityMin);
        Assert.True(nextStability <= stability); // Should decrease for Again rating
    }

    [Theory]
    [InlineData(Rating.Hard)]
    [InlineData(Rating.Good)]
    [InlineData(Rating.Easy)]
    public void CalculateNextStability_RecallRatings_ShouldCalculateRecallStability(Rating rating)
    {
        var difficulty = 5.0;
        var stability = 10.0;
        var retrievability = 0.8;

        var nextStability = _stabilityCalculator.CalculateNextStability(
            difficulty, stability, retrievability, rating, _defaultParameters);

        Assert.True(nextStability >= FsrsConstants.StabilityMin);

        if (rating == Rating.Good)
        {
            Assert.True(nextStability >= stability); // Should generally increase for Good rating
        }
    }

    [Fact]
    public void CalculateNextStability_HardRating_ShouldApplyPenalty()
    {
        var difficulty = 5.0;
        var stability = 10.0;
        var retrievability = 0.8;

        var hardStability = _stabilityCalculator.CalculateNextStability(
            difficulty, stability, retrievability, Rating.Hard, _defaultParameters);

        var goodStability = _stabilityCalculator.CalculateNextStability(
            difficulty, stability, retrievability, Rating.Good, _defaultParameters);

        Assert.True(hardStability < goodStability); // Hard should produce lower stability than Good
    }

    [Fact]
    public void CalculateNextStability_EasyRating_ShouldApplyBonus()
    {
        var difficulty = 5.0;
        var stability = 10.0;
        var retrievability = 0.8;

        var easyStability = _stabilityCalculator.CalculateNextStability(
            difficulty, stability, retrievability, Rating.Easy, _defaultParameters);

        var goodStability = _stabilityCalculator.CalculateNextStability(
            difficulty, stability, retrievability, Rating.Good, _defaultParameters);

        Assert.True(easyStability > goodStability); // Easy should produce higher stability than Good
    }
}