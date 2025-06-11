

using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.DTOs.Product;

public record ProductToSelectResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid ProductItemId,
    string Color,
    string? ProductImage,
    ProductBodyZone BodyZone,
    string ParentCategory,
    string Gender);