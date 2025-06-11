namespace LookGenerator.Application.Common.DTOs.SizeGuide;

public record SizeGuideTableResponse(List<string> Sizes, List<SizeGuideRow> Rows);