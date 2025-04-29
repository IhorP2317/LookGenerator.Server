using System.Text.Json.Serialization;

namespace LookGenerator.Infrastructure.Models.Responses.SizeGuide;
public class SizeGuide
{
    [JsonPropertyName("TOP")]
    public SizeCategoryData TOP { get; set; } = default!;
    [JsonPropertyName("BOTTOM")]
    public SizeCategoryData BOTTOM { get; set; } = default!;
    [JsonPropertyName("SHOES")]
    public SizeCategoryData SHOES { get; set; } = default!;
}
