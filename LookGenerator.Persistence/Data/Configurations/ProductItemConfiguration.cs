using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class ProductItemConfiguration:BaseEntityConfiguration<ProductItem>
    {
        public override void Configure(EntityTypeBuilder<ProductItem> builder)
        {
            base.Configure(builder);
            builder.HasMany(pi => pi.Variations)
                .WithOne(pv => pv.ProductItem)
                .HasForeignKey(pv => pv.ProductItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }