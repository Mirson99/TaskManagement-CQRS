using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Commands.CompleteTask;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Handlers;

public class CompleteTaskCommandHandler: ICommandHandler<CompleteTaskCommand, bool>
{
    private readonly TaskDbContext _taskDbContext;
    
    public CompleteTaskCommandHandler(TaskDbContext taskDbContext)
    {
        _taskDbContext = taskDbContext;
    }
    public async Task<bool> Handle(CompleteTaskCommand command, CancellationToken cancellationToken)
    {
        var task = await _taskDbContext.Tasks.FindAsync(command.TaskId, cancellationToken);
        if (task is null)
            return false;

        if (task.IsCompleted)
            return true;

        task.IsCompleted = true;
        await _taskDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}