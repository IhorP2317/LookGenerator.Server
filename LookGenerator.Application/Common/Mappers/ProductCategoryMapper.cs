using LookGenerator.Application.Common.DTOs.ProductCategory;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Mappers;

public static class ProductCategoryMapper
{
    public static ICollection<ProductCategoryResponse>
        ToResponses(this ICollection<ProductCategory> productCategories) =>
        productCategories.Select(ToResponse).ToList();

    public static ProductCategoryResponse ToResponse(this ProductCategory productCategory) =>
        new(productCategory.Id, productCategory.Name);
}