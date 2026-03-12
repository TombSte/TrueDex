using AutoMapper;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using TrueDex.Api.Extensions;
using TrueDex.Api.Models.Responses;
using TrueDex.Api.UseCases.Queries.GetPokemon;

namespace TrueDex.Api.MinimalApi;

public static class RootApi
{
    public static void AddTrueDexMinimalApi(this WebApplication app)
    {
        
        var group = app.MapGroup("pokemon");
        group.MapGet("{name}",
            async ( [FromRoute] string name, IMediator mediator, IMapper mapper ) =>
            {
                var result = await mediator.Send(GetPokemonRequest.Build(name));
                if (result.IsFailed) return result.ToProblemResult();
                var response = mapper.Map<PokemonResponse>(result.Value);
                return Results.Ok(response);
            });
    }
}
