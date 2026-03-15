namespace TrueDex.Api.UseCases.Queries.GetPokemon;

public record GetPokemonResult
{
    public required string Name { get; set; }
    public string Description { get; set; }
    public string Habitat { get; set; }
    public bool IsLegendary { get; set; } 
}