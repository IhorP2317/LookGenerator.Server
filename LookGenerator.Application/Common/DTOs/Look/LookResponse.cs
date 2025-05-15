using LookGenerator.Application.Common.DTOs.Product;
using LookGenerator.Application.Common.DTOs.User;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.DTOs.Look;

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
    Guid? ModifiedBy,
    LookUserResponse? Creator,
    int LikeCount = 0,
    int PinCount = 0,
    bool IsLiked = false,
    bool IsPinned = false,
    decimal TotalPrice = 0);