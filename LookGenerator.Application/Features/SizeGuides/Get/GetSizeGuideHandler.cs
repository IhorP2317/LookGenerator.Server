using System.Text.Json;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.Constants;
using LookGenerator.Application.Common.DTOs.SizeGuide;
using LookGenerator.Application.Common.DTOs.SizeOption;
using LookGenerator.Application.Common.Exceptions;
using LookGenerator.Application.Common.Helpers;
using LookGenerator.Application.Common.Mappers;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.SizeGuides.Get;

public class GetSizeGuideHandler(
    IApplicationDbContext applicationDbContext
) : IQueryHandler<GetSizeGuideQuery, SizeGuideTableResponse>
{
public async Task<SizeGuideTableResponse> Handle(GetSizeGuideQuery request, CancellationToken cancellationToken)
{
    var gender = ExtractStringFromJsonElement(request.Filters[SizeGuideFilterType.Gender]).ToUpper();
    var bodyZone = ExtractStringFromJsonElement(request.Filters[SizeGuideFilterType.BodyZone]).ToUpper();

    // find the gender and bodyZone categories...
    var rootCategory = await applicationDbContext.SizeCategories
        .AsNoTracking()
        .FirstAsync(x => x.Name.ToUpper() == gender && x.ParentCategoryId == null, cancellationToken);

    var bodyZoneCategory = await applicationDbContext.SizeCategories
        .AsNoTracking()
        .FirstAsync(x => x.Name.ToUpper() == bodyZone && x.ParentCategoryId == rootCategory.Id, cancellationToken);

    // build descendant map...
    var allCats = await applicationDbContext.SizeCategories
        .AsNoTracking()
        .Include(sc => sc.SizeOptions)
        .Where(x => x.ParentCategoryId != null)
        .ToListAsync(cancellationToken);

    var childrenMap = allCats
        .GroupBy(x => x.ParentCategoryId)
        .ToDictionary(g => g.Key!.Value, g => g.ToList());

    var parameters = GetAllDescendants(bodyZoneCategory.Id)
        .Where(cat => cat.SizeOptions?.Any() == true)
        .ToList();

    // fetch all relevant variations
    var bodyZoneKey = LookGenerationConstants.SizeZonesByCategory[bodyZone];
    var variations = await applicationDbContext.ProductVariations
        .AsNoTracking()
        .Include(pv => pv.ProductItem).ThenInclude(pi => pi.Product)
        .Include(pv => pv.MasterSizeIdentifier)
        .Where(pv =>
            pv.ProductItem.Product.Gender.ToUpper() == gender &&
            pv.ProductItem.Product.BodyZone == bodyZoneKey
        )
        .ToListAsync(cancellationToken);

    // **1.** Collect and sort by the human‐readable Size property
    var sizes = variations
        .Select(v => v.Size)    // your size string, e.g. "S", "M", "L"
        .Where(s => !string.IsNullOrEmpty(s))
        .Distinct()
        .OrderBy(s => s, new ClothingSizeComparer())
        .ToList();

    // fetch your SizeOption ↔ Variation mappings
    var links = await applicationDbContext.SizeOptionMasterIdentifiers
        .AsNoTracking()
        .Include(l => l.SizeOption)
        .Include(l => l.MasterSizeIdentifier)
        .ToListAsync(cancellationToken);

    // **2.** Build a map keyed by variation.Size
    var optionMap = new Dictionary<string, List<SizeOption>>();
    foreach (var v in variations)
    {
        var opts = links
            .Where(l => l.MasterSizeIdentifier.Id == v.MasterSizeIdentifier.Id)
            .Select(l => l.SizeOption)
            .ToList();

        if (!optionMap.ContainsKey(v.Size))
            optionMap[v.Size] = [];

        optionMap[v.Size].AddRange(opts);
    }

  
    var rows = new List<SizeGuideRow>();
    
    foreach (var param in parameters)
    {
        var vals = new List<SizeOptionResponse>();
        foreach (var sz in sizes)
        {
            if (optionMap.TryGetValue(sz, out var optsForSize))
            {
             
                var match = optsForSize.FirstOrDefault(o => (param.SizeOptions ?? []).Any(p => p.Id == o.Id));
                vals.Add(match != null
                    ? match.ToResponse(sz)
                    : new SizeOptionResponse(Guid.Empty, sz, 0, 0));
            }
            else
            {
                vals.Add(new SizeOptionResponse(Guid.Empty, sz, 0, 0));
            }
        }

        rows.Add( new SizeGuideRow(param.Name)
        {
            Values = vals
        });
    }
    if (!LookGenerationConstants.RequiredMeasurementsByCategoryAndGender
            .TryGetValue(gender, out var zoneMap) ||
        !zoneMap.TryGetValue(bodyZone, out var requiredParams))
    {
        return new SizeGuideTableResponse([], []);
    }

    rows = rows
        .Where(r => requiredParams.Contains(r.Parameter, StringComparer.OrdinalIgnoreCase))
        .ToList();

    var validSizes = new HashSet<string>();
    foreach (var val in from row in rows from val in row.Values where val.Id != Guid.Empty select val)
        validSizes.Add(val.Name);

    sizes = sizes.Where(sz => validSizes.Contains(sz)).ToList();

    foreach (var row in rows)
        row.Values = row.Values.Where(v => sizes.Contains(v.Name)).ToList();

    
    foreach(var row in rows) {
        row.Values = sizes
            .Select(sz => row.Values.FirstOrDefault(v => v.Name == sz)
                          ?? new SizeOptionResponse(Guid.Empty, sz, 0, 0))
            .ToList();
    }

    return new SizeGuideTableResponse(sizes, rows);

    List<SizeCategory> GetAllDescendants(Guid id)
    {
        return !childrenMap.TryGetValue(id, out var kids) ? [] : kids.SelectMany(c => new[]{c}.Concat(GetAllDescendants(c.Id))).ToList();
    }
}

private static string ExtractStringFromJsonElement(object value)
{
    if (value is not JsonElement { ValueKind: JsonValueKind.String } elt)
        throw new BadRequestException("Expected a string filter");
    return elt.GetString()!;
}

}


