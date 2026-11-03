
using AssetTrackingSystem.Exceptions;
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
        if (string.IsNullOrWhiteSpace(brand))
        {
            throw new ValidationException("Asset brand can't be empty");
        }
        if (string.IsNullOrWhiteSpace(model))
        {
            throw new ValidationException("Asset model can't be empty");
        }
        if (!IsValidDate(purchaseDate))
        {
            throw new ValidationException("Purchase date can’t be in the future");
        }
        if (!IsValidPrice(priceUsd))
        {
            throw new ValidationException("Price must be greater than 0 with at most 2 decimals");
        }
        if (office is null)
        {
            throw new ValidationException("Office can't be empty");
        }

        Brand = brand;
        Model = model;
        PurchaseDate = purchaseDate;
        PriceUsd = priceUsd;
        Office = office;
    }
    public static bool IsValidDate(DateTime purchaseDate)
    {   //compare date without time
        return purchaseDate.Date <= DateTime.Today;
    }
    public static bool IsValidPrice(decimal price)
    {
        return price > 0 && (Math.Round(price, 2) == price);
    }
}
