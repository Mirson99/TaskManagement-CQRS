using TaskManagement.Application.Abstractions;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Queries.GetAllTasks;

public sealed record GetAllTasksQuery():  IQuery<List<TaskDto>>;