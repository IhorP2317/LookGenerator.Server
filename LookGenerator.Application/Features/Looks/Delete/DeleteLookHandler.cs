using LookGenerator.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Looks.Delete;

public class DeleteLookHandler(IApplicationDbContext applicationDbContext) : ICommandHandler<DeleteLookCommand>
{
    public async Task Handle(DeleteLookCommand request, CancellationToken cancellationToken)
    {
        var existingLook = await applicationDbContext.Looks.FirstAsync(l => l.Id == request.Id, cancellationToken);
        applicationDbContext.Looks.Remove(existingLook);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}