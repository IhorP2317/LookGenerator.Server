namespace LookGenerator.Application.Common.DTOs.User;

public record LookUserResponse(Guid Id, string UserName, string Email, string Role);