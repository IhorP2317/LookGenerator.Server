using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Features.Looks.Update;

public record UpdateLookCommand(Guid Id, string Name, string? Description,  string ColorPalette, LookStatus Status, ICollection<Guid> ProductVariationIds):ICommand;