using FSRS.Core.Enums;
using FSRS.Core.Models;

namespace FSRS.Core.Interfaces;

/// <summary>
/// Main scheduler interface for FSRS spaced repetition system
/// </summary>
public interface IScheduler
{
    /// <summary>
    /// FSRS algorithm parameters
    /// </summary>
    double[] Parameters { get; }

    /// <summary>
    /// Target retention rate (0-1)
    /// </summary>
    double DesiredRetention { get; }

    /// <summary>
    /// Learning step intervals for new cards
    /// </summary>
    TimeSpan[] LearningSteps { get; }

    /// <summary>
    /// Relearning step intervals for forgotten cards
    /// </summary>
    TimeSpan[] RelearningSteps { get; }

    /// <summary>
    /// Maximum review interval in days
    /// </summary>
    int MaximumInterval { get; }

    /// <summary>
    /// Whether to apply interval fuzzing
    /// </summary>
    bool EnableFuzzing { get; }

    /// <summary>
    /// Gets the current retrievability (recall probability) of a card
    /// </summary>
    /// <param name="card">Card to check</param>
    /// <param name="currentDateTime">Current time (optional)</param>
    /// <returns>Retrievability between 0 and 1</returns>
    double GetCardRetrievability(Card card, DateTime? currentDateTime = null);

    /// <summary>
    /// Processes a card review and returns updated card and review log
    /// </summary>
    /// <param name="card">Card being reviewed</param>
    /// <param name="rating">User's performance rating</param>
    /// <param name="reviewDateTime">When the review occurred (optional)</param>
    /// <param name="reviewDuration">How long the review took in ms (optional)</param>
    /// <returns>Tuple of updated card and review log</returns>
    (Card UpdatedCard, ReviewLog ReviewLog) ReviewCard(Card card, Rating rating, DateTime? reviewDateTime = null, int? reviewDuration = null);
}