namespace FSRS.Core.Enums;

/// <summary>
/// Represents the current learning state of a card
/// </summary>
public enum State
{
    /// <summary>
    /// Card is being learned for the first time
    /// </summary>
    Learning = 1,

    /// <summary>
    /// Card has graduated to regular review schedule
    /// </summary>
    Review = 2,

    /// <summary>
    /// Card was forgotten and needs to be relearned
    /// </summary>
    Relearning = 3
}