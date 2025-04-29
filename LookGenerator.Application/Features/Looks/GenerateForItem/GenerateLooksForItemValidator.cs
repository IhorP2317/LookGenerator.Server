using FluentValidation;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.Constants;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Looks.GenerateForItem;

public class GenerateLooksForItemValidator : AbstractValidator<GenerateLooksForItemCommand>
{
    public GenerateLooksForItemValidator(IApplicationDbContext applicationDbContext)
    {
        RuleFor(gli => gli.ProductVariationId)
            .NotEmpty()
            .WithMessage("Product variation Id is required.")
            .MustAsync(async (id, _) => (await applicationDbContext.ProductVariations.FindAsync(id)) != null)
            .WithMessage("Product variation not found.");
        RuleFor(command => command)
            .NotEmpty()
            .WithMessage("At least one measurement must be specified.")
            .MustAsync(async (command, cancellationToken) =>
            {
                var productVariation = await applicationDbContext.ProductVariations
                    .AsNoTracking()
                    .Include(pv => pv.ProductItem)
                    .ThenInclude(pi => pi.Product)
                    .ThenInclude(p => p.ProductCategory)
                    .FirstAsync(pv => pv.Id == command.ProductVariationId, cancellationToken);

                var currentCategory = productVariation.ProductItem.Product.ProductCategory;

                while (currentCategory.ParentCategoryId != null)
                {
                    currentCategory = await applicationDbContext.ProductCategories
                        .AsNoTracking()
                        .FirstAsync(c => c.Id == currentCategory.ParentCategoryId, cancellationToken);
                }

                var gender = currentCategory.Name.ToUpperInvariant();
                if (!LookGenerationConstants.RequiredMeasurementsByCategoryAndGender.TryGetValue(gender,
                        out var requiredByCategory))
                    return false;

                var bodyZone = productVariation.ProductItem.Product.BodyZone;


                var coveredMeasurements = new HashSet<string>();

                var productCategory = productVariation.ProductItem.Product.ProductCategory;
                var isBelt = productCategory.Name.Equals("Belts", StringComparison.OrdinalIgnoreCase);

                if (isBelt)
                {
                    coveredMeasurements.Add("waistContour");
                }
                else
                {
                    switch (bodyZone)
                    {
                        case ProductBodyZone.UpperBody:
                            if (requiredByCategory.TryGetValue("TOP", out var topMeasurements))
                                coveredMeasurements.UnionWith(topMeasurements);
                            break;
                        case ProductBodyZone.LowerBody:
                            if (requiredByCategory.TryGetValue("BOTTOM", out var bottomMeasurements))
                                coveredMeasurements.UnionWith(bottomMeasurements);
                            break;
                        case ProductBodyZone.Feet:
                            if (requiredByCategory.TryGetValue("SHOES", out var shoesMeasurements))
                                coveredMeasurements.UnionWith(shoesMeasurements);
                            break;
                        case ProductBodyZone.HeadOrExtras:
                           
                            break;
                        default:
                            return false;
                    }
                }
                
                var categoriesToCheck = new List<string> { "TOP", "BOTTOM", "SHOES" };
                
                var requiredMeasurements = new List<string>();

                foreach (var category in categoriesToCheck)
                {
                    if (!requiredByCategory.TryGetValue(category, out var measurements))
                        continue;

                    requiredMeasurements.AddRange(measurements.Where(measurement => !coveredMeasurements.Contains(measurement)));
                }

                requiredMeasurements = requiredMeasurements.Distinct().ToList();
                
                var allProvided = requiredMeasurements
                    .All(measurement => command.Measurements.TryGetValue(measurement, out var value) && value > 0);

                return allProvided;

            })
            .WithMessage("All required measurements must be provided and greater than 0.");
    }
}