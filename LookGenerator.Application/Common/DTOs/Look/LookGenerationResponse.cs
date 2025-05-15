using System.Text.Json.Serialization;

namespace LookGenerator.Application.Common.DTOs.Look;

public record LookGenerationResponse(
    [property: JsonPropertyName("colorPalette")]
    string ColorPalette,
    [property: JsonPropertyName("lookName")] // UPDATED
    string Name,
    [property: JsonPropertyName("productsItems")] // UPDATED
    ICollection<Guid> ProductItemIds,
    [property: JsonPropertyName("description")]
    string? Description = null


)
{
    [JsonPropertyName("productImages")]
    public Dictionary<Guid, string>? ProductImages { get; set; } = null;
};

public record LooksGenerationResponse(
    [property: JsonPropertyName("looks")] ICollection<LookGenerationResponse> Looks,
    [property: JsonPropertyName("description")]
    string? Description = null
);

