using FSRS.Core.Enums;

namespace FSRS.Core.Interfaces;

/// <summary>
/// Calculates memory stability for cards in different states
/// </summary>
public interface IStabilityCalculator
{
    /// <summary>
    /// Calculates initial stability for a new card
    /// </summary>
    double CalculateInitialStability(Rating rating, double[] parameters);

    /// <summary>
    /// Calculates stability for short-term reviews (within same day)
    /// </summary>
    double CalculateShortTermStability(double stability, Rating rating, double[] parameters);

    /// <summary>
    /// Calculates stability for long-term reviews
    /// </summary>
    double CalculateNextStability(double difficulty, double stability, double retrievability, Rating rating, double[] parameters);
}