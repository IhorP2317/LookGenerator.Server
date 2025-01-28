using FluentValidation;
using LookGenerator.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Users.Create ;

    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator(IApplicationDbContext context)
        {
            RuleFor(rc => rc.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .MustAsync(
                    async (email, _) =>
                        await context.Users.AnyAsync(u => u.Email == email))
                .WithMessage("The email has already been used for another account.");

            RuleFor(rc => rc.UserName)
                .NotEmpty()
                .MustAsync(async (username, _) => !(await context.Users.AnyAsync(u => u.UserName == username)))
                .WithMessage("The username is already taken.");
            RuleFor(rc => rc.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(16)
                .Matches(@"[A-Z]+")
                .Matches(@"[a-z]+")
                .Matches(@"[0-9]+")
                .Matches(@"[^\w\s_]+|_");
        }
    }