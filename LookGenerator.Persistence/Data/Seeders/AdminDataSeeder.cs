using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;
using LookGenerator.Persistence.Settings;
using Microsoft.Extensions.Options;

namespace LookGenerator.Persistence.Data.Seeders;

public class AdminDataSeeder(IApplicationDbContext applicationDbContext, IOptions<AdminSettings> adminSettings)
    : IDataSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existingUser = await applicationDbContext.Users
            .FindAsync([adminSettings.Value.Id], cancellationToken);
        if (existingUser != null) return;

        var newUser = RetrieveUserFromSettings();
        await applicationDbContext.Users.AddAsync(newUser, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    public void Seed()
    {
        SeedAsync().GetAwaiter().GetResult();
    }

    private User RetrieveUserFromSettings() => new()
    {
        Id = adminSettings.Value.Id,
        Email = adminSettings.Value.Email,
        UserName = adminSettings.Value.UserName,
        Role = adminSettings.Value.Role,
        EmailConfirmed = true,
        CreatedAt = DateTime.UtcNow
    };
}
