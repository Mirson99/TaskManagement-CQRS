using FluentValidation.TestHelper;
using TaskManagement.Application.Commands.CreateTask;
using TaskManagement.Domain.Enums;


namespace TaskManagement.Application.Tests.Commands.CreateTask;

public class CreateTaskCommandValidatorTests
{
    private readonly CreateTaskCommandValidator _validator;
    
    public CreateTaskCommandValidatorTests()
    {
        _validator = new CreateTaskCommandValidator();
    }
    
    [Fact]
    public void Validate_Should_HaveError_WhenTitleIsEmpty()
    {
        // Arrange
        var command = new CreateTaskCommand("", "TestDescription", DateTime.UtcNow.AddDays(1), 0);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required");
    }
    
    [Fact]
    public void Validate_Should_HaveError_WhenTitleHasMoreThan200Characters()
    {
        // Arrange
        var tooLongTitle = new string('a', 201);
        var command = new CreateTaskCommand(tooLongTitle, "TestDescription", DateTime.UtcNow.AddDays(1), 0);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title must not exceed 200 characters");
    }
    
    [Fact]
    public void Validate_Should_HaveError_WhenDescriptionHasMoreThan1000Characters()
    {
        // Arrange
        var tooLongDescription = new string('a', 1001);
        var command = new CreateTaskCommand("TestTitle", tooLongDescription, DateTime.UtcNow.AddDays(1), 0);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 1000 characters");
    }
    
    [Fact]
    public void Validate_Should_HaveError_WhenDueDateIsPast()
    {
        // Arrange
        var command = new CreateTaskCommand("TestTitle", "TestDescription", DateTime.MinValue, 0);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DueAt)
            .WithErrorMessage("Due date must be in the future");
    }
    
    [Fact]
    public void Validate_Should_HaveError_WhenPriorityIsNotInEnum()
    {
        // Arrange
        var command = new CreateTaskCommand("TestTitle", "TestDescription", DateTime.UtcNow.AddDays(1), TaskPriority.High + 2);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Priority)
            .WithErrorMessage("Invalid priority value");
    }
    
    [Fact]
    public void Validate_Should_NotHaveError_WhenInputIsOkay()
    {
        // Arrange
        var command = new CreateTaskCommand("TestTitle", "TestDescription", DateTime.UtcNow.AddDays(1), TaskPriority.High);
        
        // Act
        var result = _validator.TestValidate(command);
        
        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
        result.ShouldNotHaveValidationErrorFor(x => x.DueAt);
        result.ShouldNotHaveValidationErrorFor(x => x.Priority);
    }
}