using LookGenerator.Application.Common.DTOs;
using LookGenerator.Application.Common.DTOs.Look;
using LookGenerator.Application.Features.Looks.Generate;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Mappers;

public static class ColourMapper
{
    /// <summary>
    /// Maps a Colour entity to a ColourRequest with only the required data
    /// </summary>
    public static ColourRequest ToRequest(this Colour colour)
    {
        return new ColourRequest(
            Id: colour.Id,
            Name: colour.Name
        );
    }

    
    /// <summary>
    /// Maps a collection of ProductItem entities to ProductItemRequest objects
    /// </summary>
    public static ICollection<ProductItemRequest> ToRequests(this IEnumerable<ProductItem>? productItems)
    {
        return productItems?.Select(pi => pi.ToRequest()).ToList() ?? [];
    }
}