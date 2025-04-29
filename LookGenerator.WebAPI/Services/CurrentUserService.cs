using System.Security.Claims;
using LookGenerator.Application.Abstractions;

namespace LookGenerator.WebAPI.Services ;

    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public string? AccessTokenRaw
        {
            get
            {
                var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
                if (!string.IsNullOrEmpty(authorizationHeader) &&
                    authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    return authorizationHeader["Bearer ".Length..].Trim();
                }
                return null;
            }
        }

        public string? UserId =>
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        public string? Username =>
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

        public string? Email =>
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

        public string? UserRole =>
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
    }