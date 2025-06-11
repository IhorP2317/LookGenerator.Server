using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.AttributeOption;

namespace LookGenerator.Application.Features.AttributeOptions.GetAttributeOptions;

public record GetAttributeOptionsQuery:IQuery<ICollection<AttributeOptionResponse>>;
