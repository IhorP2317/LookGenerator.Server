using FluentValidation;
using LookGenerator.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Users.Get;

public class GetUserValidator : AbstractValidator<GetUserQuery>
{
    public GetUserValidator(IApplicationDbContext applicationDbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required")
            .MustAsync(async (id, _) => (await applicationDbContext.Users.AnyAsync(u => u.Id == id)))
            .WithMessage("User not found");
    }
}