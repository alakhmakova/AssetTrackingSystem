using AssetTrackingSystem.Models;
using AssetTrackingSystem.Exceptions;
using AssetTrackingSystem.Services;
using System.Security.AccessControl;

namespace AssetTrackingSystem.Tests;

// Test names follow the pattern: MethodName_Scenario_ExpectedResult
public class AssetServiceTests
{
    private static Office SwedenOffice() => new("Sweden", Currency.SEK);
    private static Office UsaOffice() => new("USA", Currency.USD);
    private static Office TurkeyOffice() => new("Turkey", Currency.TRY);

    // ===== Test suite 1: AddAsset gives ids 1, 2 =====
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

    // ===== Test suite 2: sorting =====
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

    // ===== Test suite 3: HasAssets method =====
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
        // Arrange
        var service = new AssetService();

        // Assert
        Assert.False(service.HasAssets);
    }
    // ===== Test suite 4: GetData method to pack the asset list and _nextId into an AssetData object =====
    [Fact]
    public void GetData_NoAssets_ReturnsNextIdOneAndEmptyList()
    {
        // Arrange
        var service = new AssetService();

        // Act
        AssetData data = service.GetData();

        // Assert
        Assert.Equal(1, data.NextId);
        Assert.Empty(data.Assets);
    }

    [Fact]
    public void GetData_TwoAssets_ReturnsAllAssetsAndNextIdThree()
    {
        // Arrange
        var service = new AssetService();
        var first = new MobilePhone("Samsung", "Galaxy S26 Plus", new DateTime(2026, 11, 5), 1500m, TurkeyOffice());
        var second = new Computer("HP", "EliteBook 840", new DateTime(2025, 9, 1), 1100m, UsaOffice());
        service.AddAsset(first);
        service.AddAsset(second);

        // Act
        AssetData data = service.GetData();

        // Assert
        Assert.Equal(3, data.NextId);
        Assert.Equal(2, data.Assets.Count);
        Assert.Equal(1, data.Assets[0].Id);
        Assert.Equal(2, data.Assets[1].Id);
    }
    // ===== Test suite 5: LoadData method to unpack AssetData object into a memory =====
    [Fact]
    public void LoadData_TwoAssets_RestoresAssetsAndNextId()
    {
        // Arrange: another service gives the assets ids 1 and 2 -> data like from the file
        var source = new AssetService();
        source.AddAsset(new MobilePhone("Samsung", "Galaxy S26 Plus", new DateTime(2026, 11, 5), 1500m, TurkeyOffice()));
        source.AddAsset(new Computer("HP", "EliteBook 840", new DateTime(2025, 9, 1), 1100m, UsaOffice()));
        AssetData data = source.GetData();   // NextId = 3

        var service = new AssetService();

        // Act
        service.LoadData(data);

        // Assert: both assets are loaded
        List<Asset> assets = service.GetAssetsSortedById();
        Assert.Equal(2, assets.Count);
        Assert.Equal("Galaxy S26 Plus", assets[0].Model);

        // Assert: numbering continues from the file
        var newAsset = new Computer("Dell", "XPS 13", new DateTime(2024, 5, 20), 1200m, SwedenOffice());
        service.AddAsset(newAsset);
        Assert.Equal(3, newAsset.Id);
    }

    [Fact]
    public void LoadData_NextIdTooSmall_NewAssetGetsNextFreeId()
    {
        // Arrange: another service gives the assets ids 1 and 2 -> data like from the file
        var source = new AssetService();
        source.AddAsset(new MobilePhone("Samsung", "Galaxy S26 Plus", new DateTime(2026, 11, 5), 1500m, TurkeyOffice()));
        source.AddAsset(new Computer("HP", "EliteBook 840", new DateTime(2025, 9, 1), 1100m, UsaOffice()));
        AssetData data = source.GetData();   // NextId = 3
        data.NextId = 1; // file was edited by hand

        var service = new AssetService();

        // Act
        service.LoadData(data);

        // Assert: new asset has to get Id 3, not 1
        var newAsset = new Computer("Dell", "XPS 13", new DateTime(2024, 5, 20), 1200m, SwedenOffice());
        service.AddAsset(newAsset);
        Assert.Equal(3, newAsset.Id);
    }

    [Fact]
    public void GetAssetById_EmptyList_ThrowsAssetNotFoundException()
    {
        // Arrange
        var service = new AssetService();

        // Act & Assert
        Assert.Throws<AssetNotFoundException>(() => service.GetAssetById(1));
    }

    [Fact]
    public void GetAssetById_IdDoesNotExist_ThrowsAssetNotFoundException()
    {
        // Arrange
        var service = new AssetService();
        var asset = new Computer("HP", "EliteBook 840", new DateTime(2025, 9, 1), 1100m, SwedenOffice());
        service.AddAsset(asset);

        // Act & Assert
        Assert.Throws<AssetNotFoundException>(() => service.GetAssetById(100));
    }

    [Fact]
    public void GetAssetById_IdExists_ReturnsAssetWithGivenId()
    {
        // Arrange
        var service = new AssetService();
        var first = new Computer("HP", "EliteBook 840", new DateTime(2025, 9, 1), 1100m, SwedenOffice());
        var second = new MobilePhone("Google", "Pixel 11", new DateTime(2026, 7, 11), 950m, TurkeyOffice());
        var third = new MobilePhone("Xiaomi", "15", new DateTime(2025, 9, 1), 620m, UsaOffice());
        service.AddAsset(first);
        service.AddAsset(second);
        service.AddAsset(third);

        // Act 
        Asset found = service.GetAssetById(2);

        // Assert
        Assert.Same(second, found);
    }

    // ===== Test suite 7: Remove asset =====
    [Fact]
    public void RemoveAssetById_IdExists_RemovesOnlyThatAsset()
    {
        // Arrange
        var service = new AssetService();
        var first = new Computer("HP", "EliteBook 840", new DateTime(2025, 9, 1), 1100m, SwedenOffice());
        var second = new MobilePhone("Google", "Pixel 11", new DateTime(2026, 7, 11), 950m, TurkeyOffice());
        var third = new MobilePhone("Xiaomi", "15", new DateTime(2025, 9, 1), 620m, UsaOffice());
        service.AddAsset(first);
        service.AddAsset(second);
        service.AddAsset(third);

        // Act 
        service.RemoveAssetById(2);

        // Assert
        List<Asset> remaining = service.GetAssetsSortedById();
        Assert.Equal(2, remaining.Count);
        Assert.DoesNotContain(second, remaining);
        Assert.Contains(first, remaining);
        Assert.Contains(third, remaining);
    }

    [Fact]
    public void RemoveAssetById_IdDoesNotExist_ThrowsAssetNotFoundException()
    {
        // Arrange
        var service = new AssetService();
        var first = new Computer("HP", "EliteBook 840", new DateTime(2025, 9, 1), 1100m, SwedenOffice());
        var second = new MobilePhone("Google", "Pixel 11", new DateTime(2026, 7, 11), 950m, TurkeyOffice());
        var third = new MobilePhone("Xiaomi", "15", new DateTime(2025, 9, 1), 620m, UsaOffice());
        service.AddAsset(first);
        service.AddAsset(second);
        service.AddAsset(third);

        // Act & Assert
        Assert.Throws<AssetNotFoundException>(() => service.RemoveAssetById(20));
        Assert.Equal(3, service.GetAssetsSortedById().Count);
    }

    [Fact]
    public void AddAsset_AfterRemove_DoesNotReuseRemovedId()
    {
        // Arrange
        var service = new AssetService();
        var first = new Computer("HP", "EliteBook 840", new DateTime(2025, 9, 1), 1100m, SwedenOffice());
        var second = new MobilePhone("Google", "Pixel 11", new DateTime(2026, 7, 11), 950m, TurkeyOffice());
        var third = new MobilePhone("Xiaomi", "15", new DateTime(2025, 9, 1), 620m, UsaOffice());
        service.AddAsset(first);
        service.AddAsset(second);
        service.AddAsset(third);

        // Act 
        service.RemoveAssetById(2);
        var fourth = new MobilePhone("Google", "Pixel 11", new DateTime(2026, 7, 11), 950m, TurkeyOffice());
        service.AddAsset(fourth);

        // Assert
        Assert.Equal(4, fourth.Id);
    }
    // ===== Test suite 8: Search by brand or model =====

    [Fact]
    public void SearchAssets_TextNotFound_ThrowsAssetNotFoundException()
    {
        // Arrange
        var service = new AssetService();
        var first = new Computer("HP", "EliteBook 840", new DateTime(2025, 9, 1), 1100m, SwedenOffice());
        var second = new MobilePhone("Google", "Pixel 11", new DateTime(2026, 7, 11), 950m, TurkeyOffice());
        var third = new MobilePhone("Xiaomi", "15", new DateTime(2025, 9, 1), 620m, UsaOffice());
        service.AddAsset(first);
        service.AddAsset(second);
        service.AddAsset(third);

        // Act & Assert
        Assert.Throws<AssetNotFoundException>(() => service.SearchAssets("Apple"));
    }

    [Fact]
    public void SearchAssets_EmptyList_ThrowsAssetNotFoundException()
    {
        // Arrange
        var service = new AssetService();

        // Act & Assert
        Assert.Throws<AssetNotFoundException>(() => service.SearchAssets("Apple"));
    }

    [Fact]
    public void SearchAssets_SearchByBrand_ReturnsAssetsWithGivenBrand()
    {
        // Arrange
        var service = new AssetService();
        var first = new Computer("HP", "EliteBook 840", new DateTime(2025, 9, 1), 1100m, SwedenOffice());
        var second = new MobilePhone("Google", "Pixel 11", new DateTime(2026, 7, 11), 950m, TurkeyOffice());
        var third = new MobilePhone("Xiaomi", "15", new DateTime(2025, 9, 1), 620m, UsaOffice());
        var fourth = new Computer("HP", "AI 15t-fd200", new DateTime(2026, 4, 19), 1499.99m, SwedenOffice());
        service.AddAsset(first);
        service.AddAsset(second);
        service.AddAsset(third);
        service.AddAsset(fourth);

        // Act
        List<Asset> found = service.SearchAssets("HP");

        // Assert
        Assert.Contains(first, found);
        Assert.Contains(fourth, found);
        Assert.DoesNotContain(second, found);
        Assert.DoesNotContain(third, found);
    }

    [Fact]
    public void SearchAssets_SearchByModel_ReturnsAssetsWithGivenModel()
    {
        // Arrange
        var service = new AssetService();
        var first = new MobilePhone("Google", "Pixel 11", new DateTime(2026, 7, 11), 950m, TurkeyOffice());
        var second = new Computer("HP", "EliteBook 840", new DateTime(2025, 9, 1), 1100m, SwedenOffice());
        var third = new Computer("HP", "AI 15t-fd200", new DateTime(2026, 4, 19), 1499.99m, SwedenOffice());
        var fourth = new MobilePhone("Xiaomi", "15", new DateTime(2025, 9, 1), 620m, UsaOffice());
        service.AddAsset(first);
        service.AddAsset(second);
        service.AddAsset(third);
        service.AddAsset(fourth);

        // Act
        List<Asset> found = service.SearchAssets("15");

        // Assert
        Assert.Contains(third, found);
        Assert.Contains(fourth, found);
        Assert.DoesNotContain(second, found);
        Assert.DoesNotContain(first, found);
    }

    [Theory]
    [InlineData("oo")]
    [InlineData("OO")]
    [InlineData("Oo")]
    public void SearchAssets_TextInBrandOrModel_ReturnsBothMatches(string text)
    {
        // Arrange
        var service = new AssetService();
        var first = new MobilePhone("Google", "Pixel 11", new DateTime(2026, 7, 11), 950m, TurkeyOffice());
        var second = new Computer("HP", "EliteBook 840", new DateTime(2025, 9, 1), 1100m, SwedenOffice());
        var third = new Computer("HP", "AI 15t-fd200", new DateTime(2026, 4, 19), 1499.99m, SwedenOffice());
        var fourth = new MobilePhone("Xiaomi", "15", new DateTime(2025, 9, 1), 620m, UsaOffice());
        service.AddAsset(first);
        service.AddAsset(second);
        service.AddAsset(third);
        service.AddAsset(fourth);

        // Act
        List<Asset> found = service.SearchAssets(text);

        // Assert
        Assert.DoesNotContain(third, found);
        Assert.DoesNotContain(fourth, found);
        Assert.Contains(second, found);
        Assert.Contains(first, found);
    }
}