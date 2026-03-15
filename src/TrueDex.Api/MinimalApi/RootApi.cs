
using Mediator;
using Microsoft.AspNetCore.Mvc;
using TrueDex.Api.Misc.Extensions;
using TrueDex.Api.Misc.Mapping;
using TrueDex.Api.Models.Responses;
using TrueDex.Api.UseCases.Queries.GetPokemon;
using TrueDex.Api.UseCases.Queries.GetTranslatedPokemon;

namespace TrueDex.Api.MinimalApi;

public static class RootApi
{
    public static void AddTrueDexMinimalApi(this WebApplication app)
    {
        var group = app.MapGroup("pokemon");
        
        group.MapGet("{name}",
            async ( [FromRoute] string name, IMediator mediator) =>
            {
                var result = await mediator.Send(GetPokemonRequest.Build(name));
                if (result.IsFailed) return result.ToProblemResult();
                var response = result.Value.ToPokemonResponse();
                return Results.Ok(response);
            }).Produces<PokemonResponse>(StatusCodes.Status200OK);
        
        group.MapGet("translated/{name}",
            async ( [FromRoute] string name, IMediator mediator ) =>
            {
                var result = await mediator.Send(GetTranslatedPokemonRequest.Build(name));
                if (result.IsFailed) return result.ToProblemResult();
                var response = result.Value.ToPokemonResponse();
                return Results.Ok(response);
            }).Produces<PokemonResponse>(StatusCodes.Status200OK);
    }
}
