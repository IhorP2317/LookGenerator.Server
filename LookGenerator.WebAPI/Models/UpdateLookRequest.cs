using LookGenerator.Domain.Entities;

namespace LookGenerator.WebAPI.Models;

public record UpdateLookRequest(string Name, string? Description,  string ColorPalette, LookStatus Status, ICollection<Guid> ProductVariationIds);