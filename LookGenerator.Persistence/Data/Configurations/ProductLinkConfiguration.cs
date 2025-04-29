using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations;

public class ProductLinkConfiguration:BaseEntityConfiguration<ProductLink>
{
    public override void Configure(EntityTypeBuilder<ProductLink> builder)
    {
        base.Configure(builder);
        builder.HasIndex(pl => new { pl.Url, pl.RegionName });
        builder.HasOne(pl => pl.ProductItem)
            .WithMany(pI => pI.Links)
            .HasForeignKey(pl => pl.ProductItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}