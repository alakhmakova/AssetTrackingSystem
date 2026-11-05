namespace AssetTrackingSystem.Exceptions;

public class CurrencyRateNotFoundException : Exception
{
    public CurrencyRateNotFoundException()
        : base("Exchange rate not found.") { }

    public CurrencyRateNotFoundException(string message)
        : base(message) { }

    public CurrencyRateNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
