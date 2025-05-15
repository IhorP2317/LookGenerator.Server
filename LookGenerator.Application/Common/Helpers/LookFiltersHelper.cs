using System.Linq.Expressions;
using System.Text.Json;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.Look;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Helpers;

public static class LookFiltersHelper
{
    public static Func<IQueryable<Look>, IQueryable<Look>> GetLookFilter(
        KeyValuePair<LookFilterType, object> filter, ICurrentUserService currentUserService)
    {
        var value = filter.Value.ToString()?.ToLower();

        return filter switch
        {
            { Key: LookFilterType.SearchTerm } => q => value == null
                ? q
                : q.Where(l =>
                    l.Name.ToLower().Contains(value) ||
                    (l.Description ?? "").ToLower().Contains(value) ||
                    l.ColorPalette.ToLower().Contains(value) ||
                    l.LookProductVariations.Any(pv =>
                        pv.ProductVariation.ProductItem.Product.Name.ToLower().Contains(value))
                ),
            { Key: LookFilterType.Gender } => q =>
                q.Where(l => l.LookProductVariations.All(pv =>
                    pv.ProductVariation.ProductItem.Product.Gender.ToLower() == filter.Value.ToString()!.ToLower())
                ),
            { Key: LookFilterType.Attributes } => q =>
            {
                if (filter.Value is not JsonElement { ValueKind: JsonValueKind.Array } jsonElement) return q;
                var attrIds = jsonElement.EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.String && Guid.TryParse(e.GetString(), out _))
                    .Select(e => Guid.Parse(e.GetString()!))
                    .ToList();

                if (attrIds.Count == 0) return q;

                return q.Where(l =>
                    attrIds.All(attrId =>
                        l.LookProductVariations.Any(pv =>
                            pv.ProductVariation.ProductItem.Product.ProductAttributeOptions
                                .Any(pao => pao.AttributeOptionId == attrId)
                        )
                    )
                );
            },


            { Key: LookFilterType.Colours } => q =>
            {
                if (filter.Value is not JsonElement { ValueKind: JsonValueKind.Array } jsonElement) return q;
                var colors = jsonElement.EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(e.GetString()))
                    .Select(e => e.GetString()!.ToLower()).Distinct().ToList();

                return q.Where(look =>
                    colors.All(color =>
                        look.LookProductVariations.Any(pv =>
                            pv.ProductVariation.ProductItem.Colour.Name.ToLower().Contains(color)
                        )
                    )
                );
            },


            { Key: LookFilterType.Status } => q =>
            {
                var statusJson = (JsonElement)filter.Value;

                var status = statusJson.ValueKind == JsonValueKind.Number
                    ? (LookStatus)statusJson.GetInt32()
                    : Enum.Parse<LookStatus>(statusJson.GetString()!, ignoreCase: true);
                return q.Where(l => l.LookStatus == status);
            },


            { Key: LookFilterType.OrderByAscending } => q =>
                q.OrderBy(GetSortExpression(value)),

            { Key: LookFilterType.OrderByDescending } => q =>
                q.OrderByDescending(GetSortExpression(value)),
            
            { Key: LookFilterType.CreatedByCurrentUser } => q =>
            {
                if (!string.IsNullOrWhiteSpace(currentUserService.UserId) &&
                    Guid.TryParse(currentUserService.UserId, out var userId))
                {
                    return q.Where(l => l.CreatedBy == userId);
                }

                return q;
            },
            _ => q => q
        };
    }


    private static Expression<Func<Look, object>> GetSortExpression(string? property)
    {
        return property?.ToLowerInvariant() switch
        {
            "name" => l => l.Name,
            _ => l => l.CreatedAt
        };
    }
}