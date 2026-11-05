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

    //Sorted by id
    public List<Asset> GetAssetsSortedById()
    {
        return _assets.OrderBy(a => a.Id).ToList();
    }
    // Brand or model contains the text (ignore case); throw AssetNotFoundException if nothing is found
    public List<Asset> SearchAssets(string text) => throw new NotImplementedException();

    // Throw AssetNotFoundException if there is no asset with this id
    public Asset GetAssetById(int id) => throw new NotImplementedException();

    // Throw AssetNotFoundException if there is no asset with this id
    public void RemoveAssetById(int id) => throw new NotImplementedException();

    // Packs what is in memory (the asset list and _nextId) into an AssetData object and returns it.
    // Assets and next id, ready to be saved in the file.
    public AssetData GetData()
    {
        return new AssetData { NextId = _nextId, Assets = GetAssetsSortedById() };
    }

    // Unpacks a ready AssetData back into memory: replaces the asset list and sets _nextId.
    public void LoadData(AssetData data)
    {
        _assets.Clear(); // readonly forbids replacing the list, but its contents can still change
        _assets.AddRange(data.Assets);
        // Never below the biggest existing id + 1, even if the file was edited by hand
        int maxId = _assets.Count > 0 ? _assets.Max(p => p.Id) : 0;
        _nextId = Math.Max(data.NextId, maxId + 1);
    }
}
