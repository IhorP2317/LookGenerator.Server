using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class LookProductVariationConfiguration:IEntityTypeConfiguration<LookProductVariation>
    {
        public void Configure(EntityTypeBuilder<LookProductVariation> builder)
        {
            builder.HasKey(lp => new { lp.LookId, lp.ProductVariationId });
            builder.HasOne(lp => lp.Look)
                .WithMany(l => l.LookProductVariations)
                .HasForeignKey(lp => lp.LookId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(lp => lp.ProductVariation)
                .WithMany(p => p.LookProductVariations)
                .HasForeignKey(lp => lp.ProductVariationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }