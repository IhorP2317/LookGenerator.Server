using LookGenerator.Application.Abstractions;

namespace LookGenerator.Application.Features.Users.Delete ;

    public record DeleteUserCommand(Guid UserId):ICommand;