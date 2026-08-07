using KonturTest.Helpers;

namespace KonturTest.Tests;

public class AmountParserTests
{
    [Theory]
    [InlineData("1000", 1000)]
    [InlineData("3001,10", 3001.10)]
    [InlineData("3001.10", 3001.10)]
    [InlineData(" 42 ", 42)]
    public void TryParse_parses_valid_amounts(string input, decimal expected)
    {
        var success = AmountParser.TryParse(input, out var result);

        Assert.True(success);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("12,34,56")]
    public void TryParse_returns_false_for_invalid_input(string? input)
    {
        var success = AmountParser.TryParse(input, out var result);

        Assert.False(success);
        Assert.Equal(0, result);
    }

    [Fact]
    public void Parse_throws_for_invalid_input()
    {
        var exception = Assert.Throws<FormatException>(() => AmountParser.Parse("not-a-number"));

        Assert.Contains("not-a-number", exception.Message);
    }

    [Fact]
    public void Format_uses_invariant_format()
    {
        Assert.Equal("3001.10", AmountParser.Format(3001.10m));
    }
}
