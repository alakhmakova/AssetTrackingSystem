using AssetTrackingSystem.Models;

namespace AssetTrackingSystem.Tests;
// Test names follow the pattern: MethodName_Scenario_ExpectedResult
//   MethodName     - what is being tested (Constructor)
//   Scenario       - the input or situation (ValidData)
//   ExpectedResult - what should happen (SetsNameAndCurrency)
public class OfficeTests
{
    [Fact]
    public void Constructor_ValidData_SetsNameAndCurrency()
    {
        // Arrange + Act
        var office = new Office("Sweden", Currency.SEK);

        // Assert
        Assert.Equal("Sweden", office.Name);
        Assert.Equal(Currency.SEK, office.Currency);
    }
}