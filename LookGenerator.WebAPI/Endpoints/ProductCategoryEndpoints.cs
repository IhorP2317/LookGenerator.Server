using Carter;
using LookGenerator.Application.Common.DTOs.ProductCategory;
using LookGenerator.Application.Features.ProductCategories.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LookGenerator.WebAPI.Endpoints;

public class ProductCategoryEndpoints:CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/product-categories")
            .WithOpenApi();
        group.MapPost(string.Empty, GetAllProductCategories)
            .WithName(nameof(GetAllProductCategories))
            .Produces<ICollection<ProductCategoryResponse>>();
    }

    private async Task<IResult> GetAllProductCategories([FromBody] GetAllProductCategoriesQuery query, ISender sender,
        CancellationToken cancellationToken = default)
    {
        return Results.Ok(await sender.Send(query, cancellationToken));
    }
}