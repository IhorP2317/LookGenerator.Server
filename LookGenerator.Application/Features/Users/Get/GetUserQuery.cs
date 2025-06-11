using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.User;

namespace LookGenerator.Application.Features.Users.Get;

public record GetUserQuery(Guid Id) : IQuery<UserResponse>;