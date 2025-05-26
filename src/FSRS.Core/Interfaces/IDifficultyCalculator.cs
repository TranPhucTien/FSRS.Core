using FSRS.Core.Enums;

namespace FSRS.Core.Interfaces;

/// <summary>
/// Calculates and updates card difficulty based on performance
/// </summary>
public interface IDifficultyCalculator
{
    /// <summary>
    /// Calculates initial difficulty for a new card
    /// </summary>
    double CalculateInitialDifficulty(Rating rating, double[] parameters);

    /// <summary>
    /// Updates difficulty based on review performance
    /// </summary>
    double CalculateNextDifficulty(double difficulty, Rating rating, double[] parameters);
}