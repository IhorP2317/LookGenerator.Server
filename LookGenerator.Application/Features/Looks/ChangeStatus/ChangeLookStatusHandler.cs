using LookGenerator.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Looks.ChangeStatus;

public class ChangeLookStatusHandler(IApplicationDbContext applicationDbContext)
    : ICommandHandler<ChangeLookStatusCommand>
{
    public async Task Handle(ChangeLookStatusCommand request, CancellationToken cancellationToken)
    {
        var look = await applicationDbContext.Looks.FirstAsync(l => l.Id == request.Id, cancellationToken);
        look.LookStatus = request.Status;
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}