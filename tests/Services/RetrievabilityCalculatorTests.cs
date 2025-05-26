using FSRS.Core.Constants;
using FSRS.Core.Interfaces;
using FSRS.Core.Models;
using FSRS.Core.Services;

namespace Services;

public class RetrievabilityCalculatorTests
{
    private readonly IRetrievabilityCalculator _retrievabilityCalculator;
    private readonly double[] _defaultParameters;

    public RetrievabilityCalculatorTests()
    {
        _retrievabilityCalculator = new RetrievabilityCalculator();
        _defaultParameters = FsrsConstants.DefaultParameters;
    }

    [Fact]
    public void CalculateRetrievability_CardWithoutLastReview_ShouldReturnZero()
    {
        var card = new Card();

        var retrievability = _retrievabilityCalculator.CalculateRetrievability(card);

        Assert.Equal(0, retrievability);
    }

    [Fact]
    public void CalculateRetrievability_CardWithLastReview_ShouldReturnValidRange()
    {
        var card = new Card
        {
            LastReview = DateTime.UtcNow.AddDays(-1),
            Stability = 10.0
        };

        var retrievability = _retrievabilityCalculator.CalculateRetrievability(card);

        Assert.InRange(retrievability, 0.0, 1.0);
    }

    [Fact]
    public void CalculateRetrievability_LongerTimePassed_ShouldProduceLowerRetrievability()
    {
        var baseDateTime = DateTime.UtcNow;
        var stability = 10.0;

        var cardRecent = new Card
        {
            LastReview = baseDateTime.AddDays(-1),
            Stability = stability
        };

        var cardOld = new Card
        {
            LastReview = baseDateTime.AddDays(-10),
            Stability = stability
        };

        var recentRetrievability = _retrievabilityCalculator.CalculateRetrievability(cardRecent, baseDateTime, _defaultParameters);
        var oldRetrievability = _retrievabilityCalculator.CalculateRetrievability(cardOld, baseDateTime, _defaultParameters);

        Assert.True(recentRetrievability > oldRetrievability);
    }

    [Fact]
    public void CalculateRetrievability_HigherStability_ShouldProduceHigherRetrievability()
    {
        var baseDateTime = DateTime.UtcNow;
        var lastReview = baseDateTime.AddDays(-5);

        var cardLowStability = new Card
        {
            LastReview = lastReview,
            Stability = 5.0
        };

        var cardHighStability = new Card
        {
            LastReview = lastReview,
            Stability = 20.0
        };

        var lowStabilityRetrievability = _retrievabilityCalculator.CalculateRetrievability(cardLowStability, baseDateTime, _defaultParameters);
        var highStabilityRetrievability = _retrievabilityCalculator.CalculateRetrievability(cardHighStability, baseDateTime, _defaultParameters);

        Assert.True(highStabilityRetrievability > lowStabilityRetrievability);
    }

    [Fact]
    public void CalculateRetrievability_SameDayReview_ShouldReturnOne()
    {
        var baseDateTime = DateTime.UtcNow;
        var card = new Card
        {
            LastReview = baseDateTime,
            Stability = 10.0
        };

        var retrievability = _retrievabilityCalculator.CalculateRetrievability(card, baseDateTime, _defaultParameters);

        Assert.Equal(1.0, retrievability, 6); // Should be 1.0 (or very close) when no time has passed
    }

    [Fact]
    public void CalculateRetrievability_NegativeElapsedTime_ShouldHandleGracefully()
    {
        var baseDateTime = DateTime.UtcNow;
        var card = new Card
        {
            LastReview = baseDateTime.AddDays(1), // Future date
            Stability = 10.0
        };

        var retrievability = _retrievabilityCalculator.CalculateRetrievability(card, baseDateTime, _defaultParameters);

        Assert.InRange(retrievability, 0.0, 1.0);
    }

    [Fact]
    public void CalculateRetrievability_DefaultCurrentDateTime_ShouldUseUtcNow()
    {
        var card = new Card
        {
            LastReview = DateTime.UtcNow.AddDays(-1),
            Stability = 10.0
        };

        var retrievability = _retrievabilityCalculator.CalculateRetrievability(card);

        Assert.InRange(retrievability, 0.0, 1.0);
    }

    [Fact]
    public void CalculateRetrievability_CustomParameters_ShouldUseProvided()
    {
        var card = new Card
        {
            LastReview = DateTime.UtcNow.AddDays(-5),
            Stability = 10.0
        };

        var customParameters = new double[21];
        Array.Copy(_defaultParameters, customParameters, 21);
        customParameters[20] = 0.1; // Different decay parameter

        var defaultRetrievability = _retrievabilityCalculator.CalculateRetrievability(card, null, _defaultParameters);
        var customRetrievability = _retrievabilityCalculator.CalculateRetrievability(card, null, customParameters);

        Assert.NotEqual(defaultRetrievability, customRetrievability);
    }
}