namespace AssetTrackingSystem.Models;

public class CurrencyRate
{
    public CurrencyRate(string currencyCode, decimal rateFromEur)
    {
        CurrencyCode = currencyCode;
        RateFromEur = rateFromEur;
    }

    public string CurrencyCode { get; } // SEK, USD, EUR, TRY
    public decimal RateFromEur { get; }
}
