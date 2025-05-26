using FSRS.Core.Extensions;

namespace Extensions;

public class MathExtensionsTests
{
    [Theory]
    [InlineData(5.0, 1.0, 10.0, 5.0)]
    [InlineData(0.5, 1.0, 10.0, 1.0)]
    [InlineData(15.0, 1.0, 10.0, 10.0)]
    [InlineData(-5.0, 0.0, 10.0, 0.0)]
    public void Clamp_Double_ShouldReturnExpectedValue(double value, double min, double max, double expected)
    {
        var result = MathExtensions.Clamp(value, min, max);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(5, 1, 10, 5)]
    [InlineData(0, 1, 10, 1)]
    [InlineData(15, 1, 10, 10)]
    [InlineData(-5, 0, 10, 0)]
    public void Clamp_Int_ShouldReturnExpectedValue(int value, int min, int max, int expected)
    {
        var result = MathExtensions.Clamp(value, min, max);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(5.0, 1.0, 10.0, 5.0)]
    [InlineData(0.5, 1.0, 10.0, 1.0)]
    [InlineData(15.0, 1.0, 10.0, 10.0)]
    public void Clamp_DoubleExtension_ShouldReturnExpectedValue(double value, double min, double max, double expected)
    {
        var result = value.Clamp(min, max);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(5, 1, 10, 5)]
    [InlineData(0, 1, 10, 1)]
    [InlineData(15, 1, 10, 10)]
    public void Clamp_IntExtension_ShouldReturnExpectedValue(int value, int min, int max, int expected)
    {
        var result = value.Clamp(min, max);
        Assert.Equal(expected, result);
    }
}