using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities;

public class Reaction:BaseEntity
{
    public Guid LookId { get; set; }
    public Look Look { get; set; } = default!;
    public ReactionType Type { get; set; }
}