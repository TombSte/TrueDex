namespace TrueDex.Api.UseCases.Queries.GetTranslatedPokemon;

public class GetTranslatedPokemonResult
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Habitat { get; set; }
    public bool IsLegendary { get; set; } 
}