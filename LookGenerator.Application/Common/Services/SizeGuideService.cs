using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.Constants;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Common.Services;

public class SizeGuideService(IApplicationDbContext applicationDbContext) : ISizeGuideService
{
    public async Task<(Dictionary<Guid, Dictionary<string, SizeOption>> topSizeMap,
            Dictionary<Guid, Dictionary<string, SizeOption>> bottomSizeMap,
            Dictionary<Guid, Dictionary<string, SizeOption>> footwearSizeMap)>
        GetSizeMapsAsync(Dictionary<string, double> measurements, string gender, CancellationToken cancellationToken)
    {
        var topSizeMap = await FindMatchingMasterSizeIdsFlexibleAsync(
            measurements, LookGenerationConstants.BodyTolerance, gender, "TOP", cancellationToken);

        var bottomSizeMap = await FindMatchingMasterSizeIdsFlexibleAsync(
            measurements, LookGenerationConstants.BodyTolerance, gender, "BOTTOM", cancellationToken);

        var footwearSizeMap = await FindMatchingMasterSizeIdsFlexibleAsync(
            measurements, LookGenerationConstants.FeetTolerance, gender, "SHOES", cancellationToken);


        return (topSizeMap, bottomSizeMap, footwearSizeMap);
    }

    public async Task<Dictionary<Guid, Dictionary<string, SizeOption>>> FindMatchingBeltMasterSizesAsync(
        double waistContour,
        string gender,
        CancellationToken cancellationToken)
    {
        var genderCategory = await applicationDbContext.ProductCategories
            .AsNoTracking()
            .Where(sc => sc.Name.ToLower() == gender.ToLower() && sc.ParentCategoryId == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (genderCategory == null)
            return new Dictionary<Guid, Dictionary<string, SizeOption>>();

        var beltCategoryIds = await FindAllBeltCategoryIdsAsync(genderCategory.Id, cancellationToken);

        if (beltCategoryIds.Count == 0)
            return new Dictionary<Guid, Dictionary<string, SizeOption>>();

        var beltMasterSizes = await applicationDbContext.MasterSizeIdentifiers
            .AsNoTracking()
            .Where(msi => msi.ProductVariations.Any(pv => beltCategoryIds.Contains(pv.ProductItem.Product.CategoryId)))
            .ToListAsync(cancellationToken);


        var result = new Dictionary<Guid, Dictionary<string, SizeOption>>();

        foreach (var masterSize in beltMasterSizes)
        {
            if (double.TryParse(masterSize.Identifier, out var beltSizeCm) &&
                Math.Abs(beltSizeCm - waistContour) <= LookGenerationConstants.BeltBodyTolerance)
            {
                result[masterSize.Id] = new Dictionary<string, SizeOption>
                {
                    { "waistContour", new SizeOption { Cm = beltSizeCm } }
                };
            }
        }


        return result;
    }


    private async Task<Dictionary<Guid, Dictionary<string, SizeOption>>> FindMatchingMasterSizeIdsFlexibleAsync(
        Dictionary<string, double> measurements,
        double tolerance,
        string gender,
        string parentCategory,
        CancellationToken cancellationToken)
    {
        var genderLower = gender.ToLower();
        var parentCategoryLower = parentCategory.ToLower();
        var targetSizeMap = new Dictionary<Guid, Dictionary<string, SizeOption>>();

        var genderCategoryId = await applicationDbContext.SizeCategories
            .AsNoTracking()
            .Where(sc => sc.Name.ToLower() == genderLower && sc.ParentCategoryId == null)
            .Select(sc => sc.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (genderCategoryId == Guid.Empty)
            return [];

        var parentCategoryId = await applicationDbContext.SizeCategories
            .AsNoTracking()
            .Where(sc => sc.Name.ToLower() == parentCategoryLower && sc.ParentCategoryId == genderCategoryId)
            .Select(sc => sc.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var relevantCategoryIds = await GetDescendantCategoryIdsAsync(parentCategoryId, cancellationToken);

        var sizeOptions = await applicationDbContext.SizeOptions
            .AsNoTracking()
            .Include(so => so.SizeCategory)
            .Where(so => relevantCategoryIds.Contains(so.SizeCategoryId))
            .ToListAsync(cancellationToken);

        var matchedSizeOptions = sizeOptions
            .Where(so =>
                measurements.TryGetValue(so.SizeCategory.Name, out var measurementValue) &&
                Math.Abs(so.Cm - measurementValue) <= tolerance)
            .ToList();

        if (matchedSizeOptions.Count == 0)
            return [];

        var masterSizePairs = await applicationDbContext.SizeOptionMasterIdentifiers
            .AsNoTracking()
            .Where(somi => matchedSizeOptions.Select(so => so.Id).Contains(somi.SizeOptionId))
            .ToListAsync(cancellationToken);

        foreach (var somi in masterSizePairs)
        {
            var sizeOption = matchedSizeOptions.FirstOrDefault(so => so.Id == somi.SizeOptionId);
            if (sizeOption == null)
                continue;

            if (!targetSizeMap.TryGetValue(somi.MasterIdentifierId, out var dimensions))
            {
                dimensions = new Dictionary<string, SizeOption>();
                targetSizeMap[somi.MasterIdentifierId] = dimensions;
            }

            dimensions[sizeOption.SizeCategory.Name] = sizeOption;
        }

        if (LookGenerationConstants.RequiredMeasurementsByCategoryAndGender.TryGetValue(gender.ToUpperInvariant(),
                out var categoryMap)
            && categoryMap.TryGetValue(parentCategory.ToUpperInvariant(), out var requiredKeys))
        {
            targetSizeMap = targetSizeMap
                .Where(pair => requiredKeys.All(key => pair.Value.ContainsKey(key)))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }


        return targetSizeMap;
    }


    private async Task<HashSet<Guid>> GetDescendantCategoryIdsAsync(
        Guid parentCategoryId,
        CancellationToken cancellationToken)
    {
        var result = new HashSet<Guid>();
        var children = await applicationDbContext.SizeCategories
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == parentCategoryId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        foreach (var childId in children)
        {
            result.Add(childId);
            var descendants = await GetDescendantCategoryIdsAsync(childId, cancellationToken);
            result.UnionWith(descendants);
        }

        return result;
    }

    private async Task<HashSet<Guid>> FindAllBeltCategoryIdsAsync(Guid genderCategoryId, CancellationToken cancellationToken)
    {
        var allCategories = await applicationDbContext.ProductCategories
            .AsNoTracking()
            .Select(c => new { c.Id, c.Name, c.ParentCategoryId })
            .ToListAsync(cancellationToken);

        var categoryById = allCategories.ToDictionary(c => c.Id);

        var beltCategoryIds = new HashSet<Guid>();

        foreach (var category in allCategories)
        {
            if (!category.Name.Equals("Belts", StringComparison.OrdinalIgnoreCase))
                continue;

            var currentParentId = category.ParentCategoryId;
            while (currentParentId.HasValue)
            {
                if (currentParentId.Value == genderCategoryId)
                {
                    beltCategoryIds.Add(category.Id);
                    break;
                }

                if (categoryById.TryGetValue(currentParentId.Value, out var parentCategory))
                {
                    currentParentId = parentCategory.ParentCategoryId;
                }
                else
                {
                    break;
                }
            }
        }

        return beltCategoryIds;
    }


}