using FSRS.Core.Enums;
using FSRS.Core.Services;
using Helpers;

namespace Services.SchedulerTests;

public class SchedulerLearningTests
{
    [Fact]
    public void LearningSteps_GoodRating_ShouldProgressThroughSteps()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var createdAt = DateTime.UtcNow;
        var card = TestHelper.CreateNewCard();

        Assert.Equal(State.Learning, card.State);
        Assert.Equal(0, card.Step);

        (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);

        Assert.Equal(State.Learning, card.State);
        Assert.Equal(1, card.Step);
        Assert.Equal(6, Math.Round((card.Due - createdAt).TotalSeconds / 100)); // ~10 minutes

        (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);

        Assert.Equal(State.Review, card.State);
        Assert.Null(card.Step);
        Assert.True(Math.Round((card.Due - createdAt).TotalSeconds / 3600) >= 24); // Over a day
    }

    [Fact]
    public void LearningSteps_AgainRating_ShouldResetToFirstStep()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var createdAt = DateTime.UtcNow;
        var card = TestHelper.CreateNewCard();

        Assert.Equal(State.Learning, card.State);
        Assert.Equal(0, card.Step);

        (card, _) = scheduler.ReviewCard(card, Rating.Again, card.Due);

        Assert.Equal(State.Learning, card.State);
        Assert.Equal(0, card.Step);
        Assert.Equal(6, Math.Round((card.Due - createdAt).TotalSeconds / 10)); // ~1 minute
    }

    [Fact]
    public void LearningSteps_HardRating_ShouldStayAtSameStep()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var createdAt = DateTime.UtcNow;
        var card = TestHelper.CreateNewCard();

        Assert.Equal(State.Learning, card.State);
        Assert.Equal(0, card.Step);

        (card, _) = scheduler.ReviewCard(card, Rating.Hard, card.Due);

        Assert.Equal(State.Learning, card.State);
        Assert.Equal(0, card.Step);
        Assert.Equal(33, Math.Round((card.Due - createdAt).TotalSeconds / 10)); // ~5.5 minutes
    }

    [Fact]
    public void LearningSteps_EasyRating_ShouldSkipToReview()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var createdAt = DateTime.UtcNow;
        var card = TestHelper.CreateNewCard();

        Assert.Equal(State.Learning, card.State);
        Assert.Equal(0, card.Step);

        (card, _) = scheduler.ReviewCard(card, Rating.Easy, card.Due);

        Assert.Equal(State.Review, card.State);
        Assert.Null(card.Step);
        Assert.True(Math.Round((card.Due - createdAt).TotalSeconds / 86400) >= 1); // At least 1 day
    }

    [Fact]
    public void NoLearningSteps_ShouldSkipDirectlyToReview()
    {
        var scheduler = new Scheduler(learningSteps: Array.Empty<TimeSpan>(), enableFuzzing: false);
        var card = TestHelper.CreateNewCard();

        Assert.Empty(scheduler.LearningSteps);

        (card, _) = scheduler.ReviewCard(card, Rating.Again, DateTime.UtcNow);

        Assert.Equal(State.Review, card.State);
        var interval = (card.Due - card.LastReview!.Value).Days;
        Assert.True(interval >= 1);
    }
}