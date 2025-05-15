using Carter;
using LookGenerator.Application.Common.DTOs.Product;
using LookGenerator.Application.Common.Helpers;
using LookGenerator.Application.Features.ProductItems.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LookGenerator.WebAPI.Endpoints;

public class ProductItemEndpoints:CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/product-items")
            .WithOpenApi();
        group.MapPost(string.Empty, GetAllProductItemsToSelect)
            .WithName(nameof(GetAllProductItemsToSelect))
            .Produces<PagedList<ProductToSelectResponse>>();


    }
    private async Task<IResult> GetAllProductItemsToSelect([FromBody] GetAllProductItemsToSelectQuery getAllProductItemsToSelectQuery,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var productItems = await sender.Send(getAllProductItemsToSelectQuery, cancellationToken);

        return Results.Ok(productItems);
    }
}