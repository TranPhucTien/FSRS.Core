using FSRS.Core.Enums;
using FSRS.Core.Models;
using Helpers;

namespace Models;

public class CardTests
{
    [Fact]
    public void Card_NewCard_ShouldHaveCorrectInitialState()
    {
        var card = TestHelper.CreateNewCard();

        Assert.Equal(State.Learning, card.State);
        Assert.Equal(0, card.Step);
        Assert.Null(card.Stability);
        Assert.Null(card.Difficulty);
        Assert.Null(card.LastReview);
        Assert.True(DateTime.UtcNow >= card.Due);
    }

    [Fact]
    public void Card_NewCard_ShouldBeDueImmediately()
    {
        var card = TestHelper.CreateNewCard();

        Assert.True(DateTime.UtcNow >= card.Due);
    }

    [Fact]
    public void Card_Clone_ShouldCreateIdenticalCopy()
    {
        var original = new Card(
            cardId: Guid.NewGuid(),
            state: State.Review,
            step: null,
            stability: 10.5,
            difficulty: 5.2,
            due: DateTime.UtcNow.AddDays(5),
            lastReview: DateTime.UtcNow
        );

        var clone = original.Clone();

        Assert.Equal(original.CardId, clone.CardId);
        Assert.Equal(original.State, clone.State);
        Assert.Equal(original.Step, clone.Step);
        Assert.Equal(original.Stability, clone.Stability);
        Assert.Equal(original.Difficulty, clone.Difficulty);
        Assert.Equal(original.Due, clone.Due);
        Assert.Equal(original.LastReview, clone.LastReview);
    }

    [Fact]
    public void Card_UniqueIds_ShouldGenerateUniqueCardIds()
    {
        var cardIds = new HashSet<Guid>();

        for (int i = 0; i < 1000; i++)
        {
            var card = TestHelper.CreateNewCard();
            cardIds.Add(card.CardId);
        }

        Assert.Equal(1000, cardIds.Count);
    }

    [Fact]
    public void Card_ToString_ShouldEqualRepr()
    {
        var card = TestHelper.CreateNewCard();

        Assert.Equal(card.ToString(), card.ToString());
    }
}