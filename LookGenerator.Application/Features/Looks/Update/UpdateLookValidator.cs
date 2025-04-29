using FluentValidation;
using LookGenerator.Application.Abstractions;


namespace LookGenerator.Application.Features.Looks.Update;

public class UpdateLookValidator : AbstractValidator<UpdateLookCommand>
{
    public UpdateLookValidator(IApplicationDbContext applicationDbContext)
    {
        RuleFor(ul => ul.Id)
            .NotEmpty()
            .WithMessage("Look Id is required.")
            .MustAsync(async (id, _) => (await applicationDbContext.Looks.FindAsync(id)) != null)
            .WithMessage("Look not found.");

        RuleFor(ul => ul.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(ul => ul.ColorPalette)
            .NotEmpty()
            .WithMessage("Color palette is required.");

        RuleFor(ul => ul.Status)
            .Must(Enum.IsDefined)
            .WithMessage("Invalid look status.");
    }
}