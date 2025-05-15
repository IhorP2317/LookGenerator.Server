using LookGenerator.Application.Common.DTOs.ProductVariation;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.DTOs.Product;

public record LookProductResponse(
    Guid Id,
    string Name,
    string Gender,
    string? Description,
    ProductBodyZone BodyZone,
    ICollection<string> Categories,
    Dictionary<string, List<string>> Attributes,
    Guid ProductItemId,
    string Color,
    string? ProductImage,
    string? ProductLink,
    ICollection<ProductVariationResponse> ProductVariations,
    decimal Price,
    string? Sizes
);