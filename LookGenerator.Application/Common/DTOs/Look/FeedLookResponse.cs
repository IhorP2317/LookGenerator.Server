using LookGenerator.Application.Common.DTOs.User;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.DTOs.Look;

public record FeedLookResponse(
    Guid Id,
    string Name,
    string? Description,
    string ColorPalette,
    LookStatus LookStatus,
    List<string?> ProductImageUrls,
    int LikeCount,
    int PinCount,
    bool IsLiked,
    bool IsPinned,
    LookUserResponse? Creator);