using FSRS.Core.Enums;
using FSRS.Core.Models;
using Helpers;

namespace Models;

public class ReviewLogTests
{
    [Fact]
    public void ReviewLog_Creation_ShouldSetPropertiesCorrectly()
    {
        var cardId = new Guid();
        var rating = Rating.Good;
        var reviewDateTime = TestHelper.GetTestDateTime();
        var reviewDuration = 5000;

        var reviewLog = new ReviewLog(cardId, rating, reviewDateTime, reviewDuration);

        Assert.Equal(cardId, reviewLog.CardId);
        Assert.Equal(rating, reviewLog.Rating);
        Assert.Equal(reviewDateTime, reviewLog.ReviewDateTime);
        Assert.Equal(reviewDuration, reviewLog.ReviewDuration);
    }

    [Fact]
    public void ReviewLog_Creation_WithoutDuration_ShouldHaveNullDuration()
    {
        var cardId = new Guid();
        var rating = Rating.Good;
        var reviewDateTime = TestHelper.GetTestDateTime();

        var reviewLog = new ReviewLog(cardId, rating, reviewDateTime);

        Assert.Equal(cardId, reviewLog.CardId);
        Assert.Equal(rating, reviewLog.Rating);
        Assert.Equal(reviewDateTime, reviewLog.ReviewDateTime);
        Assert.Null(reviewLog.ReviewDuration);
    }

    [Fact]
    public void ReviewLog_ToString_ShouldEqualRepr()
    {
        var reviewLog = new ReviewLog(new Guid(), Rating.Good, TestHelper.GetTestDateTime());

        Assert.Equal(reviewLog.ToString(), reviewLog.ToString());
    }
}