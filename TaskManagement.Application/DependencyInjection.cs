using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Commands.CompleteTask;
using TaskManagement.Application.Commands.CreateTask;
using TaskManagement.Application.Commands.DeleteTask;
using TaskManagement.Application.Dispatchers;
using TaskManagement.Application.Queries.GetTaskById;

namespace TaskManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        
        services.AddScoped<IValidator<CreateTaskCommand>, CreateTaskCommandValidator>();
        services.AddScoped<IValidator<CompleteTaskCommand>, CompleteTaskCommandValidator>();
        services.AddScoped<IValidator<DeleteTaskCommand>, DeleteTaskCommandValidator>();
        services.AddScoped<IValidator<GetTaskByIdQuery>, GetTaskByIdQueryValidator>();
        return services;
    }
}