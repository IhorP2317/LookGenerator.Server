using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Users.GetCurrentUser;

public class GetCurrentUserHandler(IApplicationDbContext dbContext):IQueryHandler<GetCurrentUserQuery,User>
{
    public async Task<User> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        return  await dbContext.Users.FirstAsync(u => u.Email == request.Email, cancellationToken);
    }
}