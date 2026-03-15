using AwesomeAssertions;
using TrueDex.Api.Misc.Mapping;
using TrueDex.Api.UseCases.Queries.GetPokemon;
using TrueDex.Api.UseCases.Queries.GetTranslatedPokemon;

namespace TrueDex.Api.Test.Misc.Mapping;

public class MappingExtensionsTest
{
    [Fact]
    public void ToPokemonResponse_ShouldMapGetPokemonResult()
    {
        var result = new GetPokemonResult
        {
            Name = "mew",
            Description = "desc.",
            Habitat = "rare",
            IsLegendary = true
        };

        var response = result.ToPokemonResponse();

        response.Name.Should().Be("mew");
        response.Description.Should().Be("desc.");
        response.Habitat.Should().Be("rare");
        response.IsLegendary.Should().BeTrue();
    }

    [Fact]
    public void ToPokemonResponse_ShouldMapGetTranslatedPokemonResult()
    {
        var result = new GetTranslatedPokemonResult
        {
            Name = "ditto",
            Description = "desc.",
            Habitat = "urban",
            IsLegendary = false
        };

        var response = result.ToPokemonResponse();

        response.Name.Should().Be("ditto");
        response.Description.Should().Be("desc.");
        response.Habitat.Should().Be("urban");
        response.IsLegendary.Should().BeFalse();
    }
}
