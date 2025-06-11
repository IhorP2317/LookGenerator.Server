using LookGenerator.Application.Common.DTOs.SizeOption;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Mappers;

public static class SizeOptionMapper
{
    public static ICollection<SizeOptionResponse> ToResponse(this ICollection<SizeOption> sizeOptions, string size)
    {
        return sizeOptions.Select(x => x.ToResponse(size)).ToList();
    }
    public static SizeOptionResponse ToResponse(this SizeOption sizeOption, string size) =>
        new SizeOptionResponse(sizeOption.Id, size, sizeOption.Cm, sizeOption.Inch);
}