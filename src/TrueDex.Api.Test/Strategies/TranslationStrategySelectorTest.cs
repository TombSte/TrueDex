using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using TrueDex.Api.Strategies;

namespace TrueDex.Api.Test.Strategies;

public class TranslationStrategySelectorTest
{
    private readonly TranslationStrategySelector _selector;

    public TranslationStrategySelectorTest()
    {
        var services = new ServiceCollection();
        services.AddKeyedScoped<ITranslationStrategy, YodaTranslationStrategy>(StrategyKeys.Yoda);
        services.AddKeyedScoped<ITranslationStrategy, ShakespeareTranslationStrategy>(StrategyKeys.Default);

        var serviceProvider = services.BuildServiceProvider();
        _selector = new TranslationStrategySelector(serviceProvider);
    }

    [Fact]
    public void GetTranslationStrategy_ShouldReturnYodaStrategy_WhenHabitatMapsToYoda()
    {
        var strategy = _selector.GetTranslationStrategy("cave");

        strategy.Should().BeOfType<YodaTranslationStrategy>();
    }

    [Fact]
    public void GetTranslationStrategy_ShouldReturnDefaultStrategy_WhenHabitatMapsToDefault()
    {
        var strategy = _selector.GetTranslationStrategy("forest");

        strategy.Should().BeOfType<ShakespeareTranslationStrategy>();
    }

    [Fact]
    public void GetDefaultTranslationStrategy_ShouldReturnDefaultStrategy()
    {
        var strategy = _selector.GetDefaultTranslationStrategy();

        strategy.Should().BeOfType<ShakespeareTranslationStrategy>();
    }
}
