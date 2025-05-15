namespace LookGenerator.Application.Common.DTOs;

public abstract record BaseEntityResponse(
    Guid Id,
    DateTime CreatedAt,
    Guid? CreatedBy,
    Guid? ModifiedBy,
    DateTime? ModifiedAt);