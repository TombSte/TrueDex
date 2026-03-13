using FluentResults;
using Mediator;

namespace TrueDex.Api.UseCases.Queries.GetTranslatedPokemon;

public record GetTranslatedPokemonRequest(string Name) : IRequest<Result<GetTranslatedPokemonResult>>
{
    public static GetTranslatedPokemonRequest Build(string name) => new(name);
}