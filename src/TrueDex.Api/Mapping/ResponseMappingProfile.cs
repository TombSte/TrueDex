using AutoMapper;
using TrueDex.Api.Models.Responses;
using TrueDex.Api.UseCases.Queries.GetPokemon;

namespace TrueDex.Api.Mapping;

public class ResponseMappingProfile : Profile
{
    public ResponseMappingProfile()
    {
        CreateMap<GetPokemonResult, PokemonResponse>();
    }
}