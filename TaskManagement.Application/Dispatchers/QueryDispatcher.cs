using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Abstractions;

namespace TaskManagement.Application.Dispatchers;

public class QueryDispatcher: IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public Task<TResponse> Dispatch<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default) where TQuery : IQuery<TResponse>
    {
        var validator = _serviceProvider.GetService<IValidator<TQuery>>();
        
        if (validator != null)
        {
            var validationResult = validator.Validate(query);
            
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
        
        var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
        return handler.Handle(query, cancellationToken);
    }
}