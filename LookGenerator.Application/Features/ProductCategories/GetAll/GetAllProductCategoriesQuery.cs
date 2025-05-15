using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.ProductCategory;

namespace LookGenerator.Application.Features.ProductCategories.GetAll;

public record GetAllProductCategoriesQuery : IQuery<ICollection<ProductCategoryResponse>>
{
    public Dictionary<ProductCategoryFilterType, object> Filters { get; set; } = new();
}