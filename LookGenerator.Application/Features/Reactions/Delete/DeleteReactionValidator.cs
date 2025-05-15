using FluentValidation;
using LookGenerator.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Reactions.Delete;

public class DeleteReactionValidator : AbstractValidator<DeleteReactionCommand>
{
    public DeleteReactionValidator(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    {
        RuleFor(x => x.LookId)
            .NotEmpty()
            .WithMessage("LookId is required")
            .MustAsync(async (lookId, _) =>
                (await applicationDbContext.Looks.AnyAsync(l => l.Id == lookId)))
            .WithMessage("Look with this Id does not exist");
        RuleFor(x => x.ReactionType)
            .IsInEnum()
            .WithMessage("ReactionType must be a valid enum value");
        RuleFor(dr => dr)
            .NotEmpty()
            .WithMessage("Look Id and Reaction Type is required")
            .MustAsync(async (dr, _) =>
            {
                var currentUserId = Guid.Parse(currentUserService.UserId!);
                return (await applicationDbContext.Reactions.AnyAsync(r =>
                    r.LookId == dr.LookId && r.CreatedBy == currentUserId && r.Type == dr.ReactionType));
            })
            .WithMessage("Reaction with this LookId and ReactionType does not exist!");
    }
}