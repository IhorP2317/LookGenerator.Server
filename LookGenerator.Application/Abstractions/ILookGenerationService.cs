using LookGenerator.Application.Common.DTOs;
using LookGenerator.Application.Common.DTOs.Look;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Abstractions;

public interface ILookGenerationService
{
    Task<LookGenerationRequest>
        GetDenormalizedProductsAsync(string gender,
            ICollection<Guid> attributeOptionIds,
            List<Guid> topSizeIds,
            List<Guid> bottomSizeIds,
            List<Guid> footwearSizeIds,
            CancellationToken cancellationToken
        );

    Task<LooksGenerationResponse> GenerateLooksAsync(
        LookGenerationRequest categoryHierarchy,
        string prioritizedColorList,
        string? fixedProductJson,
        CancellationToken ct);

    Task<List<LookResponse>> MapAndSaveLooksAsync(
        LooksGenerationResponse? response,
        LookGenerationRequest categoryHierarchy,
        Dictionary<Guid, Dictionary<string, SizeOption>> topSizeMap,
        Dictionary<Guid, Dictionary<string, SizeOption>> bottomSizeMap,
        Dictionary<Guid, Dictionary<string, SizeOption>> footwearSizeMap,
        CancellationToken cancellationToken);
}