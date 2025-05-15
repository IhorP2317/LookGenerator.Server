using System.Net.Http.Headers;
using LookGenerator.Application.Abstractions;
using LookGenerator.Infrastructure.Jobs.Definitions;
using LookGenerator.Infrastructure.Jobs.Setups;
using LookGenerator.Infrastructure.Services;
using LookGenerator.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using Quartz;

namespace LookGenerator.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("NoBaseUriClient", client =>
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
        }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true;
            return handler;
        });

        services.ConfigureOptions<BershkaColorNormalizationJobSetup>();
        services.ConfigureOptions<BershkaSizeGuideParsingJobSetup>();
        //services.ConfigureOptions<BershkaLookParsingJobSetup>();
        services.AddQuartz();
        services.Configure<OpenAiSettings>(configuration.GetSection("OpenAi"));


        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        services.AddScoped(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<OpenAiSettings>>().Value;
            return new OpenAIClient(settings.ApiKey);
        });
        services.AddScoped<IClient, OpenAiClient>();
    }
}