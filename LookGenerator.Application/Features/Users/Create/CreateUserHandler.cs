
using System.Net.Http.Json;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.Exceptions;
using LookGenerator.Application.Settings;
using LookGenerator.Domain.Entities;
using Microsoft.Extensions.Options;

namespace LookGenerator.Application.Features.Users.Create ;

    public class CreateUserHandler(
        IApplicationDbContext dbContext, IOptions<IdentityHttpClientSettings> identityHttpClientConstants,
        IHttpClientFactory httpClientFactory) : ICommandHandler<CreateUserCommand>
    {
        private readonly HttpClient _httpClient =
            httpClientFactory.CreateClient(identityHttpClientConstants.Value.ClientName);

        public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var url = $"{_httpClient.BaseAddress}{identityHttpClientConstants.Value.RegisterEndpoint}";
            var httpResponse = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                throw new CustomHttpRequestException(httpResponse.StatusCode,
                    $"Failed to create user. HTTP Status: {httpResponse.StatusCode}, Response: {errorContent}");
            }

            var registerResponse =
                await httpResponse.Content.ReadFromJsonAsync<User>(cancellationToken: cancellationToken);
            if (registerResponse == null)
            {
                throw new InternalServerException("Failed to deserialize registered user.");
            }

            var user = new User
            {
                Id = registerResponse.Id,
                UserName = registerResponse.UserName,
                Email = registerResponse.Email,
                Role = registerResponse.Role
            };
            await dbContext.Users.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }