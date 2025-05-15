namespace LookGenerator.Application.Common.DTOs.User;

public record UserResponse(
    Guid Id,
    DateTime CreatedAt,
    Guid? CreatedBy,
    Guid? ModifiedBy,
    DateTime? ModifiedAt,
    string UserName,
    string Email,
    string Role,
    bool EmailConfirmed) : BaseEntityResponse(Id, CreatedAt, CreatedBy, ModifiedBy, ModifiedAt);