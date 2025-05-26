using FSRS.Core.Constants;
using FSRS.Core.Enums;
using FSRS.Core.Interfaces;
using FSRS.Core.Models;

namespace FSRS.Core.Services;

/// <summary>
/// Main scheduler implementation for FSRS spaced repetition system
/// </summary>
public class Scheduler : IScheduler
{
    private readonly IRetrievabilityCalculator _retrievabilityCalculator;
    private readonly IStabilityCalculator _stabilityCalculator;
    private readonly IDifficultyCalculator _difficultyCalculator;
    private readonly IIntervalCalculator _intervalCalculator;
    private readonly IFuzzingService _fuzzingService;

    /// <inheritdoc/>
    public double[] Parameters { get; }

    /// <inheritdoc/>
    public double DesiredRetention { get; }

    /// <inheritdoc/>
    public TimeSpan[] LearningSteps { get; }

    /// <inheritdoc/>
    public TimeSpan[] RelearningSteps { get; }

    /// <inheritdoc/>
    public int MaximumInterval { get; }

    /// <inheritdoc/>
    public bool EnableFuzzing { get; }


    /// <summary>
    /// Creates a new FSRS scheduler with specified configuration
    /// </summary>
    /// <param name="parameters">Custom FSRS parameters (uses defaults if null)</param>
    /// <param name="desiredRetention">Target retention rate (default: 0.9)</param>
    /// <param name="learningSteps">Learning intervals (default: 1min, 10min)</param>
    /// <param name="relearningSteps">Relearning intervals (default: 10min)</param>
    /// <param name="maximumInterval">Max interval in days (default: 36500)</param>
    /// <param name="enableFuzzing">Enable interval randomization (default: true)</param>
    /// <param name="retrievabilityCalculator">Custom retrievability calculator</param>
    /// <param name="stabilityCalculator">Custom stability calculator</param>
    /// <param name="difficultyCalculator">Custom difficulty calculator</param>
    /// <param name="intervalCalculator">Custom interval calculator</param>
    /// <param name="fuzzingService">Custom fuzzing service</param>
    public Scheduler(
        double desiredRetention = 0.9,
        double[]? parameters = null,
        TimeSpan[]? learningSteps = null,
        TimeSpan[]? relearningSteps = null,
        int maximumInterval = 36500,
        bool enableFuzzing = true,
        IRetrievabilityCalculator? retrievabilityCalculator = null,
        IStabilityCalculator? stabilityCalculator = null,
        IDifficultyCalculator? difficultyCalculator = null,
        IIntervalCalculator? intervalCalculator = null,
        IFuzzingService? fuzzingService = null)
    {
        Parameters = parameters ?? FsrsConstants.DefaultParameters;
        DesiredRetention = desiredRetention;
        LearningSteps = learningSteps ?? [TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(10)];
        RelearningSteps = relearningSteps ?? [TimeSpan.FromMinutes(10)];
        MaximumInterval = maximumInterval;
        EnableFuzzing = enableFuzzing;

        _retrievabilityCalculator = retrievabilityCalculator ?? new RetrievabilityCalculator();
        _stabilityCalculator = stabilityCalculator ?? new StabilityCalculator();
        _difficultyCalculator = difficultyCalculator ?? new DifficultyCalculator();
        _intervalCalculator = intervalCalculator ?? new IntervalCalculator();
        _fuzzingService = fuzzingService ?? new FuzzingService();
    }

    /// <inheritdoc/>
    public double GetCardRetrievability(Card card, DateTime? currentDateTime = null)
    {
        return _retrievabilityCalculator.CalculateRetrievability(card, currentDateTime, Parameters);
    }

    /// <inheritdoc/>
    public (Card UpdatedCard, ReviewLog ReviewLog) ReviewCard(Card card,
        Rating rating,
        DateTime? reviewDateTime = null,
        int? reviewDuration = null)
    {
        reviewDateTime ??= DateTime.UtcNow;

        if (reviewDateTime.Value.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Review datetime must be UTC");

        var updatedCard = card.Clone();

        var daysSinceLastReview = updatedCard.LastReview != null
            ? (reviewDateTime.Value - updatedCard.LastReview.Value).Days
            : (int?)null;

        ProcessCardReview(updatedCard, rating, reviewDateTime.Value, daysSinceLastReview);

        var reviewLog = new ReviewLog(updatedCard.CardId, rating, reviewDateTime.Value, reviewDuration);

        return (updatedCard, reviewLog);
    }

    private void ProcessCardReview(Card card, Rating rating, DateTime reviewDateTime, int? daysSinceLastReview)
    {
        UpdateCardParameters(card, rating, reviewDateTime, daysSinceLastReview);
        var nextInterval = CalculateNextInterval(card, rating);

        if (EnableFuzzing && card.State == State.Review)
        {
            nextInterval = _fuzzingService.ApplyFuzzing(nextInterval, MaximumInterval);
        }

        card.Due = reviewDateTime + nextInterval;
        card.LastReview = reviewDateTime;
    }

    private void UpdateCardParameters(Card card, Rating rating, DateTime reviewDateTime, int? daysSinceLastReview)
    {
        if (card.State == State.Learning && card.Stability == null && card.Difficulty == null)
        {
            card.Stability = _stabilityCalculator.CalculateInitialStability(rating, Parameters);
            card.Difficulty = _difficultyCalculator.CalculateInitialDifficulty(rating, Parameters);
        }
        else if (daysSinceLastReview != null && daysSinceLastReview < 1)
        {
            card.Stability = _stabilityCalculator.CalculateShortTermStability(card.Stability!.Value, rating, Parameters);
            card.Difficulty = _difficultyCalculator.CalculateNextDifficulty(card.Difficulty!.Value, rating, Parameters);
        }
        else
        {
            var retrievability = GetCardRetrievability(card, reviewDateTime);
            card.Stability = _stabilityCalculator.CalculateNextStability(
                card.Difficulty!.Value, card.Stability!.Value, retrievability, rating, Parameters);
            card.Difficulty = _difficultyCalculator.CalculateNextDifficulty(card.Difficulty.Value, rating, Parameters);
        }
    }

    private TimeSpan CalculateNextInterval(Card card, Rating rating)
    {
        return card.State switch
        {
            State.Learning => CalculateLearningInterval(card, rating),
            State.Review => CalculateReviewInterval(card, rating),
            State.Relearning => CalculateRelearningInterval(card, rating),
            _ => throw new ArgumentException($"Unknown card state: {card.State}")
        };
    }

    private TimeSpan CalculateLearningInterval(Card card, Rating rating)
    {
        if (LearningSteps.Length == 0 || (card.Step >= LearningSteps.Length && rating != Rating.Again))
        {
            card.State = State.Review;
            card.Step = null;
            var days = _intervalCalculator.CalculateNextInterval(card.Stability!.Value, DesiredRetention, Parameters, MaximumInterval);
            return TimeSpan.FromDays(days);
        }

        return rating switch
        {
            Rating.Again => HandleAgainRating(card, LearningSteps),
            Rating.Hard => HandleHardRating(card, LearningSteps),
            Rating.Good => HandleGoodRating(card, LearningSteps, State.Review),
            Rating.Easy => HandleEasyRating(card),
            _ => throw new ArgumentException($"Unknown rating: {rating}")
        };
    }

    private TimeSpan CalculateReviewInterval(Card card, Rating rating)
    {
        if (rating == Rating.Again)
        {
            if (RelearningSteps.Length == 0)
            {
                var days = _intervalCalculator.CalculateNextInterval(card.Stability!.Value, DesiredRetention, Parameters, MaximumInterval);
                return TimeSpan.FromDays(days);
            }

            card.State = State.Relearning;
            card.Step = 0;
            return RelearningSteps[0];
        }

        var intervalDays = _intervalCalculator.CalculateNextInterval(card.Stability!.Value, DesiredRetention, Parameters, MaximumInterval);
        return TimeSpan.FromDays(intervalDays);
    }

    private TimeSpan CalculateRelearningInterval(Card card, Rating rating)
    {
        if (RelearningSteps.Length == 0 || (card.Step >= RelearningSteps.Length && rating != Rating.Again))
        {
            card.State = State.Review;
            card.Step = null;
            var days = _intervalCalculator.CalculateNextInterval(card.Stability!.Value, DesiredRetention, Parameters, MaximumInterval);
            return TimeSpan.FromDays(days);
        }

        return rating switch
        {
            Rating.Again => HandleAgainRating(card, RelearningSteps),
            Rating.Hard => HandleHardRating(card, RelearningSteps),
            Rating.Good => HandleGoodRating(card, RelearningSteps, State.Review),
            Rating.Easy => HandleEasyRating(card),
            _ => throw new ArgumentException($"Unknown rating: {rating}")
        };
    }

    private TimeSpan HandleAgainRating(Card card, TimeSpan[] steps)
    {
        card.Step = 0;
        return steps[0];
    }

    private TimeSpan HandleHardRating(Card card, TimeSpan[] steps)
    {
        if (card.Step == 0 && steps.Length == 1)
            return TimeSpan.FromTicks((long)(steps[0].Ticks * 1.5));

        if (card.Step == 0 && steps.Length >= 2)
            return TimeSpan.FromTicks((steps[0].Ticks + steps[1].Ticks) / 2);

        return steps[card.Step!.Value];
    }

    private TimeSpan HandleGoodRating(Card card, TimeSpan[] steps, State nextState)
    {
        if (card.Step + 1 == steps.Length)
        {
            card.State = nextState;
            card.Step = null;
            var days = _intervalCalculator.CalculateNextInterval(card.Stability!.Value, DesiredRetention, Parameters, MaximumInterval);
            return TimeSpan.FromDays(days);
        }

        card.Step++;
        return steps[card.Step.Value];
    }

    private TimeSpan HandleEasyRating(Card card)
    {
        card.State = State.Review;
        card.Step = null;
        var days = _intervalCalculator.CalculateNextInterval(card.Stability!.Value, DesiredRetention, Parameters, MaximumInterval);
        return TimeSpan.FromDays(days);
    }
}