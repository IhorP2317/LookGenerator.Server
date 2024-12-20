using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class SizeCategoryConfiguration:BaseEntityConfiguration<SizeCategory>
    {
        public override void Configure(EntityTypeBuilder<SizeCategory> builder)
        {
            base.Configure(builder);
            builder.HasIndex(s => s.Name);
            builder.HasOne(s => s.ParentCategory)
                .WithMany(s => s.SubCategories)
                .HasForeignKey(s => s.ParentCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(sc => sc.SizeOptions)
                .WithOne(so => so.SizeCategory)
                .HasForeignKey(so => so.SizeCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }