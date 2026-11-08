using AssetTrackingSystem.UI;

namespace AssetTrackingSystem.Tests;

// Only the parsing is tested here; reading from the console is tested by hand
public class InputHelperTests
{
    [Theory]
    [InlineData("12.50")]  // dot
    [InlineData("12,50")]  // comma
    public void TryParsePrice_DotOrComma_ReturnsSamePrice(string input)
    {
        // Act
        bool ok = InputHelper.TryParsePrice(input, out decimal price);

        // Assert
        Assert.True(ok);
        Assert.Equal(12.50m, price);
    }

    [Fact]
    public void TryParsePrice_WholeNumber_ReturnsPrice()
    {
        bool ok = InputHelper.TryParsePrice("1500", out decimal price);

        Assert.True(ok);
        Assert.Equal(1500m, price);
    }

    [Theory]
    [InlineData("abc")]       // not a number
    [InlineData("1,234.50")]  // thousands separator is not allowed
    [InlineData("12.5.0")]    // two separators
    [InlineData("-5")]        // minus sign is not allowed (price must be > 0 anyway)
    public void TryParsePrice_InvalidText_ReturnsFalse(string input)
    {
        bool ok = InputHelper.TryParsePrice(input, out _);

        Assert.False(ok);
    }
}
