using System.Text.Json;
using FluentValidation;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.ProductCategory;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.ProductCategories.GetAll;

public class GetAllProductCategoriesValidator : AbstractValidator<GetAllProductCategoriesQuery>
{
    public GetAllProductCategoriesValidator(IApplicationDbContext applicationDbContext)
    {
        When(x => x.Filters.Count != 0, () =>
        {
            RuleFor(x => x.Filters)
                .Must(x => x.All(f => Enum.IsDefined(f.Key)))
                .WithMessage("Invalid filter type");
        });

        When(x => x.Filters.TryGetValue(ProductCategoryFilterType.Gender, out _), () =>
        {
            RuleFor(x => x.Filters[ProductCategoryFilterType.Gender])
                .MustAsync(async (val, _) =>
                {
                    var genderName = val?.ToString()?.ToUpperInvariant();
                    return await applicationDbContext.ProductCategories
                        .AnyAsync(c => c.ParentCategoryId == null && c.Name.ToUpper() == genderName);
                })
                .WithMessage("Specified gender category does not exist.");
        });

        When(x => x.Filters.TryGetValue(ProductCategoryFilterType.HasProducts, out _), () =>
        {
            RuleFor(x => x.Filters[ProductCategoryFilterType.HasProducts])
                .Must(val =>
                {
                    if (val is JsonElement jsonElement)
                    {
                        return jsonElement.ValueKind is JsonValueKind.True or JsonValueKind.False;
                    }
                    return false;
                })
                .WithMessage("HasProducts filter must be a boolean.");
        });


    }
}