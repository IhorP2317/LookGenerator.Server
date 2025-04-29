using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.DTOs;

public record LookResponse(
    Guid Id,
    string Name,
    string? Description,
    string ColorPalette,
    LookStatus LookStatus,
    ICollection<LookProductResponse> Products,
    DateTime CreatedAt,
    Guid? CreatedBy,
    DateTime? ModifiedAt,
    Guid? ModifiedBy);