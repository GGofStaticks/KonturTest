using KonturTest.Helpers;

namespace KonturTest.Tests;

public class MonthNamesTests
{
    [Fact]
    public void All_contains_twelve_lowercase_english_months()
    {
        Assert.Equal(12, MonthNames.All.Count);
        Assert.Equal("january", MonthNames.All[0]);
        Assert.Equal("december", MonthNames.All[11]);
        Assert.All(MonthNames.All, month => Assert.Equal(month, month.ToLowerInvariant()));
    }

    [Theory]
    [InlineData("january", 0)]
    [InlineData("FEBRUARY", 1)]
    [InlineData("December", 11)]
    public void GetOrderIndex_returns_expected_index(string month, int expectedIndex)
    {
        Assert.Equal(expectedIndex, MonthNames.GetOrderIndex(month));
    }

    [Fact]
    public void GetOrderIndex_puts_unknown_months_last()
    {
        Assert.Equal(MonthNames.All.Count, MonthNames.GetOrderIndex("not-a-month"));
    }
}
