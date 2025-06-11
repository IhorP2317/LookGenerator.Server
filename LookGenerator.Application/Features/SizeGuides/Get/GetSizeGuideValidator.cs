using System.Text.Json;
using FluentValidation;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.SizeGuide;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.SizeGuides.Get;

public class GetSizeGuideValidator : AbstractValidator<GetSizeGuideQuery>
{
    public GetSizeGuideValidator(IApplicationDbContext dbContext)
    {
        RuleFor(x => x.Filters)
            .Must(filters =>
                filters.ContainsKey(SizeGuideFilterType.Gender) &&
                filters[SizeGuideFilterType.Gender] is JsonElement { ValueKind: JsonValueKind.String })
            .WithMessage("Gender filter must be provided and must be a string.");
        RuleFor(x => x.Filters)
            .Must(filters =>
                filters.ContainsKey(SizeGuideFilterType.BodyZone) &&
                filters[SizeGuideFilterType.BodyZone] is JsonElement { ValueKind: JsonValueKind.String })
            .WithMessage("Body zone filter must be provided and must be a string.");

        RuleFor(x => x.Filters[SizeGuideFilterType.Gender])
            .MustAsync(async (value, _) =>
            {
                if( value is not JsonElement { ValueKind: JsonValueKind.String } jsonElement)
                    return false;
                
                var gender =   jsonElement.GetString()?.ToUpperInvariant();
                if (string.IsNullOrWhiteSpace(gender))
                    return false;

                return await dbContext.SizeCategories
                    .AnyAsync(c => c.ParentCategoryId == null && c.Name.ToUpper() == gender);
            })
            .WithMessage("Gender category does not exist.");
        
        RuleFor(x => x.Filters[SizeGuideFilterType.BodyZone])
            .MustAsync(async (value, _) =>
            {
                if( value is not JsonElement { ValueKind: JsonValueKind.String } jsonElement)
                    return false;
                
                var bodyZone =   jsonElement.GetString()?.ToUpperInvariant();
                if (string.IsNullOrWhiteSpace(bodyZone))
                    return false;

                return await dbContext.SizeCategories
                    .AnyAsync(c => c.Name.ToUpper() == bodyZone);
            })
            .WithMessage("Body zone category does not exist.");;
    }
}
