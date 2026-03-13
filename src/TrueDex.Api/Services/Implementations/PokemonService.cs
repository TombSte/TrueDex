using FluentResults;
using PokeApiNet;
using Polly.Registry;
using TrueDex.Api.Extensions;
using TrueDex.Api.Misc.Errors;
using TrueDex.Api.Services.Interfaces;
using Pokemon = TrueDex.Api.Models.Entities.Pokemon;

namespace TrueDex.Api.Services.Implementations;

public class PokemonService(
    ResiliencePipelineProvider<string> pipelineProvider, 
    PokeApiClient client, 
    ILogger<PokemonService> logger) : IPokemonService
{
    public async ValueTask<Result<Pokemon>> GetPokemonAsync(string pokemonName, CancellationToken cancellationToken = default)
    {
        Pokemon response;
        
        try
        {
            response = await pipelineProvider
                .GetDefaultRetryPolicy()
                .ExecuteAsync<Pokemon>(async ctx =>
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