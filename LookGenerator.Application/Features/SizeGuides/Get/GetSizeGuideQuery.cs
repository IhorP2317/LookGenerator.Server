using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.SizeGuide;

namespace LookGenerator.Application.Features.SizeGuides.Get;

public record GetSizeGuideQuery : IQuery<SizeGuideTableResponse>
{
   public  Dictionary<SizeGuideFilterType, object> Filters { get; set; } = new();
}