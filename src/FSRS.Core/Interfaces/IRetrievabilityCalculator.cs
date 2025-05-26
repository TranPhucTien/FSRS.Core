using FSRS.Core.Models;

namespace FSRS.Core.Interfaces;

/// <summary>
/// Calculates the probability of successfully recalling a card
/// </summary>
public interface IRetrievabilityCalculator
{
    /// <summary>
    /// Calculates the current retrievability (recall probability) of a card
    /// </summary>
    /// <param name="card">The card to calculate retrievability for</param>
    /// <param name="currentDateTime">Current time (defaults to now)</param>
    /// <param name="parameters">FSRS parameters (defaults to standard parameters)</param>
    /// <returns>Retrievability value between 0 and 1</returns>
    double CalculateRetrievability(Card card, DateTime? currentDateTime = null, double[]? parameters = null);
}