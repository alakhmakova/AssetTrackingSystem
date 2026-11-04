using AssetTrackingSystem.Models;
using AssetTrackingSystem.Services;

namespace AssetTrackingSystem.Tests;

// Test names follow the pattern: MethodName_Scenario_ExpectedResult
public class AssetServiceTests
{
    private static Office SwedenOffice() => new("Sweden", Currency.SEK);
    private static Office UsaOffice() => new("USA", Currency.USD);
    private static Office TurkeyOffice() => new("Turkey", Currency.TRY);

    // ===== Test case 1: AddAsset gives ids 1, 2 =====
    [Fact]
    public void AddAsset_TwoAssets_GetsIdsOneAndTwo()
    {
        // Arrange: a new service for every test, so tests don't affect each other
        var service = new AssetService();
        var first = new Computer("Dell", "XPS 13", new DateTime(2024, 5, 20), 1200m, SwedenOffice());
        var second = new Computer("HP", "EliteBook 840", new DateTime(2025, 9, 1), 1100m, SwedenOffice());

        // Act
        service.AddAsset(first);
        service.AddAsset(second);

        // Assert
        Assert.Equal(1, first.Id);
        Assert.Equal(2, second.Id);
    }

    // ===== Test case 2: sorting =====
    [Fact]
    public void GetAssetsSortedByTypeAndDate_MixedAssets_SortedByTypeThenDate()
    {
        // Arrange: added in "wrong" order on purpose
        var service = new AssetService();
        service.AddAsset(new MobilePhone("Samsung", "Galaxy S23", new DateTime(2024, 11, 1), 950m, SwedenOffice()));
        service.AddAsset(new Computer("Lenovo", "ThinkPad X1", new DateTime(2025, 1, 5), 1300m, UsaOffice()));
        service.AddAsset(new Computer("Apple", "MacBook Pro", new DateTime(2023, 8, 10), 1500m, TurkeyOffice()));

        // Act
        List<Asset> result = service.GetAssetsSortedByTypeAndDate();

        // Assert: computers first (older first), then the phone
        Assert.Equal("MacBook Pro", result[0].Model);
        Assert.Equal("ThinkPad X1", result[1].Model);
        Assert.Equal("Galaxy S23", result[2].Model);
    }

    [Fact]
    public void GetAssetsSortedByOfficeAndDate_MixedAssets_SortedByOffice()
    {
        // Arrange: added in "wrong" order on purpose
        var service = new AssetService();
        service.AddAsset(new MobilePhone("Samsung", "Galaxy S23", new DateTime(2024, 11, 1), 950m, SwedenOffice()));
        service.AddAsset(new Computer("Lenovo", "ThinkPad X1", new DateTime(2025, 1, 5), 1300m, UsaOffice()));
        service.AddAsset(new Computer("Apple", "MacBook Pro", new DateTime(2023, 8, 10), 1500m, TurkeyOffice()));

        // Act
        List<Asset> result = service.GetAssetsSortedByOfficeAndDate();

        // Assert: Sweden first , then Turkey and Usa
        Assert.Equal("Galaxy S23", result[0].Model);
        Assert.Equal("MacBook Pro", result[1].Model);
        Assert.Equal("ThinkPad X1", result[2].Model);
    }

    [Fact]
    public void GetAssetsSortedByOfficeAndDate_MixedAssets_SortedByOfficeThenDate()
    {
        // Arrange: added in "wrong" order on purpose
        var service = new AssetService();
        service.AddAsset(new MobilePhone("Samsung", "Galaxy S23", new DateTime(2024, 11, 1), 950m, SwedenOffice()));
        service.AddAsset(new Computer("Lenovo", "ThinkPad X1", new DateTime(2025, 1, 5), 1300m, SwedenOffice()));
        service.AddAsset(new Computer("Apple", "MacBook Pro", new DateTime(2023, 8, 10), 1500m, SwedenOffice()));

        // Act
        List<Asset> result = service.GetAssetsSortedByOfficeAndDate();

        // Assert: oldest date first
        Assert.Equal("MacBook Pro", result[0].Model);
        Assert.Equal("Galaxy S23", result[1].Model);
        Assert.Equal("ThinkPad X1", result[2].Model);
    }

    [Fact]
    public void GetAssetsSortedByTypeAndDate_NoAssets_ReturnsEmptyList()
    {
        var service = new AssetService();

        List<Asset> result = service.GetAssetsSortedByTypeAndDate();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAssetsSortedByOfficeAndDate_NoAssets_ReturnsEmptyList()
    {
        var service = new AssetService();

        List<Asset> result = service.GetAssetsSortedByOfficeAndDate();

        Assert.Empty(result);
    }

    // ===== Test case 3: HasAssets =====
    [Fact]
    public void HasAssets_ListIsNotEmpty_ReturnsTrue()
    {
        // Arrange
        var service = new AssetService();
        var asset = new Computer("Dell", "XPS 13", new DateTime(2024, 5, 20), 1200m, SwedenOffice());

        // Act
        service.AddAsset(asset);

        // Assert
        Assert.True(service.HasAssets);
    }

    [Fact]
    public void HasAssets_ListIsEmpty_ReturnsFalse()
    {
        // Arrange: 
        var service = new AssetService();

        // Assert
        Assert.False(service.HasAssets);
    }
}