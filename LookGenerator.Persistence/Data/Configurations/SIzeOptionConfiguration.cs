using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class SIzeOptionConfiguration:BaseEntityConfiguration<SizeOption>
    {
        public override void Configure(EntityTypeBuilder<SizeOption> builder)
        {
            base.Configure(builder);
            builder.Property(s => s.Cm).HasColumnType("numeric(5,2)"); 
            builder.Property(s => s.Inch).HasColumnType("numeric(5,2)"); 
        }
    }