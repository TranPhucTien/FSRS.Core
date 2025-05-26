using FSRS.Core.Constants;
using FSRS.Core.Enums;
using FSRS.Core.Interfaces;

namespace FSRS.Core.Services;

/// <inheritdoc/>
public class StabilityCalculator : IStabilityCalculator
{
    /// <inheritdoc/>
    public double CalculateInitialStability(Rating rating, double[] parameters)
    {
        var initialStability = parameters[(int)rating - 1];
        return ClampStability(initialStability);
    }

    /// <inheritdoc/>
    public double CalculateShortTermStability(double stability, Rating rating, double[] parameters)
    {
        var shortTermStabilityIncrease = Math.Exp(parameters[17] * ((int)rating - 3 + parameters[18]))
                                       * Math.Pow(stability, -parameters[19]);

        if (rating is Rating.Good or Rating.Easy)
        {
            shortTermStabilityIncrease = Math.Max(shortTermStabilityIncrease, 1.0);
        }

        var shortTermStability = stability * shortTermStabilityIncrease;
        return ClampStability(shortTermStability);
    }

    /// <inheritdoc/>
    public double CalculateNextStability(double difficulty, double stability, double retrievability, Rating rating, double[] parameters)
    {
        var nextStability = rating == Rating.Again
            ? CalculateNextForgetStability(difficulty, stability, retrievability, parameters)
            : CalculateNextRecallStability(difficulty, stability, retrievability, rating, parameters);

        return ClampStability(nextStability);
    }

    private double CalculateNextForgetStability(double difficulty, double stability, double retrievability, double[] parameters)
    {
        var longTermParams = parameters[11]
                           * Math.Pow(difficulty, -parameters[12])
                           * (Math.Pow(stability + 1, parameters[13]) - 1)
                           * Math.Exp((1 - retrievability) * parameters[14]);

        var shortTermParams = stability / Math.Exp(parameters[17] * parameters[18]);

        return Math.Min(longTermParams, shortTermParams);
    }

    private double CalculateNextRecallStability(double difficulty, double stability, double retrievability, Rating rating, double[] parameters)
    {
        var hardPenalty = rating == Rating.Hard ? parameters[15] : 1;
        var easyBonus = rating == Rating.Easy ? parameters[16] : 1;

        return stability * (1 + Math.Exp(parameters[8]) * (11 - difficulty)
                          * Math.Pow(stability, -parameters[9])
                          * (Math.Exp((1 - retrievability) * parameters[10]) - 1)
                          * hardPenalty * easyBonus);
    }

    private static double ClampStability(double stability)
    {
        return Math.Max(stability, FsrsConstants.StabilityMin);
    }
}