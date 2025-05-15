using Carter;
using LookGenerator.Application.Common.DTOs.AttributeOption;
using LookGenerator.Application.Features.AttributeOptions;
using MediatR;

namespace LookGenerator.WebAPI.Endpoints;

public class AttributeOptionEndpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/attribute-options")
            .WithOpenApi();
        group.MapGet(string.Empty, GetAttributeOptions)
            .WithName(nameof(GetAttributeOptions))
            .Produces<ICollection<AttributeOptionResponse>>();
    }

    private async Task<IResult> GetAttributeOptions( ISender sender,
        CancellationToken cancellationToken = default)
    {
        return Results.Ok(await sender.Send(new GetAttributeOptionsQuery(),cancellationToken));
    }
}