using FluentValidation;
using LookGenerator.Application.Abstractions;

namespace LookGenerator.Application.Features.Looks.Delete;

public class DeleteLookValidator:AbstractValidator<DeleteLookCommand>
{
    public DeleteLookValidator(IApplicationDbContext applicationDbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required")
            .MustAsync(async (id, _) => (await applicationDbContext.Looks.FindAsync(id)) != null)
            .WithMessage("Look is not exist!");
    }
}
