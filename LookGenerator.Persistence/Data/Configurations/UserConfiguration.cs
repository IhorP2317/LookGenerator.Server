using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.UserName).IsUnique();
        
        builder.HasOne(b => b.Creator)
            .WithMany(u => u.Users) 
            .HasForeignKey(b => b.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
