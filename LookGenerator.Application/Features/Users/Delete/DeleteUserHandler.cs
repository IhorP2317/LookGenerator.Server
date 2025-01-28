using System.Net.Http.Headers;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.Exceptions;
using LookGenerator.Application.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LookGenerator.Application.Features.Users.Delete ;

    public class DeleteUserHandler(
        IApplicationDbContext dbContext, IOptions<IdentityHttpClientSettings> identityHttpClientConstants,
        ICurrentUserService currentUserService, IHttpClientFactory httpClientFactory) :
            ICommandHandler<DeleteUserCommand>
    {
        private readonly HttpClient _httpClient =
            httpClientFactory.CreateClient(identityHttpClientConstants.Value.ClientName);

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                currentUserService.AccessTokenRaw);
            var url = $"{_httpClient.BaseAddress}/{request.UserId}";
            var httpResponse = await _httpClient.DeleteAsync(url, cancellationToken);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                throw new CustomHttpRequestException(httpResponse.StatusCode,
                    $"Failed to create user. HTTP Status: {httpResponse.StatusCode}, Response: {errorContent}");
            }
            var userToDelete = await dbContext.Users.FirstAsync(u => u.Id == request.UserId, cancellationToken);
            dbContext.Users.Remove(userToDelete);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }