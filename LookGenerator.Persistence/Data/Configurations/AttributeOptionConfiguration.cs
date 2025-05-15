using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class AttributeOptionConfiguration:BaseEntityConfiguration<AttributeOption>
    {
        public override void Configure(EntityTypeBuilder<AttributeOption> builder)
        {
            base.Configure(builder);
            builder.HasIndex(a => a.Name);
            builder.HasOne(a => a.AttributeType)
                .WithMany(a => a.AttributeOptions)
                .HasForeignKey(a => a.AttributeTypeId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(b => b.Creator)
                .WithMany(u => u.AttributeOptions) 
                .HasForeignKey(b => b.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }