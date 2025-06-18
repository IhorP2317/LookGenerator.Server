using LookGenerator.Application.Common.DTOs.Look;
using LookGenerator.Application.Common.Helpers;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Mappers;

public static class LookMapper
{
    public static LookResponse ToResponse(this Look look,
        ICollection<ProductVariation> matchingVariations,
        Dictionary<Guid, (Dictionary<string, List<string>> attributes, List<string> categories)>? productDescriptions =
            null,
        Dictionary<Guid, Dictionary<string, SizeOption>>? topSizeMap = null,
        Dictionary<Guid, Dictionary<string, SizeOption>>? bottomSizeMap = null,
        Dictionary<Guid, Dictionary<string, SizeOption>>? footwearSizeMap = null, User? creator = null,
        int likeCount = 0, int pinCount = 0, bool isLiked = false, bool isPinned = false)
    {
        var products = matchingVariations
            .GroupBy(mv => mv.ProductItemId)
            .Select(g =>
            {
                var productItem = g.First().ProductItem;

                var sizeMap = productItem.Product.BodyZone switch
                {
                    ProductBodyZone.UpperBody => topSizeMap,
                    ProductBodyZone.LowerBody => bottomSizeMap,
                    ProductBodyZone.Feet => footwearSizeMap,
                    _ => null
                };

                return productItem.ToLookProductResponse(
                    productDescriptions?.GetValueOrDefault(productItem.ProductId).categories,
                    productDescriptions?.GetValueOrDefault(productItem.ProductId).attributes,
                    sizeMap
                );
            })
            .ToList();

        return new LookResponse(
            Id: look.Id,
            Name: look.Name,
            Description: look.Description,
            ColorPalette: look.ColorPalette,
            LookStatus: look.LookStatus,
            CreatedAt: look.CreatedAt,
            CreatedBy: look.CreatedBy,
            ModifiedAt: look.ModifiedAt,
            ModifiedBy: look.ModifiedBy,
            Products: products,
            Creator: creator.ToLookResponse(),
            LikeCount: likeCount,
            PinCount: pinCount,
            IsLiked: isLiked,
            IsPinned: isPinned,
            TotalPrice: products.Select(p => p.Price).Sum()
        );
    }

    public static PagedList<FeedLookResponse> ToPagedFeedResponse(
        this PagedList<Look> looks,
        Dictionary<Guid, (int LikeCount, int PinCount)>? reactionCounts = null,
        HashSet<Guid>? likedIds = null,
        HashSet<Guid>? pinnedIds = null)
    {
        var responses = looks.Items.Select(look =>
        {
            (int LikeCount, int PinCount) counts = (0, 0);
            if (reactionCounts != null && reactionCounts.TryGetValue(look.Id, out var foundCounts))
            {
                counts = foundCounts;
            }

            var isLiked = likedIds?.Contains(look.Id) ?? false;
            var isPinned = pinnedIds?.Contains(look.Id) ?? false;

            return look.ToFeedResponse(
                likeCount: counts.LikeCount,
                pinCount: counts.PinCount,
                isLiked: isLiked,
                isPinned: isPinned
            );
        }).ToList();

        return new PagedList<FeedLookResponse>(responses, looks.Page, looks.PageSize, looks.TotalCount);
    }

    public static FeedLookResponse ToFeedResponse(this Look look, int likeCount = 0, int pinCount = 0,
        bool isLiked = false, bool isPinned = false)
    {
        return new FeedLookResponse(
            Id: look.Id,
            Name: look.Name,
            Description: look.Description,
            ColorPalette: look.ColorPalette,
            LookStatus: look.LookStatus,
            ProductImageUrls: look.LookProductVariations
                .Select(lpv => lpv.ProductVariation.ProductItem)
                .DistinctBy(pi => pi.Id)
                .ToList()
                .Select(pi =>pi.Images.Select(i => i.ImageUrl).FirstOrDefault())
                .Distinct()
                .ToList(),
            LikeCount: likeCount,
            PinCount: pinCount,
            IsLiked: isLiked,
            IsPinned: isPinned,
            
            Creator: look.Creator.ToLookResponse()
        );
    }
}