using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Common.Helpers;

public class PagedList<T>(List<T> items, int page, int pageSize, int totalCount)
{
    public List<T> Items { get; set; } = items;
    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;
    public int TotalCount { get; } = totalCount;

    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;


    public static async Task<PagedList<T>> CreateAsync( IQueryable<T> query, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<T>(items, page, pageSize, totalCount);
    }
}