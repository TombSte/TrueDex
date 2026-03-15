using TrueDex.Api.Models.Responses;
using TrueDex.Api.UseCases.Queries.GetPokemon;
using TrueDex.Api.UseCases.Queries.GetTranslatedPokemon;

namespace TrueDex.Api.Misc.Mapping;

public static class MappingExtensions
{
    public static PokemonResponse ToPokemonResponse(this GetPokemonResult result)
        => new()
        {
            Description = result.Description,
            Name = result.Name,
            Habitat = result.Habitat,
            IsLegendary = result.IsLegendary,
        };

    public static PokemonResponse ToPokemonResponse(this GetTranslatedPokemonResult result)
        => new()
        {
            Description = result.Description,
            Name = result.Name,
            Habitat = result.Habitat,
            IsLegendary = result.IsLegendary,
        };
}