namespace LookGenerator.Application.Common.DTOs;

public record ProductVariationResponse(
    Guid Id,
    Guid ProductItemId,
    decimal Price,
    string? Size,
    Guid MasterSizeIdentifierId)
{
    public Dictionary<string, string>? ProductDimensions { get; set; } = null;
};