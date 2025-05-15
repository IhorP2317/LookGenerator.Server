using FluentValidation;
using LookGenerator.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Looks.GetOneDetail;

public class GetLookValidator : AbstractValidator<GetLookQuery>
{
    public GetLookValidator(IApplicationDbContext applicationDbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required")
            .MustAsync(async (id, _) =>
                (await applicationDbContext.Looks.AnyAsync(l => l.Id == id))).WithMessage("Look not found");
    }
}