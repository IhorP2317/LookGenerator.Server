using FluentValidation;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.Constants;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Looks.Generate;

public class GenerateLooksValidator : AbstractValidator<GenerateLooksCommand>
{
    public GenerateLooksValidator(IApplicationDbContext context)
    {
        RuleFor(c => c.Gender)
            .NotEmpty()
            .WithMessage("At least one category must be specified.")
            .MustAsync(async (gender, _) =>
            {
                var lowerGender = gender.ToLower();
                var existing = await context.ProductCategories
                    .AsNoTracking()
                   .FirstOrDefaultAsync(c => c.Name.ToLower() == lowerGender);
                return existing != null;
            }).WithMessage("One or more categories do not exist.");

        RuleFor(command => command.AttributeOptionIds)
            .NotEmpty().WithMessage("At least one attribute option must be specified.")
            .MustAsync(async (attributeOptionIds, _) =>
            {
                var existing = await context.AttributeOptions
                    .AsNoTracking()
                    .Where(a => attributeOptionIds.Contains(a.Id))
                    .CountAsync();
                return existing == attributeOptionIds.Count;
            }).WithMessage("One or more attribute options do not exist.");
        RuleFor(command => command.Colours)
            .NotEmpty().WithMessage("At least one colour must be specified.")
            .MustAsync(async (colours, _) =>
            {
                var lowerColours = colours.Select(c => c.ToLowerInvariant()).Distinct().ToList();

                var existingColourNames = await context.Colours
                    .AsNoTracking()
                    .Where(c => lowerColours.Contains(c.Name.ToLower()))
                    .Select(c => c.Name.ToLower())
                    .Distinct()
                    .ToListAsync();

                return existingColourNames.Count == lowerColours.Count;
            }).WithMessage("One or more colours do not exist.");
        RuleFor(command => command.Measurements).NotEmpty().WithMessage("At least one measurement must be specified.")
            .Must(measurements =>
            {
               
                return LookGenerationConstants.RequiredMeasurementsByCategoryAndGender.Values
                    .SelectMany(genderDict => genderDict.Values)  
                    .SelectMany(measurementArray => measurementArray)  
                    .Distinct() 
                    .All(key => measurements.TryGetValue(key, out var value) && value > 0);
            })
            .WithMessage("All required measurements (chestContour, hipContour, waistContour, footMeasure) must be provided and greater than 0.");
    }
}