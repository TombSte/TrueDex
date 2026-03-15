using AwesomeAssertions;
using FluentResults;
using NSubstitute;
using TrueDex.Api.Models.Entities;
using TrueDex.Api.Services.Interfaces;
using TrueDex.Api.UseCases.Queries.GetPokemon;

namespace TrueDex.Api.Test.UseCases.Queries.GetPokemon;

public class GetPokemonHandlerTest
{
    private readonly GetPokemonHandler _handler;
    private readonly IPokemonService _service = Substitute.For<IPokemonService>();

    public GetPokemonHandlerTest()
    {
        _handler = new GetPokemonHandler(_service);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedResult_WhenServiceSucceeds()
    {
        const string name = "lugia";
        using var cancellationTokenSource = new CancellationTokenSource();

        _service.GetPokemonAsync(name, cancellationTokenSource.Token)
            .Returns(Result.Ok(new Pokemon
            {
                Name = name,
                Description = "Lugia description",
                IsLegendary = true,
                Habitat = "sea"
            }));

        var result = await _handler.Handle(new GetPokemonRequest(name), cancellationTokenSource.Token);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(name);
        result.Value.Description.Should().Be("Lugia description");
        result.Value.Habitat.Should().Be("sea");
        result.Value.IsLegendary.Should().BeTrue();

        await _service.Received(1).GetPokemonAsync(name, cancellationTokenSource.Token);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenServiceFails()
    {
        const string name = "Luga";
        var error = new Error("Pokemon not found");

        _service.GetPokemonAsync(name, CancellationToken.None)
            .Returns(Result.Fail<Pokemon>(error));

        var result = await _handler.Handle(new GetPokemonRequest(name), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].Message.Should().Be("Pokemon not found");
    }
}
