using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class MasterSizeIdentifierConfiguration:IEntityTypeConfiguration<MasterSizeIdentifier>
    {
        public virtual void Configure(EntityTypeBuilder<MasterSizeIdentifier> builder)
        {
            builder.HasKey(m => m.Identifier);
            builder.HasOne(m => m.SizeOption)
                .WithMany(s => s.MasterSizeIds)
                .HasForeignKey(m => m.SizeOptionId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(m => m.ProductVariations)
                .WithOne(p => p.MasterSizeIdentifier)
                .HasForeignKey(p => p.MasterSizeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }