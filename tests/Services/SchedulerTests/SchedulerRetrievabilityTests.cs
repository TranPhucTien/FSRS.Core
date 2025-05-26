using FSRS.Core.Enums;
using Helpers;

namespace Services.SchedulerTests;

public class SchedulerRetrievabilityTests
{
    [Fact]
    public void GetCardRetrievability_NewCard_ShouldReturnZero()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = TestHelper.CreateNewCard();

        Assert.Equal(State.Learning, card.State);
        var retrievability = scheduler.GetCardRetrievability(card);
        Assert.Equal(0, retrievability);
    }

    [Fact]
    public void GetCardRetrievability_LearningCard_ShouldReturnValidRange()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = TestHelper.CreateNewCard();

        (card, _) = scheduler.ReviewCard(card, Rating.Good);
        Assert.Equal(State.Learning, card.State);

        var retrievability = scheduler.GetCardRetrievability(card);
        Assert.InRange(retrievability, 0, 1);
    }

    [Fact]
    public void GetCardRetrievability_ReviewCard_ShouldReturnValidRange()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = TestHelper.CreateNewCard();

        (card, _) = scheduler.ReviewCard(card, Rating.Good);
        (card, _) = scheduler.ReviewCard(card, Rating.Good);
        Assert.Equal(State.Review, card.State);

        var retrievability = scheduler.GetCardRetrievability(card);
        Assert.InRange(retrievability, 0, 1);
    }

    [Fact]
    public void GetCardRetrievability_RelearningCard_ShouldReturnValidRange()
    {
        var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
        var card = TestHelper.CreateNewCard();

        // Progress to Review then to Relearning
        (card, _) = scheduler.ReviewCard(card, Rating.Good);
        (card, _) = scheduler.ReviewCard(card, Rating.Good);
        (card, _) = scheduler.ReviewCard(card, Rating.Again);
        Assert.Equal(State.Relearning, card.State);

        var retrievability = scheduler.GetCardRetrievability(card);
        Assert.InRange(retrievability, 0, 1);
    }
}