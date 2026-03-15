namespace TrueDex.Api.Models.Entities;

public class Pokemon
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Habitat { get; set; }
    public bool IsLegendary { get; set; }
}