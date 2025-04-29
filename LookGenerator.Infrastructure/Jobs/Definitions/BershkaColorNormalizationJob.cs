using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;
using LookGenerator.Infrastructure.Jobs.Constants;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace LookGenerator.Infrastructure.Jobs.Definitions;
[DisallowConcurrentExecution]
public class BershkaColorNormalizationJob(IApplicationDbContext applicationDbContext):IJob
{
   public async Task Execute(IJobExecutionContext context)
{
    var allColors = await applicationDbContext.Colours.ToListAsync();
    
    foreach (var color in allColors)
    {
        if (!BershkaJobConstants.ColorMappings.TryGetValue(color.Name, out var normalizedName) ||
            string.Equals(color.Name, normalizedName, StringComparison.OrdinalIgnoreCase)) continue;
        color.Name = normalizedName;
        applicationDbContext.Colours.Update(color);
    }

    await applicationDbContext.SaveChangesAsync();
}

   

}