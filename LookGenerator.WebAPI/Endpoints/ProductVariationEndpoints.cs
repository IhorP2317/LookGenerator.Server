using Carter;
using LookGenerator.Application.Common.DTOs.ProductVariation;
using LookGenerator.Application.Features.ProductVariations.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LookGenerator.WebAPI.Endpoints;

public class ProductVariationEndpoints:CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
       var group = app.MapGroup("/api/product-variations")
            .WithOpenApi();
        group.MapPost(string.Empty, GetAll)
            .WithName(nameof(GetAll))
            .Produces<ICollection<ProductVariationResponse>>();
    }
    private async Task<IResult> GetAll([FromBody] GetAllProductVariationsQuery getAllProductVariationsQuery, 
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var productVariations = await sender.Send(getAllProductVariationsQuery, cancellationToken);

        return Results.Ok(productVariations);
    }
}