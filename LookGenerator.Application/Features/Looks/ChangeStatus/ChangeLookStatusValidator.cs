using FluentValidation;
using LookGenerator.Application.Abstractions;


namespace LookGenerator.Application.Features.Looks.ChangeStatus;

public class ChangeLookStatusValidator:AbstractValidator<ChangeLookStatusCommand>
{
    public ChangeLookStatusValidator(IApplicationDbContext applicationDbContext)
    {
        RuleFor(cls => cls.Id)
            .NotEmpty()
            .WithMessage("Id is required")
            .MustAsync(async (id, _) => (await applicationDbContext.Looks.FindAsync(id)) != null)
            .WithMessage("Look is not exist!");
        
        RuleFor(cls => cls.Status)
            .Must(Enum.IsDefined)
            .WithMessage("Invalid look status.");
    }
    
}