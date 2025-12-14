using FluentValidation;
using TaskManagement.Application.Abstractions;
namespace TaskManagement.Application.Dispatchers;
using Microsoft.Extensions.DependencyInjection;

public class CommandDispatcher: ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> Dispatch<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TResponse>
    {
        var validator = _serviceProvider.GetService<IValidator<TCommand>>();
        
        if (validator != null)
        {
            var validationResult = validator.Validate(command);
            
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
        
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();
        return handler.Handle(command, cancellationToken);
    }
}