using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class AttributeTypeConfiguration:BaseEntityConfiguration<AttributeType>
    {
        public override void Configure(EntityTypeBuilder<AttributeType> builder)
        {
            base.Configure(builder);
            builder.HasIndex(a => a.Name);
            builder.HasOne(b => b.Creator)
                .WithMany(u => u.AttributeTypes) // Proper back navigation
                .HasForeignKey(b => b.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }