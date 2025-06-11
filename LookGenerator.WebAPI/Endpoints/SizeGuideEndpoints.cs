using Carter;
using LookGenerator.Application.Common.DTOs.SizeGuide;
using LookGenerator.Application.Features.SizeGuides.Get;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LookGenerator.WebAPI.Endpoints;

public class SizeGuideEndpoints:CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/size-guide").WithTags("Size Guide").WithOpenApi();
        group.MapPost(string.Empty, GetSizeGuide)
            .WithName(nameof(GetSizeGuide))
            .Produces<SizeGuideTableResponse>();
    }

    private async Task<IResult> GetSizeGuide([FromBody] GetSizeGuideQuery getSizeGuideQuery, ISender sender,
        CancellationToken cancellationToken = default)
    {
        return Results.Ok(await sender.Send(getSizeGuideQuery, cancellationToken));
    }
}