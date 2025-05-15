using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Features.Reactions.Create;

public record CreateReactionCommand(Guid LookId, ReactionType ReactionType):ICommand;