using FluentAssertions;
using Moq;
using TaskManagement.Application.Commands.CompleteTask;
using TaskManagement.Application.Commands.DeleteTask;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Handlers;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Application.Tests.Commands.DeleteTask;

public class DeleteTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;

    public DeleteTaskCommandHandlerTests()
    {
        _taskRepositoryMock = new();
    }
    
    [Fact]
    public async Task Handle_Should_ReturnFalse_WhenTaskDoesntExist()
    {
        // Arrange
        var command = new DeleteTaskCommand(99);
        var handler = new DeleteTaskCommandHandler(_taskRepositoryMock.Object);
        
        // Act
        var result = await handler.Handle(command, default);
        
        // Assert
        result.Should().BeFalse();
        _taskRepositoryMock.Verify(
            x => x.RemoveAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), 
            Times.Never
        );
    }
    [Fact]
        public async Task Handle_Should_ReturnTrue_WhenTaskExist()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = 1,
                Title = "Uncompleted",
                IsCompleted = false,
            };
            
            _taskRepositoryMock
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);
            
            var command = new DeleteTaskCommand(1);
            var handler = new  DeleteTaskCommandHandler(_taskRepositoryMock.Object);
            
            //Act
            var result = await handler.Handle(command, default);
            
            // Assert
            result.Should().BeTrue();
            _taskRepositoryMock.Verify(
                x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), 
                Times.Once
            );
            _taskRepositoryMock.Verify(
                x => x.RemoveAsync(
                    It.Is<TaskItem>(t => t.Id == 1),
                    It.IsAny<CancellationToken>()
                ), 
                Times.Once
            );
        }
    
}