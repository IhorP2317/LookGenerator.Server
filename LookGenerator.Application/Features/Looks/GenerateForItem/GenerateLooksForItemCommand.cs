using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs;
using LookGenerator.Application.Common.DTOs.Look;

namespace LookGenerator.Application.Features.Looks.GenerateForItem;

public record GenerateLooksForItemCommand(Guid ProductVariationId,  Dictionary<string, double> Measurements):ICommand<ICollection<LookResponse>>;