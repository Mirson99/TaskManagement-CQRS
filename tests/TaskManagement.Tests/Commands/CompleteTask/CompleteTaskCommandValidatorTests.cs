using FluentValidation.TestHelper;
using TaskManagement.Application.Commands.CompleteTask;

namespace TaskManagement.Application.Tests.Commands.CompleteTask;

public class CompleteTaskCommandValidatorTests
{
    private readonly CompleteTaskCommandValidator _validator;
    
    public CompleteTaskCommandValidatorTests()
    {
        _validator = new CompleteTaskCommandValidator();
    }
    
    [Fact]
    public void Validate_Should_HaveError_WhenTaskIdIsZero()
    {
        // Arrange
        var command = new CompleteTaskCommand(0);
        
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
        var command = new CompleteTaskCommand(-1);
        
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
        var command = new CompleteTaskCommand(1);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TaskId);
    }
}