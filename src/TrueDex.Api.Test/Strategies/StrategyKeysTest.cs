using AwesomeAssertions;
using TrueDex.Api.Strategies;

namespace TrueDex.Api.Test.Strategies;

public class StrategyKeysTest
{
    [Fact]
    public void GetStrategyKey_ShouldReturnYoda_WhenHabitatIsCave()
    {
        var result = StrategyKeys.GetStrategyKey("cave");

        result.Should().Be(StrategyKeys.Yoda);
    }

    [Theory]
    [InlineData("forest")]
    [InlineData("sea")]
    [InlineData("rare")]
    [InlineData("")]
    [InlineData(null)]
    public void GetStrategyKey_ShouldReturnDefault_WhenHabitatIsNotCave(string? habitat)
    {
        var result = StrategyKeys.GetStrategyKey(habitat);

        result.Should().Be(StrategyKeys.Default);
    }
}
