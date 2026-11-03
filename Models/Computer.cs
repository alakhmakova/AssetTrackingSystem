namespace AssetTrackingSystem.Models;

public class Computer : Asset
{
    public Computer(string brand, string model, DateTime purchaseDate, decimal priceUsd, Office office)
        : base(brand, model, purchaseDate, priceUsd, office)
    {
    }

    public override string Type => "Computer";
}
