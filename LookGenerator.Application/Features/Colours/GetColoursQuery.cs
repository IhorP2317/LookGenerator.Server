using LookGenerator.Application.Abstractions;

namespace LookGenerator.Application.Features.Colours;

public record GetColoursQuery:IQuery<ICollection<string>>;
