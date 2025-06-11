using LookGenerator.Application.Common.DTOs.SizeOption;

namespace LookGenerator.Application.Common.DTOs.SizeGuide;

public record SizeGuideRow(string Parameter)
{
  public  List<SizeOptionResponse> Values { get; set; } = [];
}