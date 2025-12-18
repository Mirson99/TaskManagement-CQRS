using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Commands.CompleteTask;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Infrastructure.Handlers;

public class CompleteTaskCommandHandler: ICommandHandler<CompleteTaskCommand, bool>
{
    private readonly ITaskRepository _taskRepository;
    
    public CompleteTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }
    public async Task<bool> Handle(CompleteTaskCommand command, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(command.TaskId, cancellationToken);
        if (task is null)
            return false;

        if (task.IsCompleted)
            return true;

        task.IsCompleted = true;
        await _taskRepository.UpdateAsync(task, cancellationToken);
        return true;
    }
}