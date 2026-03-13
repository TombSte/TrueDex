using FluentResults;
using TrueDex.Api.Models.Entities;

namespace TrueDex.Api.Services.Interfaces;

public interface IPokemonService
{
    ValueTask<Result<Pokemon>> GetPokemonAsync(string pokemonName, CancellationToken cancellationToken = default);
}