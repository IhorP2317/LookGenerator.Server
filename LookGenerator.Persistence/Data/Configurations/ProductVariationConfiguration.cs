using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class ProductVariationConfiguration:BaseEntityConfiguration<ProductVariation>
    {
        public override void Configure(EntityTypeBuilder<ProductVariation> builder)
        {
            base.Configure(builder);
            builder.Property(p => p.Price)
                .HasColumnType("numeric(18,2)"); 

        }
    }