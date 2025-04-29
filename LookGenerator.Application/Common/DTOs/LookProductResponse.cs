using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.DTOs;

public record LookProductResponse(
    Guid Id,
    string Name,
    string? Description,
    ProductBodyZone BodyZone,
    ICollection<string> Categories,
    Dictionary<string, List<string>> Attributes,
    Guid ProductItemId,
    string Color,
    string? ProductImage,
    string? ProductLink,
    ICollection<ProductVariationResponse> ProductVariations
);