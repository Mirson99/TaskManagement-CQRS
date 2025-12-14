namespace TaskManagement.Application.Abstractions;

public interface IQueryDispatcher
{
    Task<TResponse> Dispatch<TQuery ,TResponse>(TQuery query, CancellationToken cancellationToken=default)
        where TQuery : IQuery<TResponse>;
}