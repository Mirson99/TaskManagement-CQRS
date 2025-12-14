using TaskManagement.Application.Abstractions;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Commands.CompleteTask;

public sealed record CompleteTaskCommand(int TaskId): ICommand<bool>;