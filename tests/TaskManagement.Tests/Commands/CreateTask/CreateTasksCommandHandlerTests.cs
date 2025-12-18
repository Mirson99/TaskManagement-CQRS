using FluentAssertions;
using Moq;
using TaskManagement.Application.Commands.CreateTask;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Handlers;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Application.Tests.Commands.CreateTask;

public class CreateTasksCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;

    public CreateTasksCommandHandlerTests()
    {
        _taskRepositoryMock = new();
    }
    
    [Fact]
    public async Task Handle_Should_ReturnId_WhenTaskAdded()
    {
        // Arrange
        var command = new CreateTaskCommand("TestTitle", "TestDescription", DateTime.UtcNow.AddDays(1), 0);
        var handler = new CreateTaskCommandHandler(_taskRepositoryMock.Object);
        
        // Act
        int id = await handler.Handle(command, default);
        
        // Assert
        id.Should().BeGreaterThanOrEqualTo(0);
        
        _taskRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<TaskItem>(t => 
                    t.Title == "TestTitle" &&
                    t.Description == "TestDescription" &&
                    t.IsCompleted == false &&
                    t.CreatedAt != default
                ), 
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
    
}