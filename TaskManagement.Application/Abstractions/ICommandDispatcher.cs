namespace TaskManagement.Application.Abstractions;

public interface ICommandDispatcher
{
    Task<TResponse> Dispatch<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken=default)
        where TCommand : ICommand<TResponse>;
}