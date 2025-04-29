using System.Text.Json.Serialization;

namespace LookGenerator.Infrastructure.Models.Responses.SizeGuide;

public class SizeBlock
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;
    [JsonPropertyName("hasUnits")]
    public bool HasUnits { get; set; }
    [JsonPropertyName("onlyMultilength")]
    public bool OnlyMultilength { get; set; } = false;
    [JsonPropertyName("fields")]
    public List<SizeField> Fields { get; set; } = default!;
}