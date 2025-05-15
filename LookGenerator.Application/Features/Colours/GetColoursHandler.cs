using LookGenerator.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Colours;

public class GetColoursHandler(IApplicationDbContext applicationDbContext)
    : IQueryHandler<GetColoursQuery, ICollection<string>>
{
    public async Task<ICollection<string>> Handle(GetColoursQuery request, CancellationToken cancellationToken)
    {
        var distinctColours = await applicationDbContext.Colours
            .AsNoTracking()
            .Select(x => x.Name.ToLower())
            .Distinct()
            .ToListAsync(cancellationToken);
        return distinctColours;
    }
}