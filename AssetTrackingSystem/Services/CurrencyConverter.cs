using AssetTrackingSystem.Models;
using AssetTrackingSystem.Exceptions;
using System.Globalization;
using System.Xml;
namespace AssetTrackingSystem.Services;

public class CurrencyConverter
{
    private const string RatesUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml"; // Exchange rate XML document
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

    public static List<CurrencyRate> FetchRates()
    {
        List<CurrencyRate> rates = new();
        // reader will be closed automatically when the method returns, even when exception is thrown
        using XmlTextReader reader = new XmlTextReader(RatesUrl);
        while (reader.Read()) // Goes through the XML document and saves the currency exchange rates to the local list
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == "currency") // Identifies each currency attribute and saves the currency code and rate as an object
                    {
                        string currencyCode = reader.Value;

                        reader.MoveToNextAttribute();
                        decimal rate = decimal.Parse(reader.Value, CultureInfo.InvariantCulture);
                        rates.Add(new CurrencyRate(currencyCode, rate));
                    }
                }
            }
        }
        return rates;
    }
}
