using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Features.Looks.ChangeStatus;

public record ChangeLookStatusCommand(Guid Id, LookStatus Status):ICommand;
