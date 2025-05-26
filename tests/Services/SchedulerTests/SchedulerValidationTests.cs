using FSRS.Core.Constants;
using FSRS.Core.Enums;
using Helpers;

namespace Services.SchedulerTests;

public class SchedulerValidationTests
{
    [Fact]
    public void ReviewCard_NonUtcDateTime_ShouldThrowException()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = TestHelper.CreateNewCard();
        var nonUtcDateTime = new DateTime(2022, 11, 29, 12, 30, 0, DateTimeKind.Local);

        var exception = Assert.Throws<ArgumentException>(() =>
            scheduler.ReviewCard(card, Rating.Good, nonUtcDateTime));

        Assert.Contains("UTC", exception.Message);
    }

    [Fact]
    public void ReviewCard_UtcDateTime_ShouldSetCorrectTimezone()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = TestHelper.CreateNewCard();

        (card, _) = scheduler.ReviewCard(card, Rating.Good, DateTime.UtcNow);

        Assert.Equal(DateTimeKind.Utc, card.Due.Kind);
        Assert.Equal(DateTimeKind.Utc, card.LastReview!.Value.Kind);
        Assert.True(card.Due >= card.LastReview);
    }

    [Fact]
    public void StabilityLowerBound_ShouldAlwaysBeAboveMinimum()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = TestHelper.CreateNewCard();

        for (int i = 0; i < 1000; i++)
        {
            (card, _) = scheduler.ReviewCard(
                card,
                Rating.Again,
                card.Due.AddDays(1)
            );
            Assert.True(card.Stability >= FsrsConstants.StabilityMin);
        }
    }
}