

using FSRS.Core.Models;
using FSRS.Core.Services;

namespace Helpers;

public static class TestHelper
{
    public static Scheduler CreateSchedulerWithoutFuzzing()
    {
        return new Scheduler(enableFuzzing: false);
    }

    public static Card CreateNewCard()
    {
        return new Card();
    }

    public static DateTime GetTestDateTime()
    {
        return new DateTime(2022, 11, 29, 12, 30, 0, DateTimeKind.Utc);
    }

    public static void AssertIntervalEquals(int expected, Card card, DateTime reviewDateTime)
    {
        var actual = (card.Due - card.LastReview!.Value).Days;
        Assert.Equal(expected, actual);
    }
}