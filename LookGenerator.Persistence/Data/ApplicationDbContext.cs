using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;
using LookGenerator.Persistence.Data.Configurations;
using LookGenerator.Persistence.Data.Interceptors;
using LookGenerator.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LookGenerator.Persistence.Data ;

    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IOptions<AdminSettings> adminSettings) : DbContext(options),
        IApplicationDbContext
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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new AttributeOptionConfiguration());
            modelBuilder.ApplyConfiguration(new AttributeTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LookConfiguration());
            modelBuilder.ApplyConfiguration(new LookProductVariationConfiguration());
            modelBuilder.ApplyConfiguration(new MasterSizeIdentifierConfiguration());
            modelBuilder.ApplyConfiguration(new ProductAttributeOptionConfiguration());
            modelBuilder.ApplyConfiguration(new ProductCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductImageConfiguration());
            modelBuilder.ApplyConfiguration(new ProductItemConfiguration());
            modelBuilder.ApplyConfiguration(new ProductVariationConfiguration());
            modelBuilder.ApplyConfiguration(new SizeCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new SIzeOptionConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration(adminSettings.Value));
        }
    }