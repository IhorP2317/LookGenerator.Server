using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Abstractions ;

    public interface IApplicationDbContext
    {
        public DbSet<AttributeOption> AttributeOptions { get; set; }
        public DbSet<AttributeType> AttributeTypes { get; set; }
        public DbSet<Look> Looks { get; set; }
        public DbSet<LookProductVariation> LookProductVariations { get; set; }
        public DbSet<MasterSizeIdentifier> MasterSizeIdentifiers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAttributeOption> ProductAttributeOptions { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }
        public DbSet<ProductVariation> ProductVariations { get; set; }
        public DbSet<SizeCategory> SizeCategories { get; set; }
        public DbSet<SizeOption> SizeOptions { get; set; }
        public DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }