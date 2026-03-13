using FluentValidation;

namespace TrueDex.Api.UseCases.Queries.GetTranslatedPokemon;

public class GetTranslatedPokemonValidator : AbstractValidator<GetTranslatedPokemonRequest>
{
    public GetTranslatedPokemonValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name is required");
    }
}