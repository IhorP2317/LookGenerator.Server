using FluentValidation;
using LookGenerator.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Users.Delete ;

    public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserValidator(IApplicationDbContext applicationDbContext)
        {
            RuleFor(command => command.UserId).NotEmpty()
                .MustAsync(async (userId, _) => await applicationDbContext.Users.AnyAsync(u => u.Id == userId))
                .WithMessage(
                    "User does not exist!");
        }
    }