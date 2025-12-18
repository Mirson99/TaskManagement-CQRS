using FluentAssertions;
using FluentValidation;
using Moq;
using TaskManagement.Application.Commands.CompleteTask;
using TaskManagement.Application.Commands.CreateTask;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Handlers;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Application.Tests.Commands.CompleteTask;

public class CompleteTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;

    public CompleteTaskCommandHandlerTests()
    {
        _taskRepositoryMock = new();
    }
    
    [Fact]
    public async Task Handle_Should_ReturnFalse_WhenTaskDoesntExist()
    {
        // Arrange
        var command = new CompleteTaskCommand(99);
        var handler = new CompleteTaskCommandHandler(_taskRepositoryMock.Object);
        
        // Act
        var result = await handler.Handle(command, default);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task Handle_Should_ReturnTrue_WhenTaskAlreadyCompleted()
    {
        // Arrange
        var completedTask = new TaskItem
        {
            Id = 1,
            Title = "Already completed task",
            IsCompleted = true,
        };
        
        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(completedTask);
        
        var command = new CompleteTaskCommand(1);
        var handler = new  CompleteTaskCommandHandler(_taskRepositoryMock.Object);
        
        //Act
        var result = await handler.Handle(command, default);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_Should_ReturnTrue_WhenCompleteTask()
    {
        // Arrange
        var unCompletedTask = new TaskItem
        {
            Id = 1,
            Title = "Uncompleted",
            IsCompleted = false,
        };
        
        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(unCompletedTask);
        
        var command = new CompleteTaskCommand(1);
        var handler = new  CompleteTaskCommandHandler(_taskRepositoryMock.Object);
        
        //Act
        var result = await handler.Handle(command, default);
        
        // Assert
        result.Should().BeTrue();
        unCompletedTask.IsCompleted.Should().BeTrue();
        _taskRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), 
            Times.Once
        );
        _taskRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
    
    [Fact]
    public async Task Handle_Should_ReturnError_WhenIdSmallerThanOne()
    {
        
        var command = new CompleteTaskCommand(0);
        var handler = new  CompleteTaskCommandHandler(_taskRepositoryMock.Object);
        
        //Act
        var result = await handler.Handle(command, default);
        
        // Assert
        
        _taskRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), 
            Times.Never
        );
        _taskRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}