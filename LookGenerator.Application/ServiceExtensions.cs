using System.Net.Http.Headers;
using System.Reflection;
using FluentValidation;
using LookGenerator.Application.Common.Behaviors;
using LookGenerator.Application.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LookGenerator.Application ;

    public static class ServiceExtensions
    {
        public static void ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IdentityHttpClientSettings>(configuration.GetSection("IdentityHttpClient"));
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.Lifetime = ServiceLifetime.Scoped;
            });
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            var identityHttpClientSettings = configuration
                .GetSection("IdentityHttpClient")
                .Get<IdentityHttpClientSettings>();
            services.AddHttpClient(identityHttpClientSettings!.ClientName, client =>
            {
                client.BaseAddress = new Uri(identityHttpClientSettings.BaseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/problem+json"));
                client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) => true;
                return handler;
            });
        }
    }