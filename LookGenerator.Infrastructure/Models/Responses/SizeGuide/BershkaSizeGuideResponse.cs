using System.Text.Json.Serialization;

namespace LookGenerator.Infrastructure.Models.Responses.SizeGuide;

public class BershkaSizeGuideResponse
{
    [JsonPropertyName("dimensionsLocale")]
    public Dictionary<string, Dictionary<string, string>> DimensionsLocale { get; set; } = default!;
    [JsonPropertyName("sizeGuide")]
    public SizeGuide SizeGuide { get; set; } = default!;
}