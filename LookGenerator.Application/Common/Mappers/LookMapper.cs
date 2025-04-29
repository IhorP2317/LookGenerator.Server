using LookGenerator.Application.Common.DTOs;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Mappers;

public static class LookMapper
{
    public static LookResponse ToResponse(this Look look,
        ICollection<ProductVariation> matchingVariations,
        Dictionary<Guid, (Dictionary<string, List<string>> attributes, List<string> categories)>? productDescriptions = null,
        Dictionary<Guid, Dictionary<string, SizeOption>>? topSizeMap = null,
        Dictionary<Guid, Dictionary<string, SizeOption>>? bottomSizeMap = null,
        Dictionary<Guid, Dictionary<string, SizeOption>>? footwearSizeMap = null)
    {
        var products = matchingVariations
            .GroupBy(mv => mv.ProductItemId)
            .Select(g =>
            {
                var productItem = g.First().ProductItem;

                var sizeMap = productItem.Product.BodyZone switch
                {
                    ProductBodyZone.UpperBody => topSizeMap,
                    ProductBodyZone.LowerBody => bottomSizeMap,
                    ProductBodyZone.Feet => footwearSizeMap,
                    _ => null
                };

                return productItem.ToLookProductResponse(
                    productDescriptions?.GetValueOrDefault(productItem.ProductId).categories,
                    productDescriptions?.GetValueOrDefault(productItem.ProductId).attributes,
                    sizeMap
                );
            })
            .ToList();

        return new LookResponse(
            Id: look.Id,
            Name: look.Name,
            Description: look.Description,
            ColorPalette: look.ColorPalette,
            LookStatus: look.LookStatus,
            CreatedAt: look.CreatedAt,
            CreatedBy: look.CreatedBy,
            ModifiedAt: look.ModifiedAt,
            ModifiedBy: look.ModifiedBy,
            Products: products
        );
    }

}
