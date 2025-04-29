using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Features.Looks.Generate;

public record GenerateLooksResponse(ICollection<Look> Looks);