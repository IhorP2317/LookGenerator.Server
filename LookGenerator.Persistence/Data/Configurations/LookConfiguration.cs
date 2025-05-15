using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations;

public class LookConfiguration : BaseEntityConfiguration<Look>
{
    public override void Configure(EntityTypeBuilder<Look> builder)
    {
        base.Configure(builder);
        builder.HasIndex(l => l.Name);
        builder.HasIndex(l => l.Description);
        builder.HasOne(b => b.Creator)
            .WithMany(u => u.Looks)
            .HasForeignKey(b => b.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}