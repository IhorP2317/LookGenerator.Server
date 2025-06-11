using LookGenerator.Application.Common.DTOs.Look;
using LookGenerator.Application.Common.DTOs.Product;
using LookGenerator.Application.Common.DTOs.ProductVariation;
using LookGenerator.Application.Common.Helpers;
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
            Gender: productItem.Product.Gender,
            BodyZone: productItem.Product.BodyZone,
            Categories: categories ?? [],
            Attributes: attributes ?? new Dictionary<string, List<string>>(),
            ProductItemId: productItem.Id,
            Color: productItem.Colour.Name,
            ProductImage: productItem.Images.FirstOrDefault()?.ImageUrl,
            ProductLink: productItem.Links.FirstOrDefault()?.Url,
            Description: productItem.Product.Description,
            ProductVariations: variations,
            Price: variations.Count != 0
                ? variations.Average(v => v.Price)
                : 0,
            Sizes: variations.Count != 0 && productItem.Product.BodyZone != ProductBodyZone.HeadOrExtras
                ? string.Join(" /", variations.Select(v => v.Size))
                : null
        );
    }


  

    public static ProductRequest ToRequest(this Product product, ICollection<string> categories,
        ICollection<ProductItemRequest>? productItems = null)
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

    public static PagedList<ProductToSelectResponse> ToPagedSelectResponse(this PagedList<ProductItem> products)
    {
        return new PagedList<ProductToSelectResponse>(
            items: products.Items.Select(p => p.ToSelectResponse()).ToList(),
            page: products.Page,
            pageSize: products.PageSize,
            totalCount: products.TotalCount
        );
    }


    public static ProductToSelectResponse ToSelectResponse(this ProductItem productItem)
    {
        return new ProductToSelectResponse(
            Id: productItem.Product.Id,
            Name: productItem.Product.Name,
            Description: productItem.Product.Description,
            ProductItemId: productItem.Id,
            Color: productItem.Colour.Name,
            ProductImage: productItem.Images.FirstOrDefault()?.ImageUrl,
            BodyZone: productItem.Product.BodyZone,
            ParentCategory: productItem.Product.ProductCategory.Name,
            Gender:productItem.Product.Gender
        );
    }
}