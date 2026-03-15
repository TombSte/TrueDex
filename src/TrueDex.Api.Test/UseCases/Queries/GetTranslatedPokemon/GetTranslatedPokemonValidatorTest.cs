using AwesomeAssertions;
using TrueDex.Api.UseCases.Queries.GetTranslatedPokemon;

namespace TrueDex.Api.Test.UseCases.Queries.GetTranslatedPokemon;

public class GetTranslatedPokemonValidatorTest
{
    private readonly GetTranslatedPokemonValidator _validator = new GetTranslatedPokemonValidator();

    [Fact]
    public void Validate_Should_Success()
    {
        var validationResult = _validator.Validate(new GetTranslatedPokemonRequest("charizard"));

        validationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_ReturnError_WhenNameEmpty()
    {
        var validationResult = _validator.Validate(new GetTranslatedPokemonRequest(""));

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Count.Should().Be(1);
        validationResult.Errors[0].PropertyName.Should().Be(nameof(GetTranslatedPokemonRequest.Name));
        validationResult.Errors[0].ErrorMessage.Should().Be("Name is required");
    }
}
