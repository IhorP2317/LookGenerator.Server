using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class LookConfiguration : BaseEntityConfiguration<Look>
    {
        public override void Configure(EntityTypeBuilder<Look> builder)
        {
            base.Configure(builder);
            builder.HasOne(l => l.Creator)
                .WithMany()
                .HasForeignKey(l => l.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }