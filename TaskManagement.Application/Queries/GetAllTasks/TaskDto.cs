namespace TaskManagement.Application.Queries.GetAllTasks;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public string Status => IsCompleted ? "Done" : "Pending";
}