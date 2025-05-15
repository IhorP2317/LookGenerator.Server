using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.Look;
using LookGenerator.Application.Common.Helpers;

namespace LookGenerator.Application.Features.Looks.GetPaged;

public record GetLooksQuery : IQuery<PagedList<FeedLookResponse>>
{
    public Dictionary<LookFilterType, object> Filters { get; set; } = new();
}

