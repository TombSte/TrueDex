using FluentResults;
using Mediator;
using TrueDex.Api.Services.Interfaces;

namespace TrueDex.Api.UseCases.Queries.GetTranslatedPokemon;

public class GetTranslatedPokemonHandler(
    IPokemonService pokemonService, 
    ITranslationService translationService,
    ILogger<GetTranslatedPokemonHandler> logger) : IRequestHandler<GetTranslatedPokemonRequest, Result<GetTranslatedPokemonResult>>
{
    public async ValueTask<Result<GetTranslatedPokemonResult>> Handle(GetTranslatedPokemonRequest request, CancellationToken cancellationToken)
    {
        var pokemonResult = await pokemonService.GetPokemonAsync(request.Name, cancellationToken);
        
        if(pokemonResult.IsFailed) return Result.Fail(pokemonResult.Errors);

        var result = new GetTranslatedPokemonResult()
        {
            Name = pokemonResult.Value.Name,
            Description = pokemonResult.Value.Description,
            Habitat = pokemonResult.Value.Habitat,
            IsLegendary = pokemonResult.Value.IsLegendary,
        };
        
        translationService.SetTranslationStrategy(pokemonResult.Value.Habitat);

        var translationResult = await translationService.TranslateAsync(pokemonResult.Value.Description);

        if (translationResult.IsFailed)
        {
            logger.LogWarning("Translation failed. Keep using original description..");
            return result;
        }
        
        result.Description = translationResult.Value;

        return result;
    }
}