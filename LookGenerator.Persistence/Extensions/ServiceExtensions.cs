using LookGenerator.Application.Abstractions;
using LookGenerator.Persistence.Data;
using LookGenerator.Persistence.Data.Interceptors;
using LookGenerator.Persistence.Data.Seeders;
using LookGenerator.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LookGenerator.Persistence.Extensions;

public static class ServiceExtensions
{
    public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<AdminSettings>(configuration.GetSection("AdminSettings"))
            .AddScoped<AuditingSaveChangesInterceptor>()
            .AddScoped<IDataSeeder,AdminDataSeeder>()
            .AddScoped<IDataSeeder,AttributeDataSeeder>();
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var auditingSaveChangesInterceptor = serviceProvider.GetRequiredService<AuditingSaveChangesInterceptor>();
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    })
                .AddInterceptors(auditingSaveChangesInterceptor)
                .UseSeeding((_,_) =>
                {
                    var seeders = serviceProvider.GetServices<IDataSeeder>();
                    foreach (var seeder in seeders)
                    {
                        seeder.Seed();
                    }
                })
                .UseAsyncSeeding(async (_, _, cancellationToken) =>
                {
                    var seeders = serviceProvider.GetServices<IDataSeeder>();
                    foreach (var seeder in seeders)
                    {
                        await seeder.SeedAsync(cancellationToken);
                    }
                })
                .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
        }).AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }
}