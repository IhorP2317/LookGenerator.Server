using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.Look;
using LookGenerator.Application.Common.Mappers;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Looks.GetOneDetail;

public class GetLookHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    ISizeGuideService sizeGuideService)
    : IQueryHandler<GetLookQuery, LookResponse>
{
    public async Task<LookResponse> Handle(GetLookQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = string.IsNullOrWhiteSpace(currentUserService.UserId)
            ? (Guid?)null
            : Guid.Parse(currentUserService.UserId!);
        var look = await applicationDbContext.Looks
            .AsNoTracking()
            .Include(l => l.Creator)
            .Include(l => l.LookProductVariations)
            .ThenInclude(lp => lp.ProductVariation)
            .ThenInclude(pv => pv.ProductItem)
            .ThenInclude(pi => pi.Images)
            .Include(l => l.LookProductVariations)
            .ThenInclude(lp => lp.ProductVariation)
            .ThenInclude(pv => pv.ProductItem)
            .ThenInclude(pi => pi.Colour)
            .Include(l => l.LookProductVariations)
            .ThenInclude(lp => lp.ProductVariation)
            .ThenInclude(pv => pv.ProductItem)
            .ThenInclude(pi => pi.Links)
            .Include(l => l.LookProductVariations)
            .ThenInclude(lp => lp.ProductVariation)
            .ThenInclude(pv => pv.ProductItem)
            .ThenInclude(pi => pi.Product)
            .ThenInclude(p => p.ProductAttributeOptions)
            .ThenInclude(pao => pao.AttributeOption)
            .ThenInclude(ao => ao.AttributeType)
            .Include(l => l.LookProductVariations)
            .ThenInclude(lp => lp.ProductVariation)
            .ThenInclude(pv => pv.ProductItem)
            .ThenInclude(pi => pi.Product)
            .ThenInclude(p => p.ProductCategory)
            .FirstAsync(l => l.Id == request.Id, cancellationToken);

        var matchingVariations = look.LookProductVariations
            .Select(lpv => lpv.ProductVariation)
            .ToList();

        var productDescriptions = look.LookProductVariations
            .Select(lpv => lpv.ProductVariation.ProductItem.Product)
            .GroupBy(p => p.Id)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var p = g.First();
                    return (
                        attributes: p.ProductAttributeOptions
                            .GroupBy(pao => pao.AttributeOption.AttributeType.Name)
                            .ToDictionary(
                                gg => gg.Key,
                                gg => gg.Select(pao => pao.AttributeOption.Name).ToList()
                            ),
                        categories: new List<string> { p.ProductCategory.Name, p.BodyZone.ToString(), p.Gender }
                    );
                }
            );

        var reactionCounts = new
        {
            LikeCount = await applicationDbContext.Reactions.AsNoTracking().CountAsync(
                r => r.LookId == look.Id && r.Type == ReactionType.Like, cancellationToken),
            PinCount = await applicationDbContext.Reactions.AsNoTracking().CountAsync(
                r => r.LookId == look.Id && r.Type == ReactionType.Pin, cancellationToken)
        };


        var userLiked = false;
        var userPinned = false;

        var topSizeMap =
            new Dictionary<Guid, Dictionary<string, SizeOption>>();
        var bottomSizeMap = new Dictionary<Guid, Dictionary<string, SizeOption>>();
        var footwearSizeMap = new Dictionary<Guid, Dictionary<string, SizeOption>>();

        foreach (var variation in matchingVariations)
        {
            var sizes = await sizeGuideService.GetDimensionsAsync(variation, cancellationToken);
            switch (variation.ProductItem.Product.BodyZone)
            {
                case ProductBodyZone.UpperBody:
                    topSizeMap[variation.MasterSizeIdentifierId] = sizes;
                    break;
                case ProductBodyZone.LowerBody:
                    bottomSizeMap[variation.MasterSizeIdentifierId] = sizes;
                    break;
                case ProductBodyZone.Feet:
                    footwearSizeMap[variation.MasterSizeIdentifierId] = sizes;
                    break;
            }
        }


        if (currentUserId.HasValue)
        {
            var reactions = await applicationDbContext.Reactions.AsNoTracking()
                .Where(r => r.LookId == look.Id &&
                            r.CreatedBy == currentUserId &&
                            (r.Type == ReactionType.Like || r.Type == ReactionType.Pin))
                .Select(r => r.Type)
                .ToListAsync(cancellationToken);

            userLiked = reactions.Contains(ReactionType.Like);
            userPinned = reactions.Contains(ReactionType.Pin);

        }


        var lookResponse = look.ToResponse(
            matchingVariations,
            productDescriptions,
            topSizeMap,
            bottomSizeMap,
            footwearSizeMap,
            creator: look.Creator,
            likeCount: reactionCounts.LikeCount,
            pinCount: reactionCounts.PinCount,
            isLiked: userLiked,
            isPinned: userPinned
        );


        return lookResponse;
    }
}