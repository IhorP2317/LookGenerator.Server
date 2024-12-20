using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class ProductImageConfiguration:BaseEntityConfiguration<ProductImage>
    {
        public override void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            base.Configure(builder);
            builder.HasIndex(pi => pi.ImageUrl);
            builder.HasOne(pi => pi.ProductItem)
                .WithMany(pi => pi.Images)
                .HasForeignKey(pi => pi.ProductItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }