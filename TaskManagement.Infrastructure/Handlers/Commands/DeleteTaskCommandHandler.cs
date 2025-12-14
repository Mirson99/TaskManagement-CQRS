using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Commands.CompleteTask;
using TaskManagement.Application.Commands.DeleteTask;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Handlers;


public class DeleteTaskCommandHandler: ICommandHandler<DeleteTaskCommand, bool>
{
    private readonly TaskDbContext _taskDbContext;
    
    public DeleteTaskCommandHandler(TaskDbContext taskDbContext)
    {
        _taskDbContext = taskDbContext;
    }

    public async Task<bool> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        var task = await _taskDbContext.Tasks.FindAsync(command.TaskId, cancellationToken);
        if (task is null)
            return false;
        _taskDbContext.Tasks.Remove(task);
        await _taskDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}