using LookGenerator.Application.Abstractions;

namespace LookGenerator.Application.Features.Users.Create ;

    public record CreateUserCommand(string UserName, string Email, string Password ):ICommand;