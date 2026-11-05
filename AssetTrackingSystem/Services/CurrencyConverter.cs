using AssetTrackingSystem.Models;
using AssetTrackingSystem.Exceptions;
namespace AssetTrackingSystem.Services;

public class CurrencyConverter
{
    private readonly List<CurrencyRate> _rates;

    public CurrencyConverter(List<CurrencyRate> rates)
    {
        _rates = rates;
    }

    private decimal GetRate(Currency currency)
    {
        if (currency == Currency.EUR)
        {
            return 1;
        }
        // Currency.SEK.ToString() gives "SEK"
        var rate = _rates.FirstOrDefault(r => r.CurrencyCode == currency.ToString());
        if (rate == null)
        {
            throw new CurrencyRateNotFoundException($"Exchange rate for {currency} not found.");
        }
        return rate.RateFromEur;
    }

    public decimal Convert(decimal amount, Currency from, Currency to)
    {
        if (from == to)
        {
            return amount;
        }
        return amount / GetRate(from) * GetRate(to);
    }
}
