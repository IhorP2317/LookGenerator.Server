using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs.Look;

namespace LookGenerator.Application.Features.Looks.GetOneDetail;

public record GetLookQuery(Guid Id):IQuery<LookResponse>;