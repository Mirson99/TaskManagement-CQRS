namespace TaskManagement.Application.Abstractions;

public interface ICommand
{
}

public interface ICommand<TResponse> : IBaseCommand
{
}

public interface IBaseCommand
{
}