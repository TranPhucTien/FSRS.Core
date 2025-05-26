using FSRS.Core.Enums;
using FSRS.Core.Interfaces;
using FSRS.Core.Extensions;

namespace FSRS.Core.Services;

/// <inheritdoc/>
public class DifficultyCalculator : IDifficultyCalculator
{
    /// <inheritdoc/>
    public double CalculateInitialDifficulty(Rating rating, double[] parameters)
    {
        var initialDifficulty = parameters[4] - Math.Exp(parameters[5] * ((int)rating - 1)) + 1;
        return ClampDifficulty(initialDifficulty);
    }

    /// <inheritdoc/>
    public double CalculateNextDifficulty(double difficulty, Rating rating, double[] parameters)
    {
        var arg1 = CalculateInitialDifficulty(Rating.Easy, parameters);
        var deltaDifficulty = -(parameters[6] * ((int)rating - 3));
        var arg2 = difficulty + LinearDamping(deltaDifficulty, difficulty);
        var nextDifficulty = MeanReversion(arg1, arg2, parameters[7]);

        return ClampDifficulty(nextDifficulty);
    }

    private static double LinearDamping(double deltaDifficulty, double difficulty)
    {
        return (10.0 - difficulty) * deltaDifficulty / 9.0;
    }

    private static double MeanReversion(double arg1, double arg2, double parameter)
    {
        return parameter * arg1 + (1 - parameter) * arg2;
    }

    private static double ClampDifficulty(double difficulty)
    {
        return difficulty.Clamp(1.0, 10.0);
    }
}