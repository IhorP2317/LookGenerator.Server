using LookGenerator.Application.Abstractions;
using LookGenerator.Persistence.Data;
using LookGenerator.Persistence.Data.Interceptors;
using LookGenerator.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LookGenerator.Persistence ;

    public static class ServiceExtensions
    {
        public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .Configure<AdminSettings>(configuration.GetSection("AdminSettings"))
                .AddScoped<AuditingSaveChangesInterceptor>();
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var auditingSaveChangesInterceptor = serviceProvider.GetRequiredService<AuditingSaveChangesInterceptor>();
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                    .AddInterceptors(auditingSaveChangesInterceptor)
                    .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
            }).AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        }
    }