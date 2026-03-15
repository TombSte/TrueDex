using AwesomeAssertions;
using FunTranslationClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Polly.Registry;
using TrueDex.Api.Misc.Errors;
using TrueDex.Api.Misc.Extensions;
using TrueDex.Api.Services.Implementations;
using TrueDex.Api.Strategies;

namespace TrueDex.Api.Test.Services;

public class TranslationServiceTest
{
    private readonly ITranslationStrategySelector _strategySelector = Substitute.For<ITranslationStrategySelector>();
    private readonly ITranslationClient _client = Substitute.For<ITranslationClient>();
    private readonly ILogger<TranslationService> _logger = Substitute.For<ILogger<TranslationService>>();

    [Fact]
    public async Task TranslateAsync_ShouldUseDefaultStrategy_WhenNoStrategyIsSet()
    {
        const string input = "A description.";
        const string translatedText = "Translated description.";
        await using var serviceProvider = CreateServiceProvider();
        
        var pipelineProvider = serviceProvider.GetRequiredService<ResiliencePipelineProvider<string>>();
        
        var defaultStrategy = Substitute.For<ITranslationStrategy>();
        _strategySelector.GetDefaultTranslationStrategy().Returns(defaultStrategy);
        defaultStrategy.ExecuteAsync(_client, input).Returns(CreateTranslationResponse(translatedText));
        var service = new TranslationService(_strategySelector, pipelineProvider, _client, _logger);

        var result = await service.TranslateAsync(input);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(translatedText);
        _strategySelector.Received(1).GetDefaultTranslationStrategy();
        await defaultStrategy.Received(1).ExecuteAsync(_client, input);
    }

    [Fact]
    public async Task TranslateAsync_ShouldUseSelectedStrategy_WhenSetTranslationStrategyIsCalled()
    {
        const string habitat = "cave";
        const string input = "A description.";
        const string translatedText = "Speak like Yoda, this does.";
        
        await using var serviceProvider = CreateServiceProvider();
        
        var pipelineProvider = serviceProvider.GetRequiredService<ResiliencePipelineProvider<string>>();
        var defaultStrategy = Substitute.For<ITranslationStrategy>();
        var selectedStrategy = Substitute.For<ITranslationStrategy>();
        _strategySelector.GetDefaultTranslationStrategy().Returns(defaultStrategy);
        _strategySelector.GetTranslationStrategy(habitat).Returns(selectedStrategy);
        selectedStrategy.ExecuteAsync(_client, input).Returns(CreateTranslationResponse(translatedText));
        var service = new TranslationService(_strategySelector, pipelineProvider, _client, _logger);

        service.SetTranslationStrategy(habitat);
        var result = await service.TranslateAsync(input);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(translatedText);
        _strategySelector.Received(1).GetTranslationStrategy(habitat);
        await selectedStrategy.Received(1).ExecuteAsync(_client, input);
        await defaultStrategy.DidNotReceive().ExecuteAsync(_client, input);
    }

    [Fact]
    public async Task TranslateAsync_ShouldReturnFailure_WhenStrategyThrowsException()
    {
        const string input = "A description.";
        
        await using var serviceProvider = CreateServiceProvider();
        var pipelineProvider = serviceProvider.GetRequiredService<ResiliencePipelineProvider<string>>();
        var defaultStrategy = Substitute.For<ITranslationStrategy>();
        _strategySelector.GetDefaultTranslationStrategy().Returns(defaultStrategy);
        
        defaultStrategy.ExecuteAsync(_client, input)
            .Returns(_ => Task.FromException<TranslationResponse>(new Exception("exception")));
        
        var service = new TranslationService(_strategySelector, pipelineProvider, _client, _logger);

        var result = await service.TranslateAsync(input);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].Message.Should().Be(ExternalErrors.TranslationErrorMessage);
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddRetryPolicies();
        return services.BuildServiceProvider();
    }

    private static TranslationResponse CreateTranslationResponse(string translatedText)
        => new()
        {
            Contents = new Contents
            {
                Translated = translatedText
            }
        };
}
