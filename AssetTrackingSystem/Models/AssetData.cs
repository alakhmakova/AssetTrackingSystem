using System;

namespace AssetTrackingSystem.Models;

public class AssetData
{
    public int NextId { get; set; } = 1;
    public List<Asset> Assets { get; set; } = new();
}
