using FSRS.Core.Enums;
using Helpers;

namespace Services.SchedulerTests;

public class SchedulerReviewTests
{
    [Fact]
    public void ReviewState_GoodRating_ShouldMaintainReviewState()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = TestHelper.CreateNewCard();

        // Progress to Review state
        (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);
        (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);

        Assert.Equal(State.Review, card.State);
        Assert.Null(card.Step);

        var prevDue = card.Due;
        (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);

        Assert.Equal(State.Review, card.State);
        Assert.True(Math.Round((card.Due - prevDue).TotalSeconds / 3600) >= 24); // At least 1 day
    }

    [Fact]
    public void ReviewState_AgainRating_ShouldMoveToRelearning()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = TestHelper.CreateNewCard();

        // Progress to Review state
        (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);
        (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);
        (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);

        var prevDue = card.Due;
        (card, _) = scheduler.ReviewCard(card, Rating.Again, card.Due);

        Assert.Equal(State.Relearning, card.State);
        Assert.Equal(10, Math.Round((card.Due - prevDue).TotalSeconds / 60)); // 10 minutes
    }
}