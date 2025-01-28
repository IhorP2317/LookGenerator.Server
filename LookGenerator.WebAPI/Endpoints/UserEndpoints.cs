using Carter;
using LookGenerator.Application.Features.Users.ConfirmEmail;
using LookGenerator.Application.Features.Users.Create;
using LookGenerator.Application.Features.Users.Delete;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LookGenerator.WebAPI.Endpoints ;

    public class UserEndpoints : CarterModule
    {
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/user")
                .WithOpenApi();
            group.MapPost("register", RegisterUser)
                .WithName(nameof(RegisterUser))
                .Produces(StatusCodes.Status200OK)
                .AllowAnonymous();
            group.MapDelete("/{userId}", DeleteUser)
                .WithName(nameof(DeleteUser))
                .Produces(StatusCodes.Status204NoContent)
                .RequireAuthorization();
            group.MapPost("email/confirm", ConfirmEmail)
                .WithName(nameof(ConfirmEmail))
                .Produces(StatusCodes.Status204NoContent);
        }

        private async Task<IResult> RegisterUser([FromBody] CreateUserCommand createUserCommand, ISender sender,
            CancellationToken cancellationToken = default)
        {
            await sender.Send(createUserCommand, cancellationToken);
            return Results.Ok();
        }

        private async Task<IResult> DeleteUser([FromRoute] Guid userId, ISender sender,
            CancellationToken cancellationToken = default)
        {
            await sender.Send(new DeleteUserCommand(userId), cancellationToken);
            return Results.NoContent();
        }
        private async Task<IResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token, ISender sender,
            CancellationToken cancellationToken = default)
        {
            await sender.Send(new ConfirmEmailCommand(email, token), cancellationToken);
            return Results.NoContent();
        }
    }