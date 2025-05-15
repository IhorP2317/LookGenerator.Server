using System.Text.Json;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.ProductCategory;
using LookGenerator.Application.Common.Helpers;
using LookGenerator.Application.Common.Mappers;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.ProductCategories.GetAll;

public class GetAllProductCategoriesHandler(IApplicationDbContext applicationDbContext)
    : IQueryHandler<GetAllProductCategoriesQuery, ICollection<ProductCategoryResponse>>
{
    public async Task<ICollection<ProductCategoryResponse>> Handle(GetAllProductCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var rawCategories = await applicationDbContext.ProductCategories
            .AsNoTracking()
            .Include(c => c.Products)
            .ToListAsync(cancellationToken);

        if (request.Filters.TryGetValue(ProductCategoryFilterType.Gender, out var value))
        {
            var paths = CategoryHelper.BuildCategoryPaths(
                rawCategories.Select(c => (c.Id, c.Name, c.ParentCategoryId))
            );

            var genderCategoryIds = paths
                .Where(kvp => kvp.Value.Any(name => name.Equals(value.ToString(), StringComparison.OrdinalIgnoreCase)))
                .Select(kvp => kvp.Key)
                .ToHashSet();
            rawCategories = rawCategories.Where(c => genderCategoryIds.Contains(c.Id)).ToList();
        }

        if (!request.Filters.TryGetValue(ProductCategoryFilterType.HasProducts, out var hasProducts)
            || hasProducts is not JsonElement { ValueKind: JsonValueKind.True or JsonValueKind.False } jsonElement)
            return rawCategories.ToResponses();
        {
            var hasProductsBool = jsonElement.GetBoolean();

            rawCategories = rawCategories
                .Where(c => c.Products?.Count > 0 == hasProductsBool)
                .ToList();
        }


        return rawCategories.ToResponses();
    }
}