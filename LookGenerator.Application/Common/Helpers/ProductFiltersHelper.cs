using System.Linq.Expressions;
using System.Text.Json;
using LookGenerator.Application.Common.DTOs.Product;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Helpers;

public static class ProductFiltersHelper
{
    public static Func<IQueryable<ProductItem>, IQueryable<ProductItem>> GetProductFilter(
        KeyValuePair<ProductFilterType, object> filter,
        Dictionary<Guid, List<string>>? categoryPaths = null)
    {
        var value = filter.Value.ToString()?.ToLower();

        return filter switch
        {
            { Key: ProductFilterType.SearchTerm } => q => value == null
                ? q
                : q.Where(productItem =>
                    productItem.Product.Name.ToLower().Contains(value) ||
                    (productItem.Product.Description ?? "").ToLower().Contains(value)
                ),

            { Key: ProductFilterType.Gender } => q =>
                q.Where(productItem => productItem.Product.Gender.ToLower() == value),

            { Key: ProductFilterType.ParentCategory } => q =>
            {
                return !Guid.TryParse(value, out var guid) ? q : q.Where(p => p.Product.CategoryId == guid);
            },

            { Key: ProductFilterType.Attributes } => q =>
            {
                if (filter.Value is not JsonElement { ValueKind: JsonValueKind.Array } jsonElement) return q;
                var attrIds = jsonElement.EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.String && Guid.TryParse(e.GetString(), out _))
                    .Select(e => Guid.Parse(e.GetString()!))
                    .ToList();

                if (attrIds.Count == 0) return q;

                return q.Where(productItem =>
                    attrIds.All(attrId =>
                        productItem.Product.ProductAttributeOptions.Any(pao => pao.AttributeOptionId == attrId)
                    )
                );
            },

            { Key: ProductFilterType.Colours } => q =>
            {
                if (filter.Value is not JsonElement { ValueKind: JsonValueKind.Array } jsonElement) return q;
                var colors = jsonElement.EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(e.GetString()))
                    .Select(e => e.GetString()!.ToLower()).Distinct().ToList();

                return q.Where(p =>
                    colors.All(color =>
                        p.Colour.Name.ToLower().Contains(color)
                    )
                );
            },

            { Key: ProductFilterType.OrderByAscending } => q =>
                q.OrderBy(GetSortExpression(value)),

            { Key: ProductFilterType.OrderByDescending } => q =>
                q.OrderByDescending(GetSortExpression(value)),

            _ => q => q
        };
    }

    private static Expression<Func<ProductItem, object>> GetSortExpression(string? property)
    {
        return property?.ToLowerInvariant() switch
        {
            "name" => p => p.Product.Name,
            _ => p => p.CreatedAt
        };
    }
}
