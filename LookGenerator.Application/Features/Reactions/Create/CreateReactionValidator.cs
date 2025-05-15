using FluentValidation;
using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Reactions.Create;

public class CreateReactionValidator : AbstractValidator<CreateReactionCommand>
{
    public CreateReactionValidator(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    {
        RuleFor(x => x.LookId)
            .NotEmpty()
            .WithMessage("LookId is required")
            .MustAsync(async (lookId, cancellationToken) =>
                await applicationDbContext.Looks.AnyAsync(l => l.Id == lookId, cancellationToken))
            .WithMessage("Look with this Id does not exist");

        RuleFor(x => x.ReactionType)
            .IsInEnum()
            .WithMessage("ReactionType must be a valid enum value");


            RuleFor(x => x)
                .MustAsync(async (cr, cancellationToken) =>
                {
                    var userId = currentUserService.UserId;
                    if (string.IsNullOrWhiteSpace(userId)) return false;

                    var currentUserId = Guid.Parse(userId);
                    return !await applicationDbContext.Reactions.AnyAsync(r =>
                            r.LookId == cr.LookId &&
                            r.CreatedBy == currentUserId &&
                            r.Type == cr.ReactionType,
                        cancellationToken);
                })
                .WithMessage("You already performed this reaction on this look");
    }
}
