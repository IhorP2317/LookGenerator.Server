using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class MasterSizeIdentifierConfiguration:BaseEntityConfiguration<MasterSizeIdentifier>
    {
        public override void Configure(EntityTypeBuilder<MasterSizeIdentifier> builder)
        {
            base.Configure(builder);
            builder.HasIndex(m => m.Identifier);
            builder.HasMany(m => m.ProductVariations)
                .WithOne(p => p.MasterSizeIdentifier)
                .HasForeignKey(p => p.MasterSizeIdentifierId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(b => b.Creator)
                .WithMany(u => u.MasterSizeIdentifiers) // Proper back navigation
                .HasForeignKey(b => b.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }