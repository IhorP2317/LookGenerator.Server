using LookGenerator.Application.Common.DTOs.AttributeOption;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Common.Mappers;

public static class AttributeOptionMapper
{
    public static AttributeOptionResponse ToResponse(this AttributeOption attributeOption) =>
        new AttributeOptionResponse(attributeOption.Id, attributeOption.Name, attributeOption.AttributeTypeId);
    public static ICollection<AttributeOptionResponse> ToResponses(this ICollection<AttributeOption> attributeOptions) => attributeOptions.Select(ToResponse).ToList();
}