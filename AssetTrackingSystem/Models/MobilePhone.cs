namespace AssetTrackingSystem.Models;

public class MobilePhone : Asset
{
    public MobilePhone(string brand, string model, DateTime purchaseDate, decimal priceUsd, Office office)
        : base(brand, model, purchaseDate, priceUsd, office)
    {
    }

    public override string Type => "MobilePhone";
}
