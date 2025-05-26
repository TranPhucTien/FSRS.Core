namespace FSRS.Core.Enums;

/// <summary>
/// Represents the user's performance rating for a card review
/// </summary>
public enum Rating
{
    /// <summary>
    /// Complete failure to recall - card needs immediate review
    /// </summary>
    Again = 1,

    /// <summary>
    /// Difficult to recall but eventually remembered
    /// </summary>
    Hard = 2,

    /// <summary>
    /// Normal recall with appropriate effort
    /// </summary>
    Good = 3,

    /// <summary>
    /// Effortless recall - card was too easy
    /// </summary>
    Easy = 4
}