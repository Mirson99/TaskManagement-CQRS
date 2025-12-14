using TaskManagement.Application.Abstractions;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Commands.CreateTask;

public sealed record CreateTaskCommand(string Title, string Description, DateTime DueAt, TaskPriority Priority): ICommand<int>;