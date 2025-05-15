using System.Text.Json;
using FluentValidation;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.Product;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.ProductItems.GetAll;

public class GetAllProductItemToSelectValidator : AbstractValidator<GetAllProductItemsToSelectQuery>
{
    public GetAllProductItemToSelectValidator(IApplicationDbContext applicationDbContext)
    {
        When(x => x.Filters.Count != 0, () =>
        {
            RuleFor(x => x.Filters)
                .Must(x => x.All(f => Enum.IsDefined(f.Key)))
                .WithMessage("Invalid filter type");
        });

        When(x => x.Filters.TryGetValue(ProductFilterType.PageNumber, out _), () =>
        {
            RuleFor(x => x.Filters[ProductFilterType.PageNumber])
                .Must(x => int.TryParse(x.ToString(), out var number) && number > 0)
                .WithMessage("PageNumber must be a positive integer.");
        });

        When(x => x.Filters.TryGetValue(ProductFilterType.PageSize, out _), () =>
        {
            RuleFor(x => x.Filters[ProductFilterType.PageSize])
                .Must(x => int.TryParse(x.ToString(), out var size) && size > 0)
                .WithMessage("PageSize must be a positive integer.");
        });

        RuleFor(x => x.Filters)
            .MustAsync(async (filters, _) =>
            {
                if (!filters.TryGetValue(ProductFilterType.Colours, out var val)) return true;
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

        When(x => x.Filters.TryGetValue(ProductFilterType.Gender, out _), () =>
        {
            RuleFor(x => x.Filters[ProductFilterType.Gender])
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
                if (!filters.TryGetValue(ProductFilterType.Attributes, out var val)) return true;
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
        When(x => x.Filters.TryGetValue(ProductFilterType.ParentCategory, out _), () =>
        {
            RuleFor(x => x.Filters[ProductFilterType.ParentCategory])
                .MustAsync(async (val, _) =>
                {
                    var parentCategoryId = val?.ToString();
                    if (string.IsNullOrWhiteSpace(parentCategoryId) || !Guid.TryParse(parentCategoryId, out var guid))
                        return false;
                    return await applicationDbContext.ProductCategories
                        .AnyAsync(c => c.Id == guid);
                })
                .WithMessage("Specified product category does not exist.");
        });
    }
}