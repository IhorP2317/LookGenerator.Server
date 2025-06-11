using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.User;
using LookGenerator.Application.Common.Mappers;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Users.Get;

public class GetUserHandler(IApplicationDbContext applicationDbContext):IQueryHandler<GetUserQuery,UserResponse>
{
    public async Task<UserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return  (await applicationDbContext.Users.FirstAsync(u => u.Id == request.Id, cancellationToken)).ToResponse();
    }
}