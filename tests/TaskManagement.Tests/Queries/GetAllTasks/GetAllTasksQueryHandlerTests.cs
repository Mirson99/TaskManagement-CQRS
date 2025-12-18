using FluentAssertions;
using Moq;
using TaskManagement.Application.Queries.GetAllTasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Handlers.Queries;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Application.Tests.Queries.GetAllTasks;

public class GetAllTasksQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;

    public GetAllTasksQueryHandlerTests()
    {
        _taskRepositoryMock = new();
    }
    
    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenThereAreNoTasks()
    {
        // Arrange
        _taskRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem>()); 
        var query = new GetAllTasksQuery();
        var handler = new GetAllTasksQueryHandler(_taskRepositoryMock.Object);
        
        // Act
        var result = await handler.Handle(query, default);
        
        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task Handle_Should_ReturnList_WhenThereAreTasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new() { Id = 1, Title = "Task 1", IsCompleted = false },
            new() { Id = 2, Title = "Task 2", IsCompleted = true },
            new() { Id = 3, Title = "Task 3", IsCompleted = false }
        };
    
        _taskRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);
    
        var query = new GetAllTasksQuery();
        var handler = new GetAllTasksQueryHandler(_taskRepositoryMock.Object);
    
        // Act
        var result = await handler.Handle(query, default);
    
        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Title));
        result.Should().Contain(dto => dto.Id == 1);
        result[0].Id.Should().Be(1);
        result[0].Title.Should().Be("Task 1");
        result[0].IsCompleted.Should().BeFalse();
    }
    
    [Fact]
    public async Task Handle_Should_MapToDto_Correctly()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "Test Task",
            Description = "Long description",
            IsCompleted = true,
            CreatedAt = new DateTime(2025, 12, 1)
        };
    
        _taskRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem> { task });
    
        var query = new GetAllTasksQuery();
        var handler = new GetAllTasksQueryHandler(_taskRepositoryMock.Object);
    
        // Act
        var result = await handler.Handle(query, default);
    
        // Assert
        var dto = result.First();
        dto.Id.Should().Be(1);
        dto.Title.Should().Be("Test Task");
        dto.IsCompleted.Should().BeTrue();
        dto.CreatedAt.Should().Be(new DateTime(2025, 12, 1));
    }
}