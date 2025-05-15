using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.DTOs;
using LookGenerator.Application.Common.DTOs.Look;

namespace LookGenerator.Application.Features.Looks.Generate;

public record GenerateLooksCommand(
    string Gender,
    ICollection<Guid> AttributeOptionIds,
    List<string> Colours,
    Dictionary<string, double> Measurements) : ICommand<ICollection<LookResponse>>;