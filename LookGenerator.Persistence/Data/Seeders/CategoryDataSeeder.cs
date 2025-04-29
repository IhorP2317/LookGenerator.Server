using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Persistence.Data.Seeders;

public class CategoryDataSeeder(IApplicationDbContext applicationDbContext) : IDataSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var parentCategories = new Dictionary<string, ProductCategory>();
        foreach (var category in _categories)
        {
            var result = await EnsureCategoryExistsAsync(category.Name, category.Description, category.ParentId,
                cancellationToken);
            parentCategories[category.Name] = result;
        }

        foreach (var subcategory in _subcategories)
        {
            await EnsureCategoryExistsAsync(subcategory.Name, subcategory.Description,
                parentCategories[subcategory.ParentName].Id,
                cancellationToken);
        }
    }

    public void Seed()
    {
        var parentCategories = new Dictionary<string, ProductCategory>();
        foreach (var category in _categories)
        {
            parentCategories[category.Name] = EnsureCategoryExists(category.Name, category.Description, category.ParentId);
        }

        foreach (var subcategory in _subcategories)
        {
             EnsureCategoryExists(subcategory.Name, subcategory.Description,
                parentCategories[subcategory.ParentName].Id);
        }
    }

    private async Task<ProductCategory> EnsureCategoryExistsAsync(string name, string description,
        Guid? categoryId = null, CancellationToken cancellationToken = default)
    {
        var existingCategory =
            await applicationDbContext.ProductCategories.FirstOrDefaultAsync(
                pc => pc.Name == name && pc.ParentCategoryId == categoryId, cancellationToken);
        if (existingCategory != null)
        {
            return existingCategory;
        }

        var newCategory = new ProductCategory
        {
            Name = name,
            Description = description,
            ParentCategoryId = categoryId
        };
        await applicationDbContext.ProductCategories.AddAsync(newCategory, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return newCategory;
    }

    private ProductCategory EnsureCategoryExists(string name, string description,
        Guid? categoryId = null)
    {
        var existingCategory =
            applicationDbContext.ProductCategories.FirstOrDefault(
                pc => pc.Name == name && pc.ParentCategoryId == categoryId);
        if (existingCategory != null)
        {
            return existingCategory;
        }

        var newCategory = new ProductCategory
        {
            Name = name,
            Description = description,
            ParentCategoryId = categoryId
        };
        applicationDbContext.ProductCategories.Add(newCategory);
        applicationDbContext.SaveChanges();
        return newCategory;
    }

    private readonly ICollection<(string Name, string Description, Guid? ParentId)> _categories =
    [
        ("Men", "Men's clothing and accessories", null),
        ("Women", "Women's clothing and accessories", null)
    ];

    private readonly ICollection<(string Name, string Description, string ParentName)> _subcategories =
    [
        ("Clothing", "Men's clothes", "Men"),
        ("Footwear", "Men's shoes", "Men"),
        ("Accesories", "Men's accessories", "Men"),
        ("Clothing", "Women's clothes", "Women"),
        ("Footwear", "Women's shoes", "Women"),
        ("Accesories", "Women's accessories", "Women")
    ];
}