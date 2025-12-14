using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Commands.CompleteTask;
using TaskManagement.Application.Commands.CreateTask;
using TaskManagement.Application.Commands.DeleteTask;
using TaskManagement.Application.Queries.GetAllTasks;
using TaskManagement.Application.Queries.GetTaskById;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Handlers;
using TaskManagement.Infrastructure.Handlers.Queries;

namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TaskDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection")
            ));
        
        services.AddScoped<ICommandHandler<CreateTaskCommand, int>, CreateTaskCommandHandler>();
        services.AddScoped<ICommandHandler<CompleteTaskCommand, bool>, CompleteTaskCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteTaskCommand, bool>, DeleteTaskCommandHandler>();
        services.AddScoped<IQueryHandler<GetAllTasksQuery, List<TaskDto>>, GetAllTasksQueryHandler>();
        services.AddScoped<IQueryHandler<GetTaskByIdQuery, TaskDetailsDto>, GetTaskByIdQueryHandler>();
        
        return services;
    }
}