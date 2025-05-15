using LookGenerator.Application.Abstractions;

namespace LookGenerator.Application.Features.Looks.Delete;

public record DeleteLookCommand(Guid Id):ICommand;