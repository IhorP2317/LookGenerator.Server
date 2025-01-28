using System.Text;
using Carter;
using LookGenerator.Application.Abstractions;
using LookGenerator.WebAPI.ExceptionHandlers;
using LookGenerator.WebAPI.Services;
using LookGenerator.WebAPI.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LookGenerator.WebAPI ;

    public static class ServiceExtensions
    {
        public static void ConfigureWebApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthSettings>(configuration.GetSection("AuthSettings"));
            var authSettings = configuration
                .GetSection("AuthSettings")
                .Get<AuthSettings>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authSettings!.Issuer,
                        ValidateAudience = true,
                        ValidAudience = authSettings.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = authSettings.SymmetricSecurityKey,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            services.AddProblemDetails()
                .AddExceptionHandler<CustomExceptionHandler>()
                .AddOpenApi()
                .AddHttpContextAccessor()
                .AddCarter(configurator: c =>
                {
                    c.WithValidatorLifetime(ServiceLifetime.Scoped);
                })
                .AddAuthorization()
                .AddScoped<ICurrentUserService, CurrentUserService>()
                .AddCors(options =>
                {
                    options.AddPolicy("AllowAny", builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
                });
        }
    }