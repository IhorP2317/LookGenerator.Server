using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations;

public class ColourConfiguration : IEntityTypeConfiguration<Colour>
{
    public void Configure(EntityTypeBuilder<Colour> builder)
    {
        builder.HasKey(colour => colour.Id);
        builder.HasMany(c => c.ProductItems).WithOne(p => p.Colour).HasForeignKey(p => p.ColourId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}