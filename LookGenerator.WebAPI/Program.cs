using Carter;
using LookGenerator.Application;
using LookGenerator.Infrastructure.Extensions;
using LookGenerator.Persistence.Extensions;
using LookGenerator.WebAPI;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

    builder.Configuration.AddUserSecrets<Program>(optional:true, reloadOnChange:true)
        .AddEnvironmentVariables();
    builder.Services.ConfigureWebApi(builder.Configuration);
    builder.Services.ConfigureApplication(builder.Configuration);
    builder.Services.ConfigurePersistence(builder.Configuration);
    builder.Services.ConfigureInfrastructure(builder.Configuration);
    var app = builder.Build();


// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        await app.ApplyMigrationsAsync();
    }

    app.UseCors("AllowAny");
    app.UseExceptionHandler()
        .UseHttpsRedirection()
        .UseAuthentication()
        .UseAuthorization();
    app.MapCarter();

    app.Run();