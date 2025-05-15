using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;


namespace LookGenerator.Application.Features.Reactions.Create;

public class CreateReactionHandler(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService): ICommandHandler<CreateReactionCommand>
{
    public async Task Handle(CreateReactionCommand request, CancellationToken cancellationToken)
    {
     var currentUserId = Guid.Parse(currentUserService.UserId!);
       var  reaction = new Reaction
        {
            LookId = request.LookId,
            Type = request.ReactionType,
            CreatedBy = currentUserId,
        };
        await applicationDbContext.Reactions.AddAsync(reaction, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}