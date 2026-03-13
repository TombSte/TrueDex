using AutoMapper;
using TrueDex.Api.Models.Responses;
using TrueDex.Api.UseCases.Queries.GetPokemon;
using TrueDex.Api.UseCases.Queries.GetTranslatedPokemon;

namespace TrueDex.Api.Misc.Mapping;

public class ResponseMappingProfile : Profile
{
    public ResponseMappingProfile()
    {
        CreateMap<GetPokemonResult, PokemonResponse>();
        CreateMap<GetTranslatedPokemonResult, PokemonResponse>();
    }
}