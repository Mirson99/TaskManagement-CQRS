namespace TaskManagement.Application.Queries.GetTaskById;

public class TaskDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DueAt { get; set; }
    
    public string Status => IsCompleted ? "Completed" : "In Progress";
}