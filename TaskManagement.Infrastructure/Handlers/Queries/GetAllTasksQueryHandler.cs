using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Queries.GetAllTasks;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Handlers.Queries;

public class GetAllTasksQueryHandler: IQueryHandler<GetAllTasksQuery, List<TaskDto>>
{
    private readonly TaskDbContext _taskDbContext;
    
    public GetAllTasksQueryHandler(TaskDbContext taskDbContext)
    {
        _taskDbContext = taskDbContext;
    }
    public async Task<List<TaskDto>> Handle(GetAllTasksQuery query, CancellationToken cancellationToken)
    {
        return await _taskDbContext.Tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            IsCompleted = t.IsCompleted,
            CreatedAt = t.CreatedAt   
        }).ToListAsync(cancellationToken);
        
        
    }
}