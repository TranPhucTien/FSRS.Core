using FSRS.Core.Enums;
using FSRS.Core.Interfaces;
using FSRS.Core.Models;
using FSRS.Core.Services;
using Helpers;

namespace Services.SchedulerTests;

public class SchedulerRelearningTests
{
    [Fact]
    public void Relearning_AgainRating_ShouldStayAtFirstStep()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = CreateRelearningCard(scheduler);

        Assert.Equal(State.Relearning, card.State);
        Assert.Equal(0, card.Step);

        var prevDue = card.Due;
        (card, _) = scheduler.ReviewCard(card, Rating.Again, card.Due);

        Assert.Equal(State.Relearning, card.State);
        Assert.Equal(0, card.Step);
        Assert.Equal(10, Math.Round((card.Due - prevDue).TotalSeconds / 60)); // 10 minutes
    }

    [Fact]
    public void Relearning_GoodRating_ShouldReturnToReview()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = CreateRelearningCard(scheduler);

        var prevDue = card.Due;
        (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);

        Assert.Equal(State.Review, card.State);
        Assert.Null(card.Step);
        Assert.True(Math.Round((card.Due - prevDue).TotalSeconds / 3600) >= 24); // At least 1 day
    }

    [Fact]
    public void NoRelearningSteps_ShouldStayInReview()
    {
        var scheduler = new Scheduler(relearningSteps: Array.Empty<TimeSpan>(), enableFuzzing: false);
        var card = TestHelper.CreateNewCard();

        Assert.Empty(scheduler.RelearningSteps);

        // Progress to Review state
        (card, _) = scheduler.ReviewCard(card, Rating.Good, DateTime.UtcNow);
        (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);

        Assert.Equal(State.Review, card.State);

        (card, _) = scheduler.ReviewCard(card, Rating.Again, card.Due);

        Assert.Equal(State.Review, card.State);
        var interval = (card.Due - card.LastReview!.Value).Days;
        Assert.True(interval >= 1);
    }

    private static Card CreateRelearningCard(IScheduler scheduler)
    {
        var card = TestHelper.CreateNewCard();

        // Progress to Review state
        (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);
        (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);
        (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);

        // Move to Relearning
        (card, _) = scheduler.ReviewCard(card, Rating.Again, card.Due);

        return card;
    }
}