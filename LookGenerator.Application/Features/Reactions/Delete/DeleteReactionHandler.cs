using LookGenerator.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Reactions.Delete;

public class DeleteReactionHandler(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    : ICommandHandler<DeleteReactionCommand>
{
    public async Task Handle(DeleteReactionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(currentUserService.UserId!);
        var reaction = await applicationDbContext.Reactions.FirstAsync(
            r => r.LookId == request.LookId && r.CreatedBy == currentUserId && r.Type == request.ReactionType,
            cancellationToken: cancellationToken);
        applicationDbContext.Reactions.Remove(reaction);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}