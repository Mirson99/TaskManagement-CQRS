using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Commands.CompleteTask;
using TaskManagement.Application.Commands.CreateTask;
using TaskManagement.Application.Commands.DeleteTask;
using TaskManagement.Application.Queries.GetAllTasks;
using TaskManagement.Application.Queries.GetTaskById;
using TaskManagement.Domain.Enums;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    
    public TasksController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllTasksQuery();
        
        var tasks = await _queryDispatcher.Dispatch<GetAllTasksQuery, List<TaskDto>>(
            query,
            HttpContext.RequestAborted
        );
        
        return Ok(tasks);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        var query = new GetTaskByIdQuery(id);
        
        var task = await _queryDispatcher.Dispatch<GetTaskByIdQuery, TaskDetailsDto?>(
            query,
            HttpContext.RequestAborted
        );
        
        if (task == null)
            return NotFound(new { message = $"Task with ID {id} not found" });
        
        return Ok(task);
    }
    
    [HttpPost] 
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.DueDate,
            request.Priority);
        
        var taskId = await _commandDispatcher.Dispatch<CreateTaskCommand, int>(
            command,
            HttpContext.RequestAborted
        );
        
        return CreatedAtAction(
            nameof(GetTaskById),
            new { id = taskId },
            new { id = taskId }
        );
    }
    
    [HttpPut("{id}/complete")]
    public async Task<IActionResult> CompleteTask(int id)
    {
        var command = new CompleteTaskCommand(id);
        
        var success = await _commandDispatcher.Dispatch<CompleteTaskCommand, bool>(
            command,
            HttpContext.RequestAborted
        );
        
        if (!success)
            return NotFound(new { message = $"Task with ID {id} not found" });
        
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var command = new DeleteTaskCommand(id);
        
        var success = await _commandDispatcher.Dispatch<DeleteTaskCommand, bool>(
            command,
            HttpContext.RequestAborted
        );
        
        if (!success)
            return NotFound(new { message = $"Task with ID {id} not found" });
        
        return NoContent();
    }
}
public class CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public TaskPriority Priority { get; set; }
}