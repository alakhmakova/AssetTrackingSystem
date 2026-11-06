using AssetTrackingSystem.Exceptions;
using AssetTrackingSystem.Models;

namespace AssetTrackingSystem.Tests;

// Test names follow the pattern: MethodName_Scenario_ExpectedResult
public class CurrencyRateTests
{
    [Fact]
    public void Constructor_ValidData_SetsCodeAndRate()
    {
        // Arrange + Act
        var rate = new CurrencyRate("SEK", 11.25m);

        // Assert
        Assert.Equal("SEK", rate.CurrencyCode);
        Assert.Equal(11.25m, rate.RateFromEur);
    }

    // A rate of 0 would make CurrencyConverter divide by zero, a negative rate makes no sense
    [Theory]
    [InlineData(0)]
    [InlineData(-1.5)]
    public void Constructor_RateZeroOrNegative_ThrowsValidationException(double rate)
    {
        // InlineData can't hold decimal, so we pass double and convert
        Assert.Throws<ValidationException>(() => new CurrencyRate("SEK", (decimal)rate));
    }

    // Without a code the converter can never find this rate
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyCurrencyCode_ThrowsValidationException(string currencyCode)
    {
        Assert.Throws<ValidationException>(() => new CurrencyRate(currencyCode, 11.25m));
    }
}
