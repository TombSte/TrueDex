using System.Text.Json.Serialization;

namespace TrueDex.Api.Models.Responses;

public class PokemonResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("habitat")]
    public string Habitat { get; set; }
    [JsonPropertyName("isLegendary")]
    public bool IsLegendary { get; set; }
}