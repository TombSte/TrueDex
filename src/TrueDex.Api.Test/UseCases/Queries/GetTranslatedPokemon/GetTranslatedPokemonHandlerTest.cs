using AwesomeAssertions;
using FluentResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TrueDex.Api.Models.Entities;
using TrueDex.Api.Services.Interfaces;
using TrueDex.Api.UseCases.Queries.GetTranslatedPokemon;

namespace TrueDex.Api.Test.UseCases.Queries.GetTranslatedPokemon;

public class GetTranslatedPokemonHandlerTest
{
    private readonly IPokemonService _pokemonService = Substitute.For<IPokemonService>();
    private readonly ITranslationService _translationService = Substitute.For<ITranslationService>();
    private readonly ILogger<GetTranslatedPokemonHandler> _logger = Substitute.For<ILogger<GetTranslatedPokemonHandler>>();
    private readonly GetTranslatedPokemonHandler _handler;

    public GetTranslatedPokemonHandlerTest()
    {
        _handler = new GetTranslatedPokemonHandler(_pokemonService, _translationService, _logger);
    }

    [Fact]
    public async Task Handle_ShouldReturnTranslatedResult_WhenTranslationSucceeds()
    {
        const string name = "mewtwo";
        const string description = "original description.";
        const string translatedDescription = "translated description.";
        using var cancellationTokenSource = new CancellationTokenSource();

        _pokemonService.GetPokemonAsync(name, cancellationTokenSource.Token)
            .Returns(Result.Ok(new Pokemon
            {
                Name = name,
                Description = description,
                Habitat = "habitat",
                IsLegendary = true,
            }));
        _translationService.TranslateAsync(description)
            .Returns(Result.Ok(translatedDescription));

        var result = await _handler.Handle(new GetTranslatedPokemonRequest(name), cancellationTokenSource.Token);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(name);
        result.Value.Description.Should().Be(translatedDescription);
        result.Value.Habitat.Should().Be("habitat");
        result.Value.IsLegendary.Should().BeTrue();

        await _pokemonService.Received(1).GetPokemonAsync(name, cancellationTokenSource.Token);
        _translationService.Received(1).SetTranslationStrategy("habitat");
        await _translationService.Received(1).TranslateAsync(description);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPokemonLookupFails()
    {
        const string name = "charizard_1";
        var error = new Error("Pokemon not found");

        _pokemonService.GetPokemonAsync(name, CancellationToken.None)
            .Returns(Result.Fail<Pokemon>(error));

        var result = await _handler.Handle(new GetTranslatedPokemonRequest(name), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].Message.Should().Be("Pokemon not found");

        _translationService.DidNotReceive().SetTranslationStrategy(Arg.Any<string>());
        await _translationService.DidNotReceive().TranslateAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_ShouldKeepOriginalDescription_WhenTranslationFails()
    {
        const string name = "ditto";
        const string description = "original description.";

        _pokemonService.GetPokemonAsync(name, CancellationToken.None)
            .Returns(Result.Ok(new Pokemon
            {
                Name = name,
                Description = description,
                Habitat = "urban",
                IsLegendary = false,
            }));
        _translationService.TranslateAsync(description)
            .Returns(Result.Fail<string>("Translation failed"));

        var result = await _handler.Handle(new GetTranslatedPokemonRequest(name), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(name);
        result.Value.Description.Should().Be(description);
        result.Value.Habitat.Should().Be("urban");
        result.Value.IsLegendary.Should().BeFalse();

        _translationService.Received(1).SetTranslationStrategy("urban");
        await _translationService.Received(1).TranslateAsync(description);
    }
}
