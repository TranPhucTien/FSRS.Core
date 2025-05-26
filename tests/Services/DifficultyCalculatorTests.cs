using FSRS.Core.Constants;
using FSRS.Core.Enums;
using FSRS.Core.Interfaces;
using FSRS.Core.Services;

namespace Services;

public class DifficultyCalculatorTests
{
    private readonly IDifficultyCalculator _difficultyCalculator;
    private readonly double[] _defaultParameters;

    public DifficultyCalculatorTests()
    {
        _difficultyCalculator = new DifficultyCalculator();
        _defaultParameters = FsrsConstants.DefaultParameters;
    }

    [Theory]
    [InlineData(Rating.Again)]
    [InlineData(Rating.Hard)]
    [InlineData(Rating.Good)]
    [InlineData(Rating.Easy)]
    public void CalculateInitialDifficulty_AllRatings_ShouldReturnValidRange(Rating rating)
    {
        var difficulty = _difficultyCalculator.CalculateInitialDifficulty(rating, _defaultParameters);

        Assert.InRange(difficulty, 1.0, 10.0);
    }

    [Fact]
    public void CalculateInitialDifficulty_EasierRatings_ShouldProduceLowerDifficulty()
    {
        var againDifficulty = _difficultyCalculator.CalculateInitialDifficulty(Rating.Again, _defaultParameters);
        var hardDifficulty = _difficultyCalculator.CalculateInitialDifficulty(Rating.Hard, _defaultParameters);
        var goodDifficulty = _difficultyCalculator.CalculateInitialDifficulty(Rating.Good, _defaultParameters);
        var easyDifficulty = _difficultyCalculator.CalculateInitialDifficulty(Rating.Easy, _defaultParameters);

        Assert.True(againDifficulty > hardDifficulty);
        Assert.True(hardDifficulty > goodDifficulty);
        Assert.True(goodDifficulty > easyDifficulty);
    }

    [Theory]
    [InlineData(5.0, Rating.Again)]
    [InlineData(5.0, Rating.Hard)]
    [InlineData(5.0, Rating.Good)]
    [InlineData(5.0, Rating.Easy)]
    public void CalculateNextDifficulty_AllRatings_ShouldReturnValidRange(double currentDifficulty, Rating rating)
    {
        var nextDifficulty = _difficultyCalculator.CalculateNextDifficulty(currentDifficulty, rating, _defaultParameters);

        Assert.InRange(nextDifficulty, 1.0, 10.0);
    }

    [Fact]
    public void CalculateNextDifficulty_AgainRating_ShouldIncreaseDifficulty()
    {
        var currentDifficulty = 5.0;
        var nextDifficulty = _difficultyCalculator.CalculateNextDifficulty(currentDifficulty, Rating.Again, _defaultParameters);

        Assert.True(nextDifficulty > currentDifficulty);
    }

    [Fact]
    public void CalculateNextDifficulty_EasyRating_ShouldDecreaseDifficulty()
    {
        var currentDifficulty = 5.0;
        var nextDifficulty = _difficultyCalculator.CalculateNextDifficulty(currentDifficulty, Rating.Easy, _defaultParameters);

        Assert.True(nextDifficulty < currentDifficulty);
    }

    [Fact]
    public void CalculateNextDifficulty_ExtremeValues_ShouldClampCorrectly()
    {
        // Test minimum boundary - Even with Easy rating from minimum difficulty, should stay at 1.0
        var minDifficulty = _difficultyCalculator.CalculateNextDifficulty(1.0, Rating.Easy, _defaultParameters);
        Assert.Equal(1.0, minDifficulty, 6);

        // Test maximum boundary - Due to mean reversion, even Again rating from max difficulty
        // may not result in exactly 10.0, but should be very close to 10.0
        var maxDifficulty = _difficultyCalculator.CalculateNextDifficulty(10.0, Rating.Again, _defaultParameters);
        Assert.True(maxDifficulty >= 9.5); // Should be close to maximum
        Assert.True(maxDifficulty <= 10.0); // Should not exceed maximum
    }

    [Fact]
    public void CalculateNextDifficulty_MeanReversion_ShouldTrendTowardEasyDifficulty()
    {
        var currentDifficulty = 8.0; // High difficulty
        var easyInitialDifficulty = _difficultyCalculator.CalculateInitialDifficulty(Rating.Easy, _defaultParameters);

        // After many Good ratings, difficulty should move toward easy initial difficulty
        var difficulty = currentDifficulty;
        for (int i = 0; i < 10; i++)
        {
            difficulty = _difficultyCalculator.CalculateNextDifficulty(difficulty, Rating.Good, _defaultParameters);
        }

        Assert.True(difficulty < currentDifficulty); // Should decrease
        // Note: Complete convergence depends on the mean reversion parameter
    }
}