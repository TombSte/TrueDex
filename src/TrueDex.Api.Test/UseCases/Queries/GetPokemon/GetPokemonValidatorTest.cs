using AwesomeAssertions;
using TrueDex.Api.UseCases.Queries.GetPokemon;

namespace TrueDex.Api.Test.UseCases.Queries.GetPokemon;

public class GetPokemonValidatorTest
{
    private readonly GetPokemonValidator _validator = new GetPokemonValidator();
    
    
    [Fact]
    public void Validate_Should_Success()
    {

        var validationResult = _validator.Validate(new GetPokemonRequest("charizard"));

        validationResult.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Validate_Should_ReturnError_WhenNameEmpty()
    {
        var validationResult = _validator.Validate(new GetPokemonRequest(""));

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Count.Should().Be(1);
        validationResult.Errors[0].PropertyName.Should().Be(nameof(GetPokemonRequest.Name));
        validationResult.Errors[0].ErrorMessage.Should().Be("Name is required");
    }
}