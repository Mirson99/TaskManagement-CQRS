using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Commands.CreateTask;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;


namespace TaskManagement.Infrastructure.Handlers;

public class CreateTaskCommandHandler: ICommandHandler<CreateTaskCommand, int>
{
    private readonly TaskDbContext _taskDbContext;
    
    public CreateTaskCommandHandler(TaskDbContext taskDbContext)
    {
        _taskDbContext = taskDbContext;
    }
    
    public async Task<int> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        var task = new TaskItem
        {
            Title = command.Title,
            Description = command.Description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };
        
        _taskDbContext.Tasks.Add(task);
        await _taskDbContext.SaveChangesAsync(cancellationToken);
        
        return task.Id;
    }
}