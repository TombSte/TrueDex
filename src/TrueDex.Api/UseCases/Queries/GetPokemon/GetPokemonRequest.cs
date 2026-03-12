using FluentResults;
using Mediator;

namespace TrueDex.Api.UseCases.Queries.GetPokemon;

public record GetPokemonRequest(string Name) : IRequest<Result<GetPokemonResult>>
{
    public static GetPokemonRequest Build(string name) => new(name);
}