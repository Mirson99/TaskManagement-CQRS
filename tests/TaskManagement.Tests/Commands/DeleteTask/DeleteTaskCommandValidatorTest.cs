using FluentValidation.TestHelper;
using TaskManagement.Application.Commands.CompleteTask;
using TaskManagement.Application.Commands.DeleteTask;

namespace TaskManagement.Application.Tests.Commands.DeleteTask;

public class DeleteTaskCommandValidatorTests
{
    private readonly DeleteTaskCommandValidator _validator;
    
    public DeleteTaskCommandValidatorTests()
    {
        _validator = new DeleteTaskCommandValidator();
    }
    
    [Fact]
    public void Validate_Should_HaveError_WhenTaskIdIsZero()
    {
        // Arrange
        var command = new DeleteTaskCommand(0);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TaskId)
            .WithErrorMessage("Task ID must be greater than 0");
    }
    
    [Fact]
    public void Validate_Should_HaveError_WhenTaskIdIsNegative()
    {
        // Arrange
        var command = new DeleteTaskCommand(-1);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TaskId)
            .WithErrorMessage("Task ID must be greater than 0");
    }
    
    [Fact]
    public void Validate_Should_NotHaveError_WhenTaskIdIsValid()
    {
        // Arrange
        var command = new DeleteTaskCommand(1);
    
        // Act
        var result = _validator.TestValidate(command);
    
        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TaskId);
    }
}