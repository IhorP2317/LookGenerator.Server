using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Persistence.Data.Seeders;

public class AttributeDataSeeder(IApplicationDbContext applicationDbContext) : IDataSeeder
{
    public  async Task SeedAsync(CancellationToken cancellationToken = default)
    {
       
        var attributeTypes = new Dictionary<string, AttributeType>();
        foreach (var type in _attributeTypes)
        {
            var result = await EnsureAttributeTypeAsync(new AttributeType { Name = type.Name }, cancellationToken);
            attributeTypes[type.Name] = result;
        }
        
      
        foreach (var option in _attributeOptions)
        {
            await EnsureAttributeOptionAsync(new AttributeOption 
            { 
                Name = option.Name, 
                AttributeTypeId = attributeTypes[option.AttributeTypeName].Id 
            }, cancellationToken);
        }
    }

    public void Seed()
    {
        var attributeTypes = new Dictionary<string, AttributeType>();
        foreach (var attributeType in _attributeTypes)
        {
            var result = EnsureAttributeType(attributeType);
            attributeTypes[attributeType.Name] = result;
        }

        foreach (var option in _attributeOptions)
        {
            EnsureAttributeOption(new AttributeOption 
            { 
                Name = option.Name, 
                AttributeTypeId = attributeTypes[option.AttributeTypeName].Id 
            });
        }

    }

    private async Task<AttributeType> EnsureAttributeTypeAsync(AttributeType attributeType,
        CancellationToken cancellationToken = default)
    {
        var existingType =
            await applicationDbContext.AttributeTypes.FirstOrDefaultAsync(a => a.Name == attributeType.Name,
                cancellationToken);
        if (existingType != null)
            return existingType;
        var newType = new AttributeType
        {
            Name = attributeType.Name,
        };
        await applicationDbContext.AttributeTypes.AddAsync(newType, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return newType;
    }
    private  AttributeType EnsureAttributeType(AttributeType attributeType)
    {
        var existingType =
             applicationDbContext.AttributeTypes.FirstOrDefault(a=> a.Name == attributeType.Name);
        if (existingType != null)
            return existingType;
        var newType = new AttributeType
        {
            Name = attributeType.Name,
        };
         applicationDbContext.AttributeTypes.Add(newType);
         applicationDbContext.SaveChanges();
        return newType;
    }

    private async Task EnsureAttributeOptionAsync(AttributeOption attributeOption,
        CancellationToken cancellationToken = default)
    {
        var existingOption = await applicationDbContext.AttributeOptions.FirstOrDefaultAsync(
            a => a.Name == attributeOption.Name && a.AttributeTypeId == attributeOption.AttributeTypeId,
            cancellationToken);
        if(existingOption != null) return;
        var newOption = new AttributeOption
        {
            Name = attributeOption.Name,
            AttributeTypeId = attributeOption.AttributeTypeId,
        };
        await applicationDbContext.AttributeOptions.AddAsync(newOption, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
    private void EnsureAttributeOption(AttributeOption attributeOption)
    {
        var existingOption = applicationDbContext.AttributeOptions.FirstOrDefault(
            a => a.Name == attributeOption.Name && a.AttributeTypeId == attributeOption.AttributeTypeId);
        if(existingOption != null) return;
        var newOption = new AttributeOption
        {
            Name = attributeOption.Name,
            AttributeTypeId = attributeOption.AttributeTypeId,
        };
        applicationDbContext.AttributeOptions.Add(newOption);
        applicationDbContext.SaveChanges();
    }
    private readonly ICollection<AttributeType> _attributeTypes =
    [
        new() { Name = "Style" },
        new() { Name = "Season" },
        new() { Name = "Occasion" }
    ];
    private readonly ICollection<(string Name, string AttributeTypeName)> _attributeOptions =
    [
        ("StreetWear", "Style"),
        ("Trendy", "Style"),
        ("Basic", "Style"),
        ("Cozy", "Style"),
        ("Summer", "Season"),
        ("Tailoring", "Season"),
        ("Casual", "Occasion"),
        ("Party", "Occasion"),
        ("Festivals", "Occasion")
    ];

}