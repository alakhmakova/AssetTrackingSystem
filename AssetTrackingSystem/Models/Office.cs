using AssetTrackingSystem.Exceptions;
namespace AssetTrackingSystem.Models;

public class Office
{
    public Office(string name, Currency currency)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Office name can't be empty");
        }
        Name = name.Trim();
        Currency = currency;
    }

    public string Name { get; }
    public Currency Currency { get; }
}
