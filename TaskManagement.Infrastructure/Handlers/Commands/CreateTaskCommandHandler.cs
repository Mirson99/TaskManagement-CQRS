using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Commands.CreateTask;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;


namespace TaskManagement.Infrastructure.Handlers;

public class CreateTaskCommandHandler: ICommandHandler<CreateTaskCommand, int>
{
    private readonly ITaskRepository _taskRepository;
    public CreateTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
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
            
            await _taskRepository.AddAsync(task, cancellationToken);
            
            return task.Id;
        }
}