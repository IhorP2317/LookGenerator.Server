using FluentValidation;
using LookGenerator.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Users.GetCurrentUser;

public class GetCurrentUserValidator : AbstractValidator<GetCurrentUserQuery>
{
    public GetCurrentUserValidator(IApplicationDbContext applicationDbContext)
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address")
            .MustAsync(async (email, _) => await applicationDbContext.Users.AnyAsync(u => u.Email == email))
            .WithMessage("User is not founded");
    }
}