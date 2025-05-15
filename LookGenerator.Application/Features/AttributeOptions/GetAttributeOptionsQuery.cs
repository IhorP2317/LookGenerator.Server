using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.AttributeOption;

namespace LookGenerator.Application.Features.AttributeOptions;

public record GetAttributeOptionsQuery:IQuery<ICollection<AttributeOptionResponse>>;
