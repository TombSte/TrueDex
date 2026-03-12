using FluentResults;
using TrueDex.Api.Models.Entities;

namespace TrueDex.Api.Interfaces;

public interface IPokemonService
{
    ValueTask<Result<Pokemon>> GetPokemon(string pokemonName, CancellationToken cancellationToken = default);
}