using LookGenerator.Application.Common.DTOs.User;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Mappers;

public static class UserMapper
{
    public static LookUserResponse? ToLookResponse(this User? user)
    {
        return user != null
            ? new LookUserResponse(
                Id: user.Id,
                UserName: user.UserName,
                Email: user.Email,
                Role: user.Role
            )
            : null;
    }

    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse(
            Id: user.Id,
            CreatedAt: user.CreatedAt,
            CreatedBy: user.CreatedBy,
            ModifiedBy: user.ModifiedBy,
            ModifiedAt: user.ModifiedAt,
            UserName: user.UserName,
            Email: user.Email,
            Role: user.Role,
            EmailConfirmed: user.EmailConfirmed
        );
    }
}