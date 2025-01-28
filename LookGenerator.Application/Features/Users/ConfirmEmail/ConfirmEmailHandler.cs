using System.Net.Http.Json;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.Exceptions;
using LookGenerator.Application.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LookGenerator.Application.Features.Users.ConfirmEmail ;

    public class ConfirmEmailHandler(IApplicationDbContext dbContext, IOptions<IdentityHttpClientSettings> identityHttpClientConstants,
        IHttpClientFactory httpClientFactory) : ICommandHandler<ConfirmEmailCommand>
    {
        private readonly HttpClient _httpClient =
            httpClientFactory.CreateClient(identityHttpClientConstants.Value.ClientName);

        public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var url =
                $"{_httpClient.BaseAddress}{identityHttpClientConstants.Value.ConfirmEmailEndpoint}?Email={request.Email}&ConfirmationToken={request
                    .Token}";
            var httpResponse = await _httpClient.PostAsync(url, null, cancellationToken: cancellationToken);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                throw new CustomHttpRequestException(httpResponse.StatusCode,
                    $"Failed to confirm email. HTTP Status: {httpResponse.StatusCode}, Response: {errorContent}");
            }
            var user = await dbContext.Users.FirstAsync(u => u.Email == request.Email, cancellationToken);
            user.EmailConfirmed = true;
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }