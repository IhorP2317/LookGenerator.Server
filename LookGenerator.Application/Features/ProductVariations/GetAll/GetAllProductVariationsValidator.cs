using FluentValidation;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.ProductVariation;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.ProductVariations.GetAll;

public class GetAllProductVariationsValidator : AbstractValidator<GetAllProductVariationsQuery>
{
    public GetAllProductVariationsValidator(IApplicationDbContext applicationDbContext)
    {
        When(x => x.Filters.Count > 0, () => 
        {
            RuleFor(x => x.Filters)
                .Must(x => x.All(f => Enum.IsDefined(f.Key)))
                .WithMessage("Invalid filter type");
        });
        When(x => x.Filters.TryGetValue(ProductVariationFilterType.ProductItem, out var value), () =>
        {
            RuleFor(x => x.Filters[ProductVariationFilterType.ProductItem])
                .Must(v => Guid.TryParse(v?.ToString(), out _))
                .WithMessage("ProductItem filter must be a valid GUID.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Filters[ProductVariationFilterType.ProductItem])
                        .MustAsync(async (v, ct) =>
                        {
                            var parsed = Guid.TryParse(v?.ToString(), out var productItemId);
                            if (!parsed) return false;

                            return await applicationDbContext.ProductItems
                                .AsNoTracking()
                                .AnyAsync(p => p.Id == productItemId, ct);
                        })
                        .WithMessage("ProductItem does not exist.");
                });
        });
    }
}