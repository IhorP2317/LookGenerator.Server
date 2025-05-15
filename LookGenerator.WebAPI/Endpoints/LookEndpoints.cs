using Carter;
using LookGenerator.Application.Common.DTOs.Look;
using LookGenerator.Application.Common.Helpers;
using LookGenerator.Application.Features.Looks.ChangeStatus;
using LookGenerator.Application.Features.Looks.Create;
using LookGenerator.Application.Features.Looks.Delete;
using LookGenerator.Application.Features.Looks.Generate;
using LookGenerator.Application.Features.Looks.GenerateForItem;
using LookGenerator.Application.Features.Looks.GetOneDetail;
using LookGenerator.Application.Features.Looks.GetPaged;
using LookGenerator.Application.Features.Looks.Update;
using LookGenerator.Domain.Entities;
using LookGenerator.WebAPI.Filters;
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
        group.MapPost(string.Empty, GetLooks)
            .WithName(nameof(GetLooks))
            .Produces<PagedList<FeedLookResponse>>();
        group.MapGet("{id:guid}", GetLook)
            .WithName(nameof(GetLook))
            .Produces<LookResponse>();
        group.MapPost("create", CreateLook)
            .WithName(nameof(CreateLook))
            .Produces<Guid>()
            .RequireAuthorization();
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
            .AddEndpointFilter<EntityOwnershipAuthorizationFilter<Look>>()
            .RequireAuthorization();
        group.MapPatch("{id:guid}", UpdateLookStatus)
            .WithName(nameof(UpdateLookStatus))
            .Produces(StatusCodes.Status204NoContent)
            .AddEndpointFilter<EntityOwnershipAuthorizationFilter<Look>>()
            .RequireAuthorization();
        group.MapDelete("{id:guid}", DeleteLook)
            .WithName(nameof(DeleteLook))
            .Produces(StatusCodes.Status204NoContent)
            .AddEndpointFilter<EntityOwnershipAuthorizationFilter<Look>>()
            .RequireAuthorization();
    }

    private async Task<IResult> GetLooks(
        [FromBody]GetLooksQuery getLooksQuery,
        ISender sender, 
        CancellationToken cancellationToken = default)
    {
        var looks = await sender.Send(getLooksQuery, cancellationToken);

        return Results.Ok(looks);
    }

    private async Task<IResult> GetLook([FromRoute] Guid id, ISender sender,
        CancellationToken cancellationToken = default)
    {
        return Results.Ok(await sender.Send(new GetLookQuery(id), cancellationToken));
    }


    private async Task<IResult> GenerateLooks([FromBody] GenerateLooksCommand generateLooksCommand, ISender sender,
        CancellationToken cancellationToken = default)
    {
        return Results.Ok(await sender.Send(generateLooksCommand, cancellationToken));
    }

    private async Task<IResult> GenerateLooksByItem([FromBody] GenerateLooksForItemCommand generateLooksForItemCommand,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        return Results.Ok(await sender.Send(generateLooksForItemCommand, cancellationToken));
    }

    private async Task<IResult> CreateLook([FromBody] CreateLookCommand createLookCommand, ISender sender,
        CancellationToken cancellationToken = default)
    {
     
        return Results.Ok(   await sender.Send(createLookCommand, cancellationToken));
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

    private async Task<IResult> UpdateLookStatus([FromRoute] Guid id,
        [FromBody] UpdateLookStatusRequest updateLookStatusRequest,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        await sender.Send(new ChangeLookStatusCommand(id, updateLookStatusRequest.Status), cancellationToken);
        return Results.NoContent();
    }

    private async Task<IResult> DeleteLook([FromRoute] Guid id, ISender sender,
        CancellationToken cancellationToken = default)
    {
        await sender.Send(new DeleteLookCommand(id), cancellationToken);
        return Results.NoContent();
    }
}