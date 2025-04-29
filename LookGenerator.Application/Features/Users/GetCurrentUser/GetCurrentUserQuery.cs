using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Features.Users.GetCurrentUser;

public record GetCurrentUserQuery(string Email):IQuery<User>;