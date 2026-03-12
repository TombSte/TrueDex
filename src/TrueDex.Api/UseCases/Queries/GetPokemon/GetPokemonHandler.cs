using FluentResults;
using Mediator;
using TrueDex.Api.Interfaces;

namespace TrueDex.Api.UseCases.Queries.GetPokemon;

public class GetPokemonHandler(IPokemonService service) : IRequestHandler<GetPokemonRequest, Result<GetPokemonResult>>
{
    public async ValueTask<Result<GetPokemonResult>> Handle(GetPokemonRequest request, CancellationToken cancellationToken)
    {
        var pokemonResult = await service.GetPokemon(request.Name, cancellationToken);
        
        if(pokemonResult.IsFailed) return Result.Fail(pokemonResult.Errors);

        return new GetPokemonResult()
        {
            Name = pokemonResult.Value.Name,
            Description = pokemonResult.Value.Description,
            Habitat = pokemonResult.Value.Habitat,
            IsLegendary = pokemonResult.Value.IsLegendary,
        };
    }
}