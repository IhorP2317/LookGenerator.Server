using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.User;

namespace LookGenerator.Application.Features.Users.GetCurrentUser;

public record GetCurrentUserQuery(string Email):IQuery<UserResponse>;