using FluentValidation;
using LookGenerator.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Users.ConfirmEmail ;

    public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailValidator(IApplicationDbContext context)
        {
            RuleFor(c => c.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .MustAsync(
                    async (email, _) =>
                        await context.Users.AnyAsync(u => u.Email == email))
                .WithMessage("The email has already been used for another account.");
            RuleFor(c => c.Token).NotEmpty().WithMessage("Confirmation Token must not be empty.");
        }
    }