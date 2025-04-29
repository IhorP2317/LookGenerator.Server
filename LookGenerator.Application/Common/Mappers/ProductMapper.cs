using LookGenerator.Application.Common.DTOs;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Mappers;

public static class ProductMapper
{
    public static LookProductResponse ToLookProductResponse(
        this ProductItem productItem,
        ICollection<string>? categories = null,
        Dictionary<string, List<string>>? attributes = null,
        Dictionary<Guid, Dictionary<string, SizeOption>>? sizeMap = null)
    {
        var variations = productItem.Variations.Where(pv =>
            sizeMap?.GetValueOrDefault(pv.MasterSizeIdentifierId) != null ||
            productItem.Product.BodyZone == ProductBodyZone.HeadOrExtras).Select(pv =>
            pv.ToProductVariationResponse(sizeMap?.GetValueOrDefault(pv.MasterSizeIdentifierId))
        ).ToList();

        return new LookProductResponse(
            Id: productItem.Product.Id,
            Name: productItem.Product.Name,
            BodyZone: productItem.Product.BodyZone,
            Categories: categories ?? [],
            Attributes: attributes ?? new Dictionary<string, List<string>>(),
            ProductItemId: productItem.Id,
            Color: productItem.Colour.Name,
            ProductImage: productItem.Images.FirstOrDefault()?.ImageUrl,
            ProductLink: productItem.Links.FirstOrDefault()?.Url,
            Description: productItem.Product.Description,
            ProductVariations: variations
        );
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
    /// <summary>
    /// Maps a Product entity to a ProductRequest with only the required data
    /// </summary>
    public static ProductRequest ToRequest(this Product product, ICollection<string>categories, ICollection<ProductItemRequest>? productItems = null)
    {
        return new ProductRequest(
            Id: product.Id,
            Name: product.Name,
            Description: product.Description,
            BodyZone: product.BodyZone.ToString(),
            ProductItems: productItems ?? product.Items.ToRequests(),
            Categories: categories
        );
    }

}