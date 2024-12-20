using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class AttributeTypeConfiguration:BaseEntityConfiguration<AttributeType>
    {
        public override void Configure(EntityTypeBuilder<AttributeType> builder)
        {
            base.Configure(builder);
            builder.HasIndex(a => a.Name);
        }
    }