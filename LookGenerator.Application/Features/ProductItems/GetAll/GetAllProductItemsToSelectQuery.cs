using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.Product;
using LookGenerator.Application.Common.Helpers;

namespace LookGenerator.Application.Features.ProductItems.GetAll;

public class GetAllProductItemsToSelectQuery:IQuery<PagedList<ProductToSelectResponse>>
{
    public Dictionary<ProductFilterType, object> Filters { get; set; } = new();
}