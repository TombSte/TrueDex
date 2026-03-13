using FluentValidation;

namespace TrueDex.Api.UseCases.Queries.GetPokemon;

public class GetPokemonValidator : AbstractValidator<GetPokemonRequest>
{
    public GetPokemonValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name is required");
    }
}