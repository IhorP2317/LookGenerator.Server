using FluentValidation;
using LookGenerator.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Looks.Create;

public class CreateLookValidator:AbstractValidator<CreateLookCommand>
{
    public CreateLookValidator(IApplicationDbContext applicationDbContext)
    {
        RuleFor(ul => ul.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(ul => ul.ColorPalette)
            .NotEmpty()
            .WithMessage("Color palette is required.");

        RuleFor(ul => ul.ProductVariationIds)
            .MustAsync(async (ids, _) =>
                await applicationDbContext.ProductVariations.CountAsync(pv => ids.Contains(pv.Id)) == ids.Count)
            .WithMessage("Some product variations not found.");
    }
}