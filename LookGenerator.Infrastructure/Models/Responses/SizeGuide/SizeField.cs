using System.Text.Json.Serialization;

namespace LookGenerator.Infrastructure.Models.Responses.SizeGuide;
public class SizeField
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
    [JsonPropertyName("required")]
    public bool Required { get; set; }
    [JsonPropertyName("sizes")]
    public List<SizeValue> Sizes { get; set; } = default!;
}