using Carter;
using LookGenerator.Application.Common.DTOs;
using LookGenerator.Application.Features.Looks.Generate;
using LookGenerator.Application.Features.Looks.GenerateForItem;
using LookGenerator.Application.Features.Looks.Update;
using LookGenerator.WebAPI.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LookGenerator.WebAPI.Endpoints;

public class LookEndpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/looks")
            .WithOpenApi();
        group.MapPost("generate", GenerateLooks)
            .WithName(nameof(GenerateLooks))
            .Produces<ICollection<LookResponse>>()
            .RequireAuthorization();
        group.MapPost("generateByItem", GenerateLooksByItem)
            .WithName(nameof(GenerateLooksByItem))
            .Produces<ICollection<LookResponse>>()
            .RequireAuthorization();
        group.MapPut("{id:guid}", UpdateLook)
            .WithName(nameof(UpdateLook))
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization();
    }

    private async Task<IResult> GenerateLooks([FromBody] GenerateLooksCommand generateLooksCommand, ISender sender,
        CancellationToken cancellationToken = default)
    {
        return Results.Ok(await sender.Send(generateLooksCommand, cancellationToken));
    }
    private async Task<IResult> GenerateLooksByItem([FromBody] GenerateLooksForItemCommand generateLooksForItemCommand, ISender sender,
        CancellationToken cancellationToken = default)
    {
        return Results.Ok(await sender.Send(generateLooksForItemCommand, cancellationToken));
    }

    private async Task<IResult> UpdateLook(
        [FromRoute] Guid id,
        [FromBody] UpdateLookRequest updateLookRequest,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var updateLookCommand = new UpdateLookCommand(
            id,
            updateLookRequest.Name,
            updateLookRequest.Description,
            updateLookRequest.ColorPalette,
            updateLookRequest.Status,
            updateLookRequest.ProductVariationIds
        );

        await sender.Send(updateLookCommand, cancellationToken);

        return Results.NoContent();
    }

}