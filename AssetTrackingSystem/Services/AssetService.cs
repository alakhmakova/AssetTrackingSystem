using AssetTrackingSystem.Models;
namespace AssetTrackingSystem.Services;

public class AssetService
{
    private readonly List<Asset> _assets = new();
    // Next id to give out; ids are never reused, even after a delete
    private int _nextId = 1;

    // true if the list has at least one asset
    public bool HasAssets => _assets.Count > 0;
    // Give the asset the next id and add it to the list

    public void AddAsset(Asset asset)
    {
        asset.Id = _nextId;
        _nextId++;
        _assets.Add(asset);
    }

    // Sorted by office, then by purchase date
    public List<Asset> GetAssetsSortedByOfficeAndDate()
    {
        return _assets.OrderBy(a => a.Office.Name).ThenBy(a => a.PurchaseDate).ToList();
    }

    // Sorted by type, then by purchase date
    public List<Asset> GetAssetsSortedByTypeAndDate()
    {
        return _assets.OrderBy(a => a.Type).ThenBy(a => a.PurchaseDate).ToList();
    }
    // Brand or model contains the text (ignore case); throw AssetNotFoundException if nothing is found
    public List<Asset> SearchAssets(string text) => throw new NotImplementedException();

    // Throw AssetNotFoundException if there is no asset with this id
    public Asset GetAssetById(int id) => throw new NotImplementedException();

    // Throw AssetNotFoundException if there is no asset with this id
    public void RemoveAssetById(int id) => throw new NotImplementedException();
}
