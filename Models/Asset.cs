namespace AssetTrackingSystem.Models;

public abstract class Asset
{

    public int Id { get; internal set; }
    //each child will return the own name 
    public abstract string Type { get; }
    public string Brand { get; }
    public string Model { get; }
    public DateTime PurchaseDate { get; }
    public decimal PriceUsd { get; }
    public Office Office { get; }

    protected Asset(string brand, string model, DateTime purchaseDate, decimal priceUsd, Office office)
    {
        Brand = brand;
        Model = model;
        PurchaseDate = purchaseDate;
        PriceUsd = priceUsd;
        Office = office;
    }
}
