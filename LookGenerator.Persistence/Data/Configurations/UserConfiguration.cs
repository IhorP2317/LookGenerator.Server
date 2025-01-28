using LookGenerator.Domain.Entities;
using LookGenerator.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations ;

    public class UserConfiguration(AdminSettings adminSettings):BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.HasMany(u => u.Looks)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);
            var user = new User
            {
                Id = adminSettings.Id,
                Email = adminSettings.Email,
                UserName = adminSettings.UserName,
                Role = adminSettings.Role,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = null,
                ModifiedAt = null,
                ModifiedBy = null
            };
            builder.HasData(user);
        }
    }