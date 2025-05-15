using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Features.Reactions.Delete;

public record DeleteReactionCommand(Guid LookId, ReactionType ReactionType):ICommand;