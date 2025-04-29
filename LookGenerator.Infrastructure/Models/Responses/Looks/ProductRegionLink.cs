using System.Text.Json.Serialization;

namespace LookGenerator.Infrastructure.Models.Responses.Looks;

public record ProductRegionLink(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("region_name")] string RegionName,
    [property: JsonPropertyName("region_stock")] int RegionStock
);