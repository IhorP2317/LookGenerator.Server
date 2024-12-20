using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class ProductAttributeOptionConfiguration:IEntityTypeConfiguration<ProductAttributeOption>
    {
        public virtual void Configure(EntityTypeBuilder<ProductAttributeOption> builder)
        {
            builder.HasKey(pa => new { pa.ProductId, pa.AttributeOptionId });
            builder.HasOne(pa => pa.Product)
                .WithMany(p => p.ProductAttributeOptions)
                .HasForeignKey(pa => pa.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pa => pa.AttributeOption)
                .WithMany(a => a.ProductAttributeOptions)
                .HasForeignKey(pa => pa.AttributeOptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }