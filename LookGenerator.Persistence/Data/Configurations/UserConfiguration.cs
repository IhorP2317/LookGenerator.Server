using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class UserConfiguration:BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.HasMany(u => u.Looks)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }