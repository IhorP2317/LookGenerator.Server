using System.Text.Json;
using System.Text.Json.Serialization;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs;
using LookGenerator.Application.Common.DTOs.Look;
using LookGenerator.Application.Common.Exceptions;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Looks.GenerateForItem;

public class GenerateLooksForItemHandler(
    IApplicationDbContext applicationDbContext,
    ILookGenerationService lookGenerationService,
    ISizeGuideService sizeGuideService) : ICommandHandler<GenerateLooksForItemCommand, ICollection<LookResponse>>
{
    public async Task<ICollection<LookResponse>> Handle(GenerateLooksForItemCommand request,
        CancellationToken cancellationToken)
    {
        var productVariation = await applicationDbContext
            .ProductVariations
            .AsNoTracking()
            .Include(pv => pv.MasterSizeIdentifier)
            .Include(pv => pv.ProductItem)
            .ThenInclude(pi => pi.Colour)
            .Include(pv => pv.ProductItem.Product)
            .ThenInclude(pi => pi.ProductAttributeOptions)
            .ThenInclude(pa => pa.AttributeOption)
            .Include(pv => pv.ProductItem.Product.ProductCategory)
            .FirstAsync(pv => pv.Id == request.ProductVariationId, cancellationToken: cancellationToken);
        var currentCategory = productVariation.ProductItem.Product.ProductCategory;

        while (currentCategory.ParentCategoryId != null)
        {
            currentCategory = await applicationDbContext.ProductCategories.AsNoTracking()
                .FirstAsync(c => c.Id == currentCategory.ParentCategoryId, cancellationToken);
        }

        var gender = currentCategory.Name.ToUpperInvariant();
        if (productVariation.ProductItem.Product.BodyZone != ProductBodyZone.HeadOrExtras &&
            !productVariation.ProductItem.Product.ProductCategory.Name.Equals("Belts",
                StringComparison.OrdinalIgnoreCase))
        {
            var sizeOptions = await applicationDbContext.SizeOptionMasterIdentifiers
                .AsNoTracking()
                .Where(somi => somi.MasterIdentifierId == productVariation.MasterSizeIdentifierId)
                .Include(somi => somi.SizeOption)
                .ThenInclude(so => so.SizeCategory)
                .Select(somi => somi.SizeOption)
                .ToListAsync(cancellationToken);


            var genderCategory = await applicationDbContext.SizeCategories
                .AsNoTracking()
                .Where(sc => sc.Name.ToLower() == gender.ToLower() && sc.ParentCategoryId == null)
                .FirstOrDefaultAsync(cancellationToken);

            if (genderCategory == null)
            {
                throw new NotFoundException($"Gender size category not found for gender: {gender}");
            }

            var genderSizeCategoryIds = await GetDescendantSizeCategoryIdsAsync(genderCategory.Id, cancellationToken);
            genderSizeCategoryIds.Add(genderCategory.Id);
            var validSizeOptions = sizeOptions
                .Where(so => genderSizeCategoryIds.Contains(so.SizeCategoryId))
                .ToList();
            foreach (var sizeOption in validSizeOptions)
            {
                var measurementName = sizeOption.SizeCategory.Name;
                request.Measurements[measurementName] = sizeOption.Cm;
            }
        }


        var (topSizeMap, bottomSizeMap, footwearSizeMap) = await sizeGuideService.GetSizeMapsAsync(
            request.Measurements, gender, cancellationToken);
        if (productVariation.ProductItem.Product.ProductCategory.Name.Equals("Belts",
                StringComparison.OrdinalIgnoreCase) && double.TryParse(productVariation.MasterSizeIdentifier.Identifier,
                out var beltSizeCm))
        {
            bottomSizeMap[productVariation.MasterSizeIdentifierId] = new Dictionary<string, SizeOption>
            {
                { "waistContour", new SizeOption { Cm = beltSizeCm } }
            };
        }
        else
        {
            var waistContour = request.Measurements["waistContour"];
            var beltIdentifiers =
                await sizeGuideService.FindMatchingBeltMasterSizesAsync(waistContour,
                    gender, cancellationToken);
            foreach (var beltIdentifier in beltIdentifiers.Where(beltIdentifier =>
                         beltIdentifier.Value.Count != 0 && !bottomSizeMap.ContainsKey(beltIdentifier.Key)))
            {
                bottomSizeMap.Add(beltIdentifier.Key, beltIdentifier.Value);
            }
        }


        if (topSizeMap.Count == 0 ||
            bottomSizeMap.Count == 0 || footwearSizeMap.Count == 0)
        {
            throw new NotFoundException("No matching products found for the given measurements!");
        }

        var categoryHierarchy =
            await lookGenerationService.GetDenormalizedProductsAsync(gender,
                productVariation.ProductItem.Product.ProductAttributeOptions.Select(pao => pao.AttributeOptionId)
                    .ToList(),
                topSizeMap.Keys.ToList(),
                bottomSizeMap.Keys.ToList(), footwearSizeMap.Keys.ToList(), cancellationToken);
        if (categoryHierarchy.Products.All(p => p.ProductItems.Count == 0))
        {
            throw new NotFoundException("No products found for the given categories!");
        }

        var fixedProduct =
            categoryHierarchy.Products.FirstOrDefault(p => p.Id == productVariation.ProductItem.ProductId);
        if (fixedProduct == null)
        {
            throw new NotFoundException("No selected product found for the given categories!");
        }

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var fixedProductJson = JsonSerializer.Serialize(fixedProduct, jsonOptions);

        var response =
            await lookGenerationService.GenerateLooksAsync(categoryHierarchy, productVariation.ProductItem.Colour.Name,
                fixedProductJson, cancellationToken);
        if (response is { Description: not null })
            throw new BadRequestException(response.Description);

        var looksResponse = await lookGenerationService.MapAndSaveLooksAsync(response, categoryHierarchy, topSizeMap,
            bottomSizeMap, footwearSizeMap, cancellationToken);

        if (looksResponse.Count == 0)
            throw new NotFoundException("No valid looks generated!");

        return looksResponse;
    }

    private async Task<HashSet<Guid>> GetDescendantSizeCategoryIdsAsync(
        Guid parentCategoryId,
        CancellationToken cancellationToken)
    {
        var result = new HashSet<Guid>();

        var childCategoryIds = await applicationDbContext.SizeCategories
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == parentCategoryId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        foreach (var childId in childCategoryIds)
        {
            result.Add(childId);
            var descendants = await GetDescendantSizeCategoryIdsAsync(childId, cancellationToken);
            result.UnionWith(descendants);
        }

        return result;
    }
}