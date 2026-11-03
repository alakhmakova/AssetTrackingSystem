using AssetTrackingSystem.Exceptions;
using AssetTrackingSystem.Models;

namespace AssetTrackingSystem.Tests;
// Test names follow the pattern: MethodName_Scenario_ExpectedResult
//   MethodName     - what is being tested (Constructor)
//   Scenario       - the input or situation (ValidData)
//   ExpectedResult - what should happen (SetsNameAndCurrency)
public class AssetTests
{
    // Helper: valid office so we don't repeat it in every test
    private static Office UsaOffice() => new("USA", Currency.USD);
    private static Office TurkeyOffice() => new("Turkey", Currency.TRY);

    // ===== Test case 1: valid data -> properties are set =====
    [Fact]
    public void Constructor_ValidComputer_SetsAllProperties()
    {
        // Arrange: prepare input data
        var office = UsaOffice();
        var date = new DateTime(2024, 3, 15);

        // Act: do the thing we test
        var computer = new Computer("Apple", "MacBook Pro", date, 1500m, office);

        // Assert: check the result (expected first, actual second)
        Assert.Equal("Apple", computer.Brand);
        Assert.Equal("MacBook Pro", computer.Model);
        Assert.Equal(date, computer.PurchaseDate);
        Assert.Equal(1500m, computer.PriceUsd);
        Assert.Equal(office, computer.Office);
        Assert.Equal("Computer", computer.Type);
    }

    [Fact]
    public void Constructor_ValidMobilePhone_SetsAllProperties()
    {
        // Arrange: prepare input data
        var office = UsaOffice();
        var date = new DateTime(2026, 1, 19);

        // Act: do the thing we test
        var mobilePhone = new MobilePhone("Samsung", "Galaxy S26 Plus", date, 1420m, office);

        // Assert: check the result (expected first, actual second)
        Assert.Equal("Samsung", mobilePhone.Brand);
        Assert.Equal("Galaxy S26 Plus", mobilePhone.Model);
        Assert.Equal(date, mobilePhone.PurchaseDate);
        Assert.Equal(1420m, mobilePhone.PriceUsd);
        Assert.Equal(office, mobilePhone.Office);
        Assert.Equal("MobilePhone", mobilePhone.Type);
    }

    // ===== Test case 2: invalid data -> exception. =====
    // Validation lives in one place (the Asset constructor), so testing one subclass is enough.
    // Test passes only if the code inside () => ... throws ValidationException
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyBrand_ThrowsValidationException(string brand)
    {
        var office = TurkeyOffice();

        Assert.Throws<ValidationException>(() =>
            new Computer(brand, "MacBook Pro", DateTime.Today, 1500m, office));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyModel_ThrowsValidationException(string model)
    {
        var office = TurkeyOffice();

        Assert.Throws<ValidationException>(() =>
            new Computer("Apple", model, DateTime.Today, 1500m, office));
    }

    [Fact]
    public void Constructor_PurchaseDateInTheFuture_ThrowsValidationException()
    {
        var office = TurkeyOffice();

        Assert.Throws<ValidationException>(() =>
    new Computer("Apple", "MacBook Pro", DateTime.Today.AddDays(1), 1500m, office));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1500)]
    [InlineData(1500.786)]
    public void Constructor_InvalidPrice_ThrowsValidationException(double price)
    {
        var office = TurkeyOffice();

        Assert.Throws<ValidationException>(() =>
            new Computer("Apple", "MacBook Pro", DateTime.Today, (decimal)price, office));
    }

    [Fact]
    public void Constructor_OfficeIsNull_ThrowsValidationException()
    {
        Assert.Throws<ValidationException>(() =>
    new Computer("Apple", "MacBook Pro", DateTime.Today, 1500m, null!));
    }

    // ===== Testing validation methods =====
    [Theory]
    [InlineData(10.55, true)]
    [InlineData(0, false)]
    [InlineData(-5, false)]
    [InlineData(10.555, false)]
    public void IsValidPrice_VariousPrices_ReturnsExpected(double price, bool expected)
    {
        // InlineData can't hold decimal, so we pass double and convert
        bool result = Asset.IsValidPrice((decimal)price);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, true)]     // today
    [InlineData(-1, true)]    // yesterday
    [InlineData(1, false)]    // tomorrow
    [InlineData(90, false)]   // in 3 months
    public void IsValidDate_VariousDates_ReturnsExpected(int daysFromToday, bool expected)
    {
        // InlineData can't hold DateTime (not a constant), so we pass days and build the date here
        var date = DateTime.Today.AddDays(daysFromToday);

        bool result = Asset.IsValidDate(date);

        Assert.Equal(expected, result);
    }
}