using System.Text.Json.Serialization;

namespace LookGenerator.Infrastructure.Models.Responses.Looks;

public record PhotoData(
    [property: JsonPropertyName("categories")] List<string> Categories,
    [property: JsonPropertyName("products")] List<PhotoProductData> Products
);