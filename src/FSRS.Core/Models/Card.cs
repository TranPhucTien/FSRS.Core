using FSRS.Core.Enums;

namespace FSRS.Core.Models;

/// <summary>
/// Represents a spaced repetition card with scheduling information
/// </summary>
public class Card
{
    /// <summary>
    /// Unique identifier for the card
    /// </summary>
    public Guid CardId { get; set; }

    /// <summary>
    /// Current learning state of the card
    /// </summary>
    public State State { get; set; }

    /// <summary>
    /// Current step in learning/relearning sequence (null for review state)
    /// </summary>
    public int? Step { get; set; }

    /// <summary>
    /// Memory stability in days - how long the card can be remembered
    /// </summary>
    public double? Stability { get; set; }

    /// <summary>
    /// Card difficulty on a scale of 1-10
    /// </summary>
    public double? Difficulty { get; set; }

    /// <summary>
    /// When the card is next due for review
    /// </summary>
    public DateTime Due { get; set; }

    /// <summary>
    /// When the card was last reviewed (null for new cards)
    /// </summary>
    public DateTime? LastReview { get; set; }

    /// <summary>
    /// Creates a new card with default or specified values
    /// </summary>
    public Card(
        Guid? cardId = null,
        State state = State.Learning,
        int? step = null,
        double? stability = null,
        double? difficulty = null,
        DateTime? due = null,
        DateTime? lastReview = null)
    {
        CardId = cardId ?? Guid.NewGuid();
        State = state;
        Step = state == State.Learning && step == null ? 0 : step;
        Stability = stability;
        Difficulty = difficulty;
        Due = due ?? DateTime.UtcNow;
        LastReview = lastReview;
    }

    /// <summary>
    /// Creates a deep copy of the card
    /// </summary>
    public Card Clone()
    {
        return new Card(CardId, State, Step, Stability, Difficulty, Due, LastReview);
    }
}