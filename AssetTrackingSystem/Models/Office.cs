namespace AssetTrackingSystem.Models;

public class Office
{
    public Office(string name, Currency currency)
    {
        Name = name;
        Currency = currency;
    }

    public string Name { get; }
    public Currency Currency { get; }
}
