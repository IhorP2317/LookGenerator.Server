using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.Look;
using LookGenerator.Application.Common.Helpers;
using LookGenerator.Application.Common.Mappers;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Looks.GetPaged;

public class GetLooksHandler(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    : IQueryHandler<GetLooksQuery, PagedList<FeedLookResponse>>
{
    public async Task<PagedList<FeedLookResponse>> Handle(GetLooksQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = string.IsNullOrWhiteSpace(currentUserService.UserId)
            ? (Guid?)null
            : Guid.Parse(currentUserService.UserId!);

        var looksQuery = applicationDbContext.Looks
            .AsNoTracking()
            .Include(l => l.Creator)
            .Include(l => l.LookProductVariations)
            .ThenInclude(lp => lp.ProductVariation)
            .ThenInclude(pv => pv.ProductItem)
            .ThenInclude(pi => pi.Product)
            .ThenInclude(p => p.ProductAttributeOptions)
            .ThenInclude(p => p.AttributeOption)
            .Include(l => l.LookProductVariations)
            .ThenInclude(lp => lp.ProductVariation)
            .ThenInclude(pv => pv.ProductItem)
            .ThenInclude(pi => pi.Colour)
            .Include(l => l.LookProductVariations)
            .ThenInclude(lp => lp.ProductVariation)
            .ThenInclude(pv => pv.ProductItem)
            .ThenInclude(pi => pi.Images)
            .AsQueryable();


        looksQuery = request.Filters.Aggregate(looksQuery,
            (current, filter) => LookFiltersHelper.GetLookFilter(filter, currentUserService)(current));

        var pageNumber = request.Filters.TryGetValue(LookFilterType.PageNumber, out var pageNumberObj) &&
                         int.TryParse(pageNumberObj.ToString(), out var parsedPageNumber)
            ? parsedPageNumber
            : 1;
        var pageSize = request.Filters.TryGetValue(LookFilterType.PageSize, out var pageSizeObj) &&
                       int.TryParse(pageSizeObj.ToString(), out var parsedPageSize)
            ? parsedPageSize
            : await looksQuery.CountAsync(cancellationToken);
        var pagedLooks = await PagedList<Look>.CreateAsync(
            looksQuery, pageNumber, pageSize, cancellationToken
        );
        var lookIds = pagedLooks.Items.Select(l => l.Id).ToList();
        var reactionCounts = await applicationDbContext.Reactions
            .Where(r => lookIds.Contains(r.LookId) && (r.Type == ReactionType.Like || r.Type == ReactionType.Pin))
            .GroupBy(r => r.LookId)
            .ToDictionaryAsync(
                g => g.Key,
                g => (
                    LikeCount: g.Count(r => r.Type == ReactionType.Like),
                    PinCount: g.Count(r => r.Type == ReactionType.Pin)
                ),
                cancellationToken
            );


        Dictionary<Guid, bool> userLiked = new();
        Dictionary<Guid, bool> userPinned = new();

        if (!currentUserId.HasValue)
            return pagedLooks.ToPagedFeedResponse(
                reactionCounts,
                userLiked.Keys.ToHashSet(),
                userPinned.Keys.ToHashSet()
            );
        {
            var userReactions = await applicationDbContext.Reactions
                .Where(r => lookIds.Contains(r.LookId) &&
                            r.CreatedBy == currentUserId &&
                            (r.Type == ReactionType.Like || r.Type == ReactionType.Pin))
                .ToListAsync(cancellationToken);

            userLiked = userReactions
                .Where(r => r.Type == ReactionType.Like)
                .ToDictionary(r => r.LookId, _ => true);

            userPinned = userReactions
                .Where(r => r.Type == ReactionType.Pin)
                .ToDictionary(r => r.LookId, _ => true);
        }

        return pagedLooks.ToPagedFeedResponse(
            reactionCounts,
            userLiked.Keys.ToHashSet(),
            userPinned.Keys.ToHashSet()
        );

    }
}