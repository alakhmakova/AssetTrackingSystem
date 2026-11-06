using AssetTrackingSystem.Exceptions;
namespace AssetTrackingSystem.Models;

public class CurrencyRate
{
    public CurrencyRate(string currencyCode, decimal rateFromEur)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            throw new ValidationException("Currency code can't be empty");
        }
        if (rateFromEur <= 0)
        {
            throw new ValidationException("Currency rate must be greater than 0");
        }
        CurrencyCode = currencyCode;
        RateFromEur = rateFromEur;
    }

    public string CurrencyCode { get; } // SEK, USD, EUR, TRY
    public decimal RateFromEur { get; }
}
