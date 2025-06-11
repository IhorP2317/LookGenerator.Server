using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.ProductVariation;
using LookGenerator.Application.Common.Helpers;
using LookGenerator.Application.Common.Mappers;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.ProductVariations.GetAll;

public class GetAllProductVariationsHandler(
    IApplicationDbContext applicationDbContext,
    ISizeGuideService sizeGuideService)
    : IQueryHandler<GetAllProductVariationsQuery, ICollection<ProductVariationResponse>>
{
    public async Task<ICollection<ProductVariationResponse>> Handle(GetAllProductVariationsQuery request, CancellationToken cancellationToken)
    {
       
        var productVariationsQuery = applicationDbContext.ProductVariations
            .AsNoTracking()
            .Include(pv => pv.ProductItem)
            .ThenInclude(pi => pi.Product)
            .Include(pv => pv.MasterSizeIdentifier)
            .AsQueryable();

        // 2. Apply filters
        productVariationsQuery = request.Filters.Aggregate(productVariationsQuery,
            (current, filter) => ProductVariationFiltersHelper.GetProductVariationFilter(filter)(current));

        // 3. Execute query
        var productVariations = await productVariationsQuery.ToListAsync(cancellationToken);

        var dimensionMap = new Dictionary<Guid, Dictionary<string, SizeOption>?>();
        foreach (var variation in productVariations)
        {
            var dimensions = await sizeGuideService.GetDimensionsAsync(variation, cancellationToken);
            dimensionMap[variation.Id] = dimensions;
        }
        productVariations = productVariations
            .OrderBy(pv => pv.MasterSizeIdentifier.Identifier ?? pv.Size, new ClothingSizeComparer())
            .ToList();

        // 6. Convert to response models
        return productVariations.ToProductVariationResponses(dimensionMap);
    }

}