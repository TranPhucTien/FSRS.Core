using FSRS.Core.Constants;
using FSRS.Core.Interfaces;
using FSRS.Core.Models;

namespace FSRS.Core.Services;

/// <inheritdoc/>
public class RetrievabilityCalculator : IRetrievabilityCalculator
{
    /// <inheritdoc/>
    public double CalculateRetrievability(Card card, DateTime? currentDateTime = null, double[]? parameters = null)
    {
        if (card.LastReview == null)
            return 0;

        currentDateTime ??= DateTime.UtcNow;
        parameters ??= FsrsConstants.DefaultParameters;

        var decay = -parameters[20];
        var factor = Math.Pow(0.9, 1.0 / decay) - 1;
        var elapsedDays = Math.Max(0, (currentDateTime.Value - card.LastReview.Value).Days);

        return Math.Pow(1 + factor * elapsedDays / card.Stability!.Value, decay);
    }
}