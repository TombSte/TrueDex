using FluentResults;
using PokeApiNet;
using Polly.Registry;
using TrueDex.Api.Extensions;
using TrueDex.Api.Interfaces;
using TrueDex.Api.Misc.Errors;
using Pokemon = TrueDex.Api.Models.Entities.Pokemon;

namespace TrueDex.Api.Implementations;

public class PokemonService(
    ResiliencePipelineProvider<string> pipelineProvider, 
    PokeApiClient client, 
    ILogger<PokemonService> logger) : IPokemonService
{
    public async ValueTask<Result<Pokemon>> GetPokemon(string pokemonName, CancellationToken cancellationToken = default)
    {
        var pipeline = pipelineProvider.GetDefaultRetryPolicy();

        Pokemon response;
        
        try
        {
            response = await pipeline.ExecuteAsync<Pokemon>(async ctx =>
            {
                var species = await client.GetResourceAsync<PokemonSpecies>(pokemonName, ctx);

                return new Pokemon()
                {
                    Name = species.Name,
                    Habitat = species.Habitat.Name,
                    IsLegendary = species.IsLegendary,
                    Description = species.FlavorTextEntries.FirstOrDefault(x => x.Language.Name == "en")?.FlavorText ??
                                  string.Empty
                };
            }, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting pokemon species");
            return ExternalErrors.PokeApiError;
        }
        
        return Result.Ok(response);
        
    }
}