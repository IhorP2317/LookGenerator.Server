using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class MasterSizeIdentifierConfiguration:BaseEntityConfiguration<MasterSizeIdentifier>
    {
        public override void Configure(EntityTypeBuilder<MasterSizeIdentifier> builder)
        {
            builder.HasMany(m => m.ProductVariations)
                .WithOne(p => p.MasterSizeIdentifier)
                .HasForeignKey(p => p.MasterSizeIdentifierId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }