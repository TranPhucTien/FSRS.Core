using FSRS.Core.Enums;
using FSRS.Core.Models;
using FSRS.Core.Services;

namespace Services.SchedulerTests;

public class SchedulerConfigurationTests
{
    [Fact]
    public void Scheduler_CustomParameters_ShouldSetCorrectly()
    {
        var customParameters = new double[]
        {
            0.1456, 0.4186, 1.1104, 4.1315, 5.2417, 1.3098, 0.8975, 0.0000,
            1.5674, 0.0567, 0.9661, 2.0275, 0.1592, 0.2446, 1.5071, 0.2272,
            2.8755, 1.234, 5.6789, 0.1437, 0.2
        };
        var desiredRetention = 0.85;
        var maximumInterval = 3650;

        var scheduler = new Scheduler(
            parameters: customParameters,
            desiredRetention: desiredRetention,
            maximumInterval: maximumInterval,
            enableFuzzing: false
        );

        Assert.Equal(customParameters, scheduler.Parameters);
        Assert.Equal(desiredRetention, scheduler.DesiredRetention);
        Assert.Equal(maximumInterval, scheduler.MaximumInterval);
        Assert.False(scheduler.EnableFuzzing);
    }

    [Fact]
    public void Scheduler_CustomLearningSteps_ShouldHandleMultipleSchedulers()
    {
        var twoStepScheduler = new Scheduler(
            learningSteps: new[] { TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(10) },
            enableFuzzing: false
        );
        var oneStepScheduler = new Scheduler(
            learningSteps: new[] { TimeSpan.FromMinutes(1) },
            enableFuzzing: false
        );
        var noStepScheduler = new Scheduler(
            learningSteps: Array.Empty<TimeSpan>(),
            enableFuzzing: false
        );

        Assert.Equal(2, twoStepScheduler.LearningSteps.Length);
        Assert.Single(oneStepScheduler.LearningSteps);
        Assert.Empty(noStepScheduler.LearningSteps);
    }

    [Fact]
    public void Scheduler_MaximumInterval_ShouldEnforceLimit()
    {
        var maximumInterval = 100;
        var scheduler = new Scheduler(maximumInterval: maximumInterval, enableFuzzing: false);
        var card = new Card();

        var ratings = new[] { Rating.Easy, Rating.Good, Rating.Easy, Rating.Good };

        foreach (var rating in ratings)
        {
            (card, _) = scheduler.ReviewCard(card, rating, card.Due);
            Assert.True((card.Due - card.LastReview!.Value).Days <= scheduler.MaximumInterval);
        }
    }
}