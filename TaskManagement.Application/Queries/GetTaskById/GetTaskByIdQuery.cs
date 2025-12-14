using TaskManagement.Application.Abstractions;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Queries.GetTaskById;

public sealed record GetTaskByIdQuery(int TaskId):  IQuery<TaskDetailsDto>;
