using System.Text.Json.Serialization;

namespace LookGenerator.Infrastructure.Models.Responses.Looks;

public record PhotoProductData(
    [property: JsonPropertyName("link")] string Link,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("sku")] string Sku,
    [property: JsonPropertyName("stock")] int Stock,
    [property: JsonPropertyName("product_region_links")] List<ProductRegionLink> ProductRegionLinks
);
