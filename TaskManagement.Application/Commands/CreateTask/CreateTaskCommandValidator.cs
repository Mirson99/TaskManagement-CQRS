using FluentValidation;

namespace TaskManagement.Application.Commands.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters");
        
        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DueAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Due date must be in the future");
        
        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Invalid priority value");
    }
}