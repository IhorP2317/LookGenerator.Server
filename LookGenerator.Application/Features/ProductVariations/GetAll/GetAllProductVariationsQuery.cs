using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.ProductVariation;

namespace LookGenerator.Application.Features.ProductVariations.GetAll;

public record GetAllProductVariationsQuery:IQuery<ICollection<ProductVariationResponse>>
{
    public Dictionary<ProductVariationFilterType, object> Filters { get; set; } = new();
}