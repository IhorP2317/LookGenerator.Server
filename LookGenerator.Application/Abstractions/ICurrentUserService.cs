namespace LookGenerator.Application.Abstractions ;

    public interface ICurrentUserService
    {
        string? AccessTokenRaw { get; }
        string? UserId { get; }
        string? Username { get; }
        string? Email { get; }
        string? UserRole { get; }
    }