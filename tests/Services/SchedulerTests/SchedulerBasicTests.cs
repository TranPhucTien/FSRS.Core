using FSRS.Core.Enums;
using Helpers;

namespace Services.SchedulerTests;

public class SchedulerBasicTests
{
    [Fact]
    public void ReviewCard_BasicSequence_ShouldProduceExpectedIntervals()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = TestHelper.CreateNewCard();
        var reviewDateTime = TestHelper.GetTestDateTime();

        var ratings = new[]
        {
            Rating.Good, Rating.Good, Rating.Good, Rating.Good, Rating.Good,
            Rating.Good, Rating.Again, Rating.Again, Rating.Good, Rating.Good,
            Rating.Good, Rating.Good, Rating.Good
        };

        var expectedIntervals = new[] { 0, 4, 14, 45, 135, 372, 0, 0, 2, 5, 10, 20, 40 };
        var actualIntervals = new List<int>();

        foreach (var rating in ratings)
        {
            (card, _) = scheduler.ReviewCard(card, rating, reviewDateTime);
            var interval = (card.Due - card.LastReview!.Value).Days;
            actualIntervals.Add(interval);
            reviewDateTime = card.Due;
        }

        Assert.Equal(expectedIntervals, actualIntervals);
    }

    [Fact]
    public void ReviewCard_RepeatedCorrectReviews_ShouldMaintainMinimumDifficulty()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = TestHelper.CreateNewCard();

        var reviewDateTimes = Enumerable.Range(0, 10)
            .Select(i => new DateTime(2022, 11, 29, 12, 30, 0, DateTimeKind.Utc).AddMicroseconds(i))
            .ToArray();

        foreach (var reviewDateTime in reviewDateTimes)
        {
            (card, _) = scheduler.ReviewCard(card, Rating.Easy, reviewDateTime);
        }

        // Sử dụng precision 1 decimal place
        Assert.Equal(1.0, card.Difficulty!.Value, 1);
    }

    [Fact]
    public void ReviewCard_WithDefaultDateTime_ShouldUseCurrent()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = TestHelper.CreateNewCard();

        (card, _) = scheduler.ReviewCard(card, Rating.Good);

        var timeDelta = card.Due - DateTime.UtcNow;
        Assert.True(timeDelta.TotalSeconds > 500); // Due in approximately 8-10 minutes
    }

    [Fact]
    public void ReviewCard_MemoState_ShouldProduceExpectedStabilityAndDifficulty()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = TestHelper.CreateNewCard();
        var reviewDateTime = TestHelper.GetTestDateTime();

        var ratings = new[] { Rating.Again, Rating.Good, Rating.Good, Rating.Good, Rating.Good, Rating.Good };
        var intervals = new[] { 0, 0, 1, 3, 8, 21 };

        for (int i = 0; i < ratings.Length; i++)
        {
            reviewDateTime = reviewDateTime.AddDays(intervals[i]);
            (card, _) = scheduler.ReviewCard(card, ratings[i], reviewDateTime);
        }

        (card, _) = scheduler.ReviewCard(card, Rating.Good, reviewDateTime);

        Assert.Equal(49.4472, Math.Round(card.Stability!.Value, 4));
        Assert.Equal(6.8271, Math.Round(card.Difficulty!.Value, 4));
    }
}