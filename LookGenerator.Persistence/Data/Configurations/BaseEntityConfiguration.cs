using LookGenerator.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
            
        }
    }