using LookGenerator.Application.Common.DTOs.ProductVariation;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Mappers;

public static class ProductVariationMapper
{
    public static ICollection<ProductVariationResponse> ToProductVariationResponses(
        this ICollection<ProductVariation> productVariations,
        Dictionary<Guid, Dictionary<string, SizeOption>?> dimensions)
    {
        return productVariations.Select(x => x.ToProductVariationResponse(dimensions[x.Id])).ToList();
    }

    public static ProductVariationResponse ToProductVariationResponse(this ProductVariation productVariation,
        Dictionary<string, SizeOption>? dimensions = null)
    {
        return new ProductVariationResponse(
            Id: productVariation.Id,
            Size: productVariation.Size,
            Price: productVariation.Price,
            MasterSizeIdentifierId: productVariation.MasterSizeIdentifierId,
            ProductItemId: productVariation.ProductItemId
        )
        {
            ProductDimensions = dimensions?.ToDictionary(
                entry => entry.Key,
                entry => $"{Math.Round(entry.Value.Cm, 1)} cm"
            )
        };
    }
}