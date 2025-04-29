using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs;

namespace LookGenerator.Application.Features.Looks.GenerateForItem;

public record GenerateLooksForItemCommand(Guid ProductVariationId,  Dictionary<string, double> Measurements):ICommand<ICollection<LookResponse>>;