using AssetTrackingSystem.Models;
using AssetTrackingSystem.Exceptions;
using AssetTrackingSystem.Services;

namespace AssetTrackingSystem.Tests;

// Test names follow the pattern: MethodName_Scenario_ExpectedResult
// Converter is created with fake rates, so tests don't depend on the internet or today's real rates
public class CurrencyConverterTests
{
    // Fake rates (units per 1 EUR), easy to calculate; no internet needed
    private static CurrencyConverter CreateConverter() => new(new List<CurrencyRate>
    {
        new("USD", 1.10m),
        new("SEK", 11.00m),
    });

    [Fact]
    public void Convert_UsdToSek_ConvertsViaEur()
    {
        var converter = CreateConverter();

        // 110 USD -> 100 EUR -> 1100 SEK
        decimal result = converter.Convert(110m, Currency.USD, Currency.SEK);

        Assert.Equal(1100m, result);
    }

    [Fact]
    public void Convert_UsdToEur_ReturnsAmountInEur()
    {
        var converter = CreateConverter();

        // 110 USD / 1.10 = 100 EUR (EUR rate is always 1)
        decimal result = converter.Convert(110m, Currency.USD, Currency.EUR);

        Assert.Equal(100m, result);
    }

    [Fact]
    public void Convert_EurToSek_ReturnsAmountInSek()
    {
        var converter = CreateConverter();

        // 100 EUR * 11.00 = 1100 SEK
        decimal result = converter.Convert(100m, Currency.EUR, Currency.SEK);

        Assert.Equal(1100m, result);
    }

    [Fact]
    public void Convert_SameCurrency_ReturnsSameAmount()
    {
        var converter = CreateConverter();

        // Same currency: the amount is returned unchanged
        decimal result = converter.Convert(500m, Currency.SEK, Currency.SEK);

        Assert.Equal(500m, result);
    }

    [Fact]
    public void Convert_CurrencyRateMissing_ThrowsCurrencyRateNotFoundException()
    {
        var converter = CreateConverter();

        // There is no TRY in the fake rates -> the converter can't find its rate
        Assert.Throws<CurrencyRateNotFoundException>(() => converter.Convert(500m, Currency.TRY, Currency.SEK));
    }
}

