using System.Text.Json;
using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;
using LookGenerator.Infrastructure.Models.Responses;
using LookGenerator.Infrastructure.Models.Responses.SizeGuide;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LookGenerator.Infrastructure.Jobs.Definitions;

public class BershkaSizeGuideParsingJob(IHttpClientFactory httpClientFactory, ILogger<BershkaSizeGuideParsingJob> logger, IApplicationDbContext applicationDbContext):IJob
{
    private readonly HttpClient _httpClient =  httpClientFactory.CreateClient("NoBaseUriClient");
    private string SizeGuideUrl =>  $"https://static.bershka.net/4/static/itxwebstandard/config/size-guide-data.json?t={DateTime.UtcNow:yyyyMMddHHmmss}";
     
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var response = await _httpClient.GetAsync(SizeGuideUrl);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();

            var sizeGuideData = JsonSerializer.Deserialize<BershkaSizeGuideResponse>(json);
            if (sizeGuideData != null)
                await SaveSizeGuideAsync(sizeGuideData);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
        }
    }
private async Task SaveSizeGuideAsync(BershkaSizeGuideResponse response, CancellationToken cancellationToken = default)
{
    var genderCategories = new Dictionary<string, Func<SizeCategoryData, GenderSizeData?>>
    {
        { "MEN", cat => cat.BERSHKA_MAN },
        { "WOMEN", cat => cat.BERSHKA_WOMAN }
    };

    var categoryTypes = new Dictionary<string, SizeCategoryData?>
    {
        { nameof(response.SizeGuide.TOP), response.SizeGuide.TOP },
        { nameof(response.SizeGuide.BOTTOM), response.SizeGuide.BOTTOM },
        { nameof(response.SizeGuide.SHOES), response.SizeGuide.SHOES },
    };

  
    var existingIdentifiers = await applicationDbContext.MasterSizeIdentifiers
        .ToDictionaryAsync(msi => msi.Identifier, cancellationToken);

    foreach (var (genderName, getGenderData) in genderCategories)
    {
        // Gender category
        var genderCategory = await applicationDbContext.SizeCategories
            .Include(c => c.SubCategories)!
                .ThenInclude(sc => sc.SubCategories)!
                    .ThenInclude(sc => sc.SubCategories)!
                        .ThenInclude(sc => sc.SizeOptions)!
                            .ThenInclude(so => so.MasterSizeIdentifiers)
            .FirstOrDefaultAsync(sc => sc.Name == genderName && sc.ParentCategoryId == null, cancellationToken);

        if (genderCategory is null)
        {
            genderCategory = new SizeCategory
            {
                Name = genderName,
                SubCategories = new List<SizeCategory>()
            };
            applicationDbContext.SizeCategories.Add(genderCategory);
        }

        foreach (var (categoryName, categoryData) in categoryTypes)
        {
            var genderData = getGenderData(categoryData!);
            if (genderData is null) continue;

            var itemCategory = genderCategory.SubCategories!
                .FirstOrDefault(sc => sc.Name == categoryName);

            if (itemCategory is null)
            {
              
              

                itemCategory = new SizeCategory
                {
                    Name = categoryName,
                    ParentCategory = genderCategory,
                    SubCategories = new List<SizeCategory>()
                };
                genderCategory.SubCategories!.Add(itemCategory);
            }

            foreach (var block in genderData.Blocks)
            {
                var correctedName = block.Title.Contains(".body.")
                    ? "BODY" : block.Title.Split('.').Last(); 
                var blockCategory = itemCategory.SubCategories!
                    .FirstOrDefault(sc => sc.Name == correctedName);
                

                if (blockCategory is null)
                {
                    blockCategory = new SizeCategory
                    {
                        Name = correctedName,
                        ParentCategory = itemCategory,
                        SubCategories = new List<SizeCategory>()
                    };
                    itemCategory.SubCategories!.Add(blockCategory);
                }

                foreach (var field in block.Fields)
                {
                    correctedName = field.Name.Contains(".body.")
                        ? "BODY" : field.Name.Split('.').Last(); 
                    var fieldCategory = blockCategory.SubCategories!
                        .FirstOrDefault(sc => sc.Name == correctedName);

                    if (fieldCategory is null)
                    {
                        
                        fieldCategory = new SizeCategory
                        {
                            Name = correctedName,
                            ParentCategory = blockCategory,
                            SizeOptions = new List<SizeOption>()
                        };
                        blockCategory.SubCategories!.Add(fieldCategory);
                    }

                    foreach (var sizeValue in field.Sizes)
                    {
                        var sizeOption = fieldCategory.SizeOptions!
                            .FirstOrDefault(so => Math.Abs(so.Cm - sizeValue.Cm) < 0.001 &&
                                                  Math.Abs(so.Inch - sizeValue.Inch) < 0.001);

                        if (sizeOption is null)
                        {
                            sizeOption = new SizeOption
                            {
                                Cm = sizeValue.Cm,
                                Inch = sizeValue.Inch,
                                SizeCategory = fieldCategory
                            };
                            fieldCategory.SizeOptions!.Add(sizeOption);
                            await applicationDbContext.SaveChangesAsync(cancellationToken);
                        }

                        foreach (var identifierText in sizeValue.Sizes)
                        {
                            if (!existingIdentifiers.TryGetValue(identifierText, out var identifier))
                            {
                                identifier = new MasterSizeIdentifier
                                {
                                    Identifier = identifierText,
                                };
                                existingIdentifiers[identifierText] = identifier;
                                await applicationDbContext.MasterSizeIdentifiers.AddAsync(identifier, cancellationToken);
                                await applicationDbContext.SaveChangesAsync(cancellationToken);
                            }

                            var exists = await applicationDbContext.SizeOptionMasterIdentifiers
                                .AnyAsync(link => link.SizeOptionId == sizeOption.Id && link.MasterIdentifierId == identifier.Id, cancellationToken);

                            if (exists) continue;
                            var newLink = new SizeOptionMasterIdentifier
                            {
                                MasterIdentifierId = identifier.Id,
                                SizeOptionId = sizeOption.Id
                            };
                            applicationDbContext.SizeOptionMasterIdentifiers.Add(newLink);
                            await applicationDbContext.SaveChangesAsync(cancellationToken);
                        }
                    }
                }
            }
        }
    }

    await applicationDbContext.SaveChangesAsync(cancellationToken);
}

}