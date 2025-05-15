using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LookGenerator.Application.Abstractions ;

    public interface IApplicationDbContext
    { 
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
       DbSet<AttributeOption> AttributeOptions { get; set; }
        DbSet<AttributeType> AttributeTypes { get; set; }
        DbSet<Colour> Colours { get; set; }
        DbSet<Look> Looks { get; set; }
        DbSet<LookProductVariation> LookProductVariations { get; set; }
        DbSet<MasterSizeIdentifier> MasterSizeIdentifiers { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<ProductAttributeOption> ProductAttributeOptions { get; set; }
        DbSet<ProductCategory> ProductCategories { get; set; }
        DbSet<ProductImage> ProductImages { get; set; }
        DbSet<ProductItem> ProductItems { get; set; }
        DbSet<ProductVariation> ProductVariations { get; set; }
         DbSet<SizeCategory> SizeCategories { get; set; }
        DbSet<SizeOption> SizeOptions { get; set; }
        DbSet<SizeOptionMasterIdentifier> SizeOptionMasterIdentifiers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reaction> Reactions { get; set; }

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry Entry(object entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
        
    }