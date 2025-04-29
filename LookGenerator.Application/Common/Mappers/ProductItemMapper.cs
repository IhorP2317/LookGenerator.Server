using LookGenerator.Application.Common.DTOs;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Mappers;

public static class ProductItemMapper
{
    /// <summary>
    /// Maps a ProductItem entity to a ProductItemRequest with only the required data
    /// </summary>
    public static ProductItemRequest ToRequest(this ProductItem productItem)
    {
        return new ProductItemRequest(
            Id: productItem.Id,
            Colour: productItem.Colour.ToRequest()
        );
    }
}