using FSRS.Core.Enums;
using FSRS.Core.Services;
using Helpers;

namespace Services.SchedulerTests;

public class SchedulerFuzzingTests
{
    [Fact]
    public void Fuzzing_WithDifferentSeeds_ShouldProduceDifferentIntervals()
    {
        // Arrange
        var iterations = 100;

        // Act
        var intervalsWithFuzzing = GenerateReviewIntervals(enableFuzzing: true, iterations: iterations);
        var intervalsWithoutFuzzing = GenerateReviewIntervals(enableFuzzing: false, iterations: iterations);

        // Assert
        Assert.False(
            intervalsWithFuzzing.All(i => i == intervalsWithFuzzing.First()),
            "Expected fuzzed intervals to vary, but all were the same."
        );

        Assert.True(
            intervalsWithoutFuzzing.All(i => i == intervalsWithoutFuzzing.First()),
            "Expected non-fuzzed intervals to be consistent, but they varied."
        );
    }

    [Fact]
    public void Fuzzing_Disabled_ShouldProduceConsistentIntervals()
    {
        var intervals = new List<int>();

        for (int i = 0; i < 5; i++)
        {
            var scheduler = TestHelper.CreateSchedulerWithoutFuzzing();
            var card = TestHelper.CreateNewCard();

            (card, _) = scheduler.ReviewCard(card, Rating.Good, DateTime.UtcNow);
            (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);

            var prevDue = card.Due;
            (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);
            var interval = (card.Due - prevDue).Days;
            intervals.Add(interval);
        }

        // All intervals should be identical when fuzzing is disabled
        Assert.True(intervals.All(i => i == intervals.First()));
    }

    private List<int> GenerateReviewIntervals(bool enableFuzzing, int iterations)
    {
        var intervals = new List<int>();

        for (int i = 0; i < iterations; i++)
        {
            var scheduler = new Scheduler(enableFuzzing: enableFuzzing);
            var card = TestHelper.CreateNewCard();

            // Apply 3 Good reviews
            (card, _) = scheduler.ReviewCard(card, Rating.Good, DateTime.UtcNow);
            (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);

            var prevDue = card.Due;
            (card, _) = scheduler.ReviewCard(card, Rating.Good, card.Due);
            var interval = (card.Due - prevDue).Days;

            intervals.Add(interval);
        }

        return intervals;
    }
}