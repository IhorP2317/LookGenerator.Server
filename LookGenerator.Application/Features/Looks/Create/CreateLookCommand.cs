using LookGenerator.Application.Abstractions;

namespace LookGenerator.Application.Features.Looks.Create;

public record CreateLookCommand( string Name, string? Description,  string ColorPalette,  ICollection<Guid> ProductVariationIds):ICommand<Guid>;
