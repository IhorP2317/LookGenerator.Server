using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.User;
using LookGenerator.Application.Common.Mappers;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Users.GetCurrentUser;

public class GetCurrentUserHandler(IApplicationDbContext dbContext):IQueryHandler<GetCurrentUserQuery,UserResponse>
{
    public async Task<UserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        return  (await dbContext.Users.FirstAsync(u => u.Email == request.Email, cancellationToken)).ToResponse();
    }
}