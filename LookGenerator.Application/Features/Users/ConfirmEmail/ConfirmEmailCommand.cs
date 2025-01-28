using LookGenerator.Application.Abstractions;

namespace LookGenerator.Application.Features.Users.ConfirmEmail ;

    public record ConfirmEmailCommand( string Email, string Token):ICommand;