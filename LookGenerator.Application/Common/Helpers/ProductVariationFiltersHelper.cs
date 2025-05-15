using LookGenerator.Application.Common.DTOs.ProductVariation;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Helpers;

public static class ProductVariationFiltersHelper
{
    public static Func<IQueryable<ProductVariation>, IQueryable<ProductVariation>> GetProductVariationFilter(
        KeyValuePair<ProductVariationFilterType, object> filter)
    {
        return filter switch
        {
            { Key: ProductVariationFilterType.ProductItem } => query =>
            {
                return !Guid.TryParse(filter.Value.ToString(), out var guid) ? query : query.Where(x => x.ProductItemId == guid);
            },
            _ => query => query

        };
    }
}