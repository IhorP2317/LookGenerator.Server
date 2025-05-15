using Carter;
using LookGenerator.Application.Features.Colours;
using MediatR;

namespace LookGenerator.WebAPI.Endpoints;

public class ColourEndpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/colours")
            .WithOpenApi();
        group.MapGet(string.Empty, GetColours)
            .WithName(nameof(GetColours))
            .Produces<ICollection<string>>();
    }

    private async Task<IResult> GetColours(
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        return Results.Ok(await sender.Send(new GetColoursQuery(), cancellationToken));
    }
}