using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations;

public class ReactionConfiguration:BaseEntityConfiguration<Reaction>
{
    public override void Configure(EntityTypeBuilder<Reaction> builder)
    {
        base.Configure(builder);
        builder.HasOne(r => r.Look)
            .WithMany(l => l.Reactions)
            .HasForeignKey(r => r.LookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Creator)
            .WithMany(u => u.Reactions)
            .HasForeignKey(r => r.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasIndex(r => r.Type);
    }
}