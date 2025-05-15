using System.Text.Json;
using FluentValidation;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.Look;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Looks.GetPaged;

public class GetLooksValidator : AbstractValidator<GetLooksQuery>
{
    public GetLooksValidator(IApplicationDbContext applicationDbContext)
    {
        When(x => x.Filters.Count != 0, () =>
        {
            RuleFor(x => x.Filters)
                .Must(x => x.All(f => Enum.IsDefined(f.Key)))
                .WithMessage("Invalid filter type");
        });
        When(x => x.Filters.TryGetValue(LookFilterType.PageNumber, out _), () =>
        {
            RuleFor(x => x.Filters[LookFilterType.PageNumber])
                .Must(x => int.TryParse(x.ToString(), out var number) && number > 0)
                .WithMessage("PageNumber must be a positive integer.");
        });

        When(x => x.Filters.TryGetValue(LookFilterType.PageSize, out _), () =>
        {
            RuleFor(x => x.Filters[LookFilterType.PageSize])
                .Must(x => int.TryParse(x.ToString(), out var size) && size > 0)
                .WithMessage("PageSize must be a positive integer.");
        });

        RuleFor(x => x.Filters)
            .MustAsync(async (filters, _) =>
            {
                if (!filters.TryGetValue(LookFilterType.Colours, out var val)) return true;
                if (val is not JsonElement { ValueKind: JsonValueKind.Array } jsonElement) return false;

                var colorNames = jsonElement
                    .EnumerateArray() 
                    .Where(e => e.ValueKind == JsonValueKind.String) 
                    .Select(e => e.GetString()) 
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Select(c => c!.ToLower())
                    .ToList(); 

                if (colorNames.Count == 0) return false;

                var dbColors = await applicationDbContext.Colours
                    .Where(c => colorNames.Contains(c.Name.ToLower()))
                    .Select(c => c.Name.ToLower())
                    .Distinct()
                    .ToListAsync();

                return dbColors.Count == colorNames.Count;
            })
            .WithMessage("One or more colour values are not valid.");


        When(x => x.Filters.TryGetValue(LookFilterType.Gender, out _), () =>
        {
            RuleFor(x => x.Filters[LookFilterType.Gender])
                .MustAsync(async (val, _) =>
                {
                    var genderName = val?.ToString()?.ToUpperInvariant();
                    return await applicationDbContext.ProductCategories
                        .AnyAsync(c => c.ParentCategoryId == null && c.Name.ToUpper() == genderName);
                })
                .WithMessage("Specified gender category does not exist.");
        });

        RuleFor(x => x.Filters)
            .MustAsync(async (filters, _) =>
            {
                if (!filters.TryGetValue(LookFilterType.Attributes, out var val)) return true;
                if (val is not JsonElement { ValueKind: JsonValueKind.Array } jsonElement) return false;
                var attrIds = jsonElement.EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.String)
                    .Select(e => Guid.TryParse(e.GetString(), out var id) ? id : Guid.Empty)
                    .Where(guid => guid != Guid.Empty)
                    .ToList();

                if (attrIds.Count == 0) return false;

                var count = await applicationDbContext.AttributeOptions
                    .CountAsync(a => attrIds.Contains(a.Id));

                return count == attrIds.Count;
            })
            .WithMessage("One or more attribute options are invalid.");


        // 🏷 Status
        When(x => x.Filters.TryGetValue(LookFilterType.Status, out _), () =>
        {
            RuleFor(x => x.Filters[LookFilterType.Status])
                .Must(val =>
                {
                    if (val is JsonElement json)
                    {
                        if (json.ValueKind == JsonValueKind.Number && json.TryGetInt32(out var intVal))
                            return Enum.IsDefined(typeof(LookStatus), intVal);
                        if (json.ValueKind == JsonValueKind.String &&
                            Enum.TryParse<LookStatus>(json.GetString(), true, out _))
                            return true;
                    }

                    return false;
                })
                .WithMessage("Invalid LookStatus value.");
        });
    }
}