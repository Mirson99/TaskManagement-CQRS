using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Queries.GetTaskById;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Handlers.Queries;

public class GetTaskByIdQueryHandler: IQueryHandler<GetTaskByIdQuery, TaskDetailsDto>
{
    private TaskDbContext _taskDbContext;
    
    public GetTaskByIdQueryHandler(TaskDbContext taskDbContext)
    {
        _taskDbContext = taskDbContext;
    }

    public async Task<TaskDetailsDto> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        return await _taskDbContext.Tasks
            .Where(t => t.Id == query.TaskId)
            .Select(t => new TaskDetailsDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                CreatedAt = t.CreatedAt,
            }).FirstOrDefaultAsync();
    }
}