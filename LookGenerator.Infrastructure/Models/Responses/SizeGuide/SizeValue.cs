using System.Text.Json.Serialization;

namespace LookGenerator.Infrastructure.Models.Responses.SizeGuide;

public class SizeValue
{
    [JsonPropertyName("sizes")]
    public List<string> Sizes { get; set; } = default!;
    [JsonPropertyName("cm")]
    public double Cm { get; set; }
    [JsonPropertyName("inch")]
    public double Inch { get; set; }
}