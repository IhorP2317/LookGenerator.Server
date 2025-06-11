using System.Linq.Expressions;
using System.Text.Json;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.Look;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Common.Helpers;

public static class LookFiltersHelper
{
    public static Func<IQueryable<Look>, IQueryable<Look>> GetLookFilter(
        KeyValuePair<LookFilterType, object> filter)
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
                var elem = (JsonElement)filter.Value;
                var wanted = new HashSet<LookStatus>();

                if (elem.ValueKind == JsonValueKind.Array)
                {
                    foreach (var child in elem.EnumerateArray())
                        if (TryParseLookStatus(child, out var st))
                            wanted.Add(st);
                }
                else
                {
                    if (TryParseLookStatus(elem, out var st))
                        wanted.Add(st);
                }

                return wanted.Count == 0
                    ? q
                    : q.Where(l => wanted.Contains(l.LookStatus));
            },

            
            { Key: LookFilterType.OrderByAscending } => q =>
                q.OrderBy(GetSortExpression(value)),

            { Key: LookFilterType.OrderByDescending } => q =>
                q.OrderByDescending(GetSortExpression(value)),

            {
                Key: LookFilterType.CreatedBy
            } => q =>
            {
                Guid? createdBy = filter.Value switch
                {
                    Guid g => g,

                    string s when Guid.TryParse(s, out var g)
                        => g,

                    JsonElement { ValueKind: JsonValueKind.String } je
                        when Guid.TryParse(je.GetString(), out var g)
                        => g,

                    _ => null
                };


                return createdBy is null ? q : q.Where(l => l.CreatedBy == createdBy.Value);
            },
            { Key: LookFilterType.LikedBy } => q =>
            {
              
                Guid? likedBy = filter.Value switch
                {
                    Guid g                                         => g,
                    string s when Guid.TryParse(s, out var g)      => g,
                    JsonElement { ValueKind: JsonValueKind.String }
                        je when Guid.TryParse(je.GetString(), out var g) => g,
                    _                                              => null
                };

          
                if (likedBy is null)
                    return q;

              
                return q.Where(l =>
                    l.Reactions.Any(r =>
                        r.Type      == ReactionType.Like &&
                        r.CreatedBy == likedBy));
            },
            { Key: LookFilterType.PinnedBy} => q =>
            {
              
                Guid? likedBy = filter.Value switch
                {
                    Guid g                                         => g,
                    string s when Guid.TryParse(s, out var g)      => g,
                    JsonElement { ValueKind: JsonValueKind.String }
                        je when Guid.TryParse(je.GetString(), out var g) => g,
                    _                                              => null
                };

          
                if (likedBy is null)
                    return q;

              
                return q.Where(l =>
                    l.Reactions.Any(r =>
                        r.Type      == ReactionType.Pin &&
                        r.CreatedBy == likedBy));
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
    // Somewhere inside your query-helper class
    private static bool TryParseLookStatus(JsonElement e, out LookStatus status)
    {
        switch (e.ValueKind)
        {
            case JsonValueKind.Number:
                status = (LookStatus)e.GetInt32();
                return true;

            case JsonValueKind.String:
                return Enum.TryParse(e.GetString(), ignoreCase: true, out status);

            case JsonValueKind.Undefined:
            case JsonValueKind.Object:
            case JsonValueKind.Array:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
            default:
                status = default;
                return false;
        }
    }

}