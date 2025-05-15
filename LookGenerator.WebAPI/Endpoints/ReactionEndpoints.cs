using Carter;
using LookGenerator.Application.Features.Reactions.Create;
using LookGenerator.Application.Features.Reactions.Delete;
using LookGenerator.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LookGenerator.WebAPI.Endpoints;

public class ReactionEndpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reactions")
            .WithOpenApi();
        group.MapPost(string.Empty, CreateReaction)
            .WithName(nameof(CreateReaction))
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization();
        group.MapDelete(string.Empty, DeleteReaction)
            .WithName(nameof(DeleteReaction))
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization();
    }

    private async Task<IResult> CreateReaction(
        [FromBody] CreateReactionCommand createReactionCommand,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        await sender.Send(createReactionCommand, cancellationToken);
        return Results.NoContent();
    }
    private async Task<IResult> DeleteReaction(
       [FromQuery] Guid lookId,
        [FromQuery] ReactionType reactionType,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        await sender.Send(new DeleteReactionCommand(lookId, reactionType), cancellationToken);
        return Results.NoContent();
    }
}