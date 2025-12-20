# Task Management API - CQRS Pattern

An educational project demonstrating the **CQRS (Command Query Responsibility Segregation)** pattern implementation in .NET **without using MediatR**. Built to understand the fundamentals of CQRS, Clean Architecture, and modern .NET development practices.

## 🎯 Purpose

This project was created as a learning exercise to:
- Understand CQRS pattern from first principles
- Implement custom command/query dispatchers
- Practice Clean Architecture layering
- Learn FluentValidation integration
- Build a simple REST API with proper error handling

## 🛠️ Tech Stack

- **.NET 9** 
- **Entity Framework Core** - ORM with SQLite
- **FluentValidation** - Request validation
- **Swagger/OpenAPI** - API documentation
- **SQLite** - Lightweight database (no setup required)

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Any IDE (Visual Studio, Rider, VS Code)

### Installation & Running

1. **Clone the repository**
   ```bash
   git clone https://github.com/Mirson99/TaskManagement-CQRS.git
   cd TaskManagement-CQRS
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update --project TaskManagement.Infrastructure --startup-project TaskManagement.API
   ```

4. **Run the application**
   ```bash
   dotnet run --project TaskManagement.API
   ```

5. **Open Swagger UI**
   ```
   https://localhost:7001/swagger
   ```

## 📚 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/tasks` | Get all tasks |
| `GET` | `/api/tasks/{id}` | Get task by ID |
| `POST` | `/api/tasks` | Create new task |
| `PUT` | `/api/tasks/{id}/complete` | Mark task as completed |
| `DELETE` | `/api/tasks/{id}` | Delete task |


## 🎓 Key Concepts Demonstrated

### CQRS Pattern
- **Commands** - Modify system state (Create, Update, Delete)
- **Queries** - Read data without modifications
- **Dispatchers** - Route commands/queries to appropriate handlers
- **Handlers** - Execute business logic

### Custom Implementation
Unlike most CQRS examples that use **MediatR**, this project implements:
- Custom `ICommand<TResponse>` and `IQuery<TResponse>` interfaces
- Custom `CommandDispatcher` and `QueryDispatcher`
- Manual handler registration in DI container

This approach provides deeper understanding of how CQRS works under the hood.

### Clean Architecture Benefits
- **Separation of Concerns** - Each layer has a single responsibility
- **Testability** - Business logic isolated from infrastructure
- **Maintainability** - Changes in one layer don't affect others
- **Independence** - Domain layer has no external dependencies

### FluentValidation
All commands are validated before execution:
- `CreateTaskCommand` - Title required, max lengths, valid priority
- `CompleteTaskCommand` - Valid task ID
- `DeleteTaskCommand` - Valid task ID

### Error Handling
Global middleware catches and formats errors:
- **Validation errors** → 400 Bad Request with detailed field errors
- **Server errors** → 500 Internal Server Error with message

## 🧪 Testing

- Unit tests for all command/query handlers
- Integration tests for API endpoints
