using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PokeApiNet;
using Polly.Registry;
using TrueDex.Api.Misc.Extensions;
using TrueDex.Api.Services.Implementations;

namespace TrueDex.Api.Test.Services;

public class PokemonServiceTest
{
    private readonly ILogger<PokemonService> _logger = Substitute.For<ILogger<PokemonService>>();
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [Fact]
    public async Task GetPokemonAsync_ShouldReturnMappedPokemon_WhenClientReturnsSpecies()
    {
        const string pokemonName = "geodude";
        using var cancellationTokenSource = new CancellationTokenSource();
        using var serviceProvider = CreateServiceProvider();
        var pipelineProvider = serviceProvider.GetRequiredService<ResiliencePipelineProvider<string>>();
        using var client = new PokeApiClient(new StubHttpMessageHandler(_ => CreatePokemonSpeciesResponse(CreatePokemonSpecies(
            name: "geodude",
            habitat: "mountain",
            isLegendary: false,
            flavorTexts:
            [
                CreateFlavorText("it", "red", "Italian description"),
                CreateFlavorText("en", "blue", "English description")
            ]))));
        var service = new PokemonService(pipelineProvider, client, _logger);

        var result = await service.GetPokemonAsync(pokemonName, cancellationTokenSource.Token);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("geodude");
        result.Value.Habitat.Should().Be("mountain");
        result.Value.IsLegendary.Should().BeFalse();
        result.Value.Description.Should().Be("English description");
    }

    [Fact]
    public async Task GetPokemonAsync_ShouldReturnEmptyDescription_WhenEnglishFlavorTextIsMissing()
    {
        using var serviceProvider = CreateServiceProvider();
        var pipelineProvider = serviceProvider.GetRequiredService<ResiliencePipelineProvider<string>>();
        using var client = new PokeApiClient(new StubHttpMessageHandler(_ => CreatePokemonSpeciesResponse(CreatePokemonSpecies(
            name: "mew",
            habitat: "rare",
            isLegendary: true,
            flavorTexts:
            [
                CreateFlavorText("it", "red", "Descrizione italiana")
            ]))));
        var service = new PokemonService(pipelineProvider, client, _logger);

        var result = await service.GetPokemonAsync("mew");

        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().BeEmpty();
        result.Value.Name.Should().Be("mew");
        result.Value.Habitat.Should().Be("rare");
        result.Value.IsLegendary.Should().BeTrue();
    }

    [Fact]
    public async Task GetPokemonAsync_ShouldReturnFailure_WhenClientThrowsException()
    {
        using var serviceProvider = CreateServiceProvider();
        var pipelineProvider = serviceProvider.GetRequiredService<ResiliencePipelineProvider<string>>();
        using var client = new PokeApiClient(new StubHttpMessageHandler(_ => throw new HttpRequestException("boom")));
        var service = new PokemonService(pipelineProvider, client, _logger);

        var result = await service.GetPokemonAsync("geodude");

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].Message.Should().Be("POKE_API_ERROR");
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddRetryPolicies();
        return services.BuildServiceProvider();
    }

    private static HttpResponseMessage CreateJsonResponse(string content)
        => new(HttpStatusCode.OK)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };

    private static HttpResponseMessage CreatePokemonSpeciesResponse(PokemonSpecies species)
        => CreateJsonResponse(JsonSerializer.Serialize(species, SerializerOptions));

    private static PokemonSpecies CreatePokemonSpecies(
        string name,
        string habitat,
        bool isLegendary,
        List<PokemonSpeciesFlavorTexts> flavorTexts)
        => new()
        {
            Name = name,
            IsLegendary = isLegendary,
            Habitat = new NamedApiResource<PokemonHabitat>
            {
                Name = habitat,
                Url = $"https://pokeapi.co/api/v2/pokemon-habitat/{habitat}/"
            },
            FlavorTextEntries = flavorTexts
        };

    private static PokemonSpeciesFlavorTexts CreateFlavorText(
        string language,
        string version,
        string flavorText)
        => new()
        {
            FlavorText = flavorText,
            Language = new NamedApiResource<Language>
            {
                Name = language,
                Url = $"https://pokeapi.co/api/v2/language/{language}/"
            },
            Version = new NamedApiResource<PokeApiNet.Version>
            {
                Name = version,
                Url = $"https://pokeapi.co/api/v2/version/{version}/"
            }
        };

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => Task.FromResult(responder(request));
    }
}
