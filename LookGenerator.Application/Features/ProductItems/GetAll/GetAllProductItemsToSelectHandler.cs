using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.Product;
using LookGenerator.Application.Common.Helpers;
using LookGenerator.Application.Common.Mappers;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.ProductItems.GetAll;

public class GetAllProductItemsToSelectHandler(IApplicationDbContext applicationDbContext)
    : IQueryHandler<GetAllProductItemsToSelectQuery, PagedList<ProductToSelectResponse>>
{
    public async Task<PagedList<ProductToSelectResponse>> Handle(GetAllProductItemsToSelectQuery request,
        CancellationToken cancellationToken)
    {
        var productItemsQuery =
            applicationDbContext.ProductItems.AsNoTracking()
                .Include(productItem => productItem.Product)
                .ThenInclude(p => p.ProductAttributeOptions)
                .Include(p => p.Colour)
                .Include(p => p.Images)
                .AsQueryable();;
        productItemsQuery = request.Filters.Aggregate(productItemsQuery,
            (current, filter) => ProductFiltersHelper.GetProductFilter(filter)(current));
        
        var pageNumber = request.Filters.TryGetValue(ProductFilterType.PageNumber, out var pageNumberObj) &&
                         int.TryParse(pageNumberObj.ToString(), out var parsedPageNumber)
            ? parsedPageNumber
            : 1;
        var pageSize = request.Filters.TryGetValue(ProductFilterType.PageSize, out var pageSizeObj) &&
                       int.TryParse(pageSizeObj.ToString(), out var parsedPageSize)
            ? parsedPageSize
            : await productItemsQuery.CountAsync(cancellationToken);
        var pagedProductItems = await PagedList<ProductItem>.CreateAsync(
            productItemsQuery, pageNumber, pageSize, cancellationToken
        );
        return pagedProductItems.ToPagedSelectResponse();
    }
}