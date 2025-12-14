using TaskManagement.Application.Abstractions;

namespace TaskManagement.Application.Commands.DeleteTask;

public sealed record DeleteTaskCommand(int TaskId): ICommand<bool>;
