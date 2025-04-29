using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs;
using LookGenerator.Application.Common.Exceptions;

namespace LookGenerator.Application.Features.Looks.Generate;

public class GenerateLooksHandler(
    ISizeGuideService sizeGuideService,
    ILookGenerationService lookGenerationService)
    : ICommandHandler<GenerateLooksCommand, ICollection<LookResponse>>
{
    public async Task<ICollection<LookResponse>> Handle(
        GenerateLooksCommand request,
        CancellationToken cancellationToken)
    {
        var (topSizeMap, bottomSizeMap, footwearSizeMap) = await sizeGuideService.GetSizeMapsAsync(request.Measurements,
            request.Gender,
            cancellationToken);
        var waistContour = request.Measurements["waistContour"];
        var beltIdentifiers =
            await sizeGuideService.FindMatchingBeltMasterSizesAsync(waistContour,
                request.Gender, cancellationToken);
        foreach (var beltIdentifier in beltIdentifiers.Where(beltIdentifier =>
                     beltIdentifier.Value.Count != 0 && !bottomSizeMap.ContainsKey(beltIdentifier.Key)))
        {
            bottomSizeMap.Add(beltIdentifier.Key, beltIdentifier.Value);
        }

        if (topSizeMap.Count == 0 ||
            bottomSizeMap.Count == 0 || footwearSizeMap.Count == 0)
        {
            throw new NotFoundException("No matching products found for the given measurements!");
        }


        var categoryHierarchy =
            await lookGenerationService.GetDenormalizedProductsAsync(request.Gender, request.AttributeOptionIds,
                topSizeMap.Keys.ToList(),
                bottomSizeMap.Keys.ToList(), footwearSizeMap.Keys.ToList(), cancellationToken);

        if (categoryHierarchy.Products.All(p => p.ProductItems.Count == 0))
        {
            throw new NotFoundException("No products found for the given categories!");
        }

        var prioritizedColorList =
            string.Join(", ", request.Colours.Select(c => c.Trim().ToLowerInvariant()).Distinct());

        var response =
            await lookGenerationService.GenerateLooksAsync(categoryHierarchy, prioritizedColorList, null,
                cancellationToken);
        if (response is { Description: not null })
            throw new BadRequestException(response.Description);

        var looksResponse = await lookGenerationService.MapAndSaveLooksAsync(response, categoryHierarchy, topSizeMap,
            bottomSizeMap, footwearSizeMap, cancellationToken);

        if (looksResponse.Count == 0)
            throw new NotFoundException("No valid looks generated!");

        return looksResponse;
    }
}