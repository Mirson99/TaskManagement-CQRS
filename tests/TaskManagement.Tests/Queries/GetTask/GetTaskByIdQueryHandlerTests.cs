using FluentAssertions;
using Moq;
using TaskManagement.Application.Queries.GetTaskById;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Handlers.Queries;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Application.Tests.Queries.GetTask;

public class GetTaskByIdQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;

    public GetTaskByIdQueryHandlerTests()
    {
        _taskRepositoryMock = new();
    }
    
    [Fact]
    public async Task Handle_Should_ReturnNull_WhenTaskDoesntExist()
    {
        // Arrange
        var command = new GetTaskByIdQuery(99);
        var handler = new GetTaskByIdQueryHandler(_taskRepositoryMock.Object);
        
        // Act
        var result = await handler.Handle(command, default);
        
        // Assert
        result.Should().Be(null);
    }
    
    [Fact]
    public async Task Handle_Should_ReturnTask_WhenTaskExist()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "Test Task",
            Description = "Test Description",
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };
    
        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
    
        var query = new GetTaskByIdQuery(1);
        var handler = new GetTaskByIdQueryHandler(_taskRepositoryMock.Object);
    
        // Act
        var result = await handler.Handle(query, default);
    
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Test Task");
        result.Description.Should().Be("Test Description");
        result.IsCompleted.Should().BeFalse();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        
        _taskRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}