using FSRS.Core.Interfaces;
using FSRS.Core.Extensions;

namespace FSRS.Core.Services;

/// <inheritdoc/>
public class IntervalCalculator : IIntervalCalculator
{
    /// <inheritdoc/>
    public int CalculateNextInterval(double stability, double desiredRetention, double[] parameters, int maximumInterval)
    {
        var decay = -parameters[20];
        var factor = Math.Pow(0.9, 1.0 / decay) - 1;

        var nextInterval = (stability / factor) * (Math.Pow(desiredRetention, 1.0 / decay) - 1);
        var roundedInterval = (int)Math.Round(nextInterval);

        return roundedInterval.Clamp(1, maximumInterval);
    }
}