using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.AttributeOption;
using LookGenerator.Application.Common.Mappers;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.AttributeOptions.GetAttributeOptions;

public class GetAttributeOptionsHandler(IApplicationDbContext applicationDbContext)
    : IQueryHandler<GetAttributeOptionsQuery, ICollection<AttributeOptionResponse>>
{
    public async Task<ICollection<AttributeOptionResponse>> Handle(GetAttributeOptionsQuery request,
        CancellationToken cancellationToken)
    {
        var attributeOptions = await applicationDbContext.AttributeOptions
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return attributeOptions.ToResponses();
    }
}