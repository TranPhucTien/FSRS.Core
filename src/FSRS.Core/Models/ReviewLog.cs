using FSRS.Core.Enums;

namespace FSRS.Core.Models;
/// <summary>
/// Represents a record of a card review session
/// </summary>
public class ReviewLog
{
    /// <summary>
    /// ID of the card that was reviewed
    /// </summary>
    public Guid CardId { get; }

    /// <summary>
    /// Rating given during the review
    /// </summary>
    public Rating Rating { get; }

    /// <summary>
    /// When the review took place
    /// </summary>
    public DateTime ReviewDateTime { get; }

    /// <summary>
    /// How long the review took in milliseconds (optional)
    /// </summary>
    public int? ReviewDuration { get; }

    /// <summary>
    /// Creates a new review log entry
    /// </summary>
    public ReviewLog(
        Guid cardId,
        Rating rating,
        DateTime reviewDateTime,
        int? reviewDuration = null)
    {
        CardId = cardId;
        Rating = rating;
        ReviewDateTime = reviewDateTime;
        ReviewDuration = reviewDuration;
    }
}