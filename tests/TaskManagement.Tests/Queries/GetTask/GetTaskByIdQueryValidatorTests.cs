using FluentValidation.TestHelper;
using TaskManagement.Application.Commands.DeleteTask;
using TaskManagement.Application.Queries.GetTaskById;

namespace TaskManagement.Application.Tests.Queries.GetTask;

public class GetTaskByIdQueryValidatorTests
{
    private readonly GetTaskByIdQueryValidator _validator;
    
    public GetTaskByIdQueryValidatorTests()
    {
        _validator = new GetTaskByIdQueryValidator();
    }
    
    [Fact]
    public void Validate_Should_HaveError_WhenTaskIdIsZero()
    {
        // Arrange
        var command = new GetTaskByIdQuery(0);
        
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
        var command = new GetTaskByIdQuery(-1);
        
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
        var query = new GetTaskByIdQuery(1);
    
        // Act
        var result = _validator.TestValidate(query);
    
        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TaskId);
    }
}