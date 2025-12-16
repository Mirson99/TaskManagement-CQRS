using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Commands.CompleteTask;
using TaskManagement.Application.Commands.DeleteTask;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Infrastructure.Handlers;


public class DeleteTaskCommandHandler: ICommandHandler<DeleteTaskCommand, bool>
{
    private readonly ITaskRepository  _taskRepository;
    public DeleteTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<bool> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(command.TaskId, cancellationToken);
        if (task is null)
            return false;
        await _taskRepository.RemoveAsync(task, cancellationToken);
        return true;
    }
}