using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TaskManagament.IntegrationTests.Fixtures;
using TaskManagament.IntegrationTests.Helpers;
using TaskManagement.API.Controllers;
using TaskManagement.Application.Queries.GetAllTasks;
using TaskManagement.Application.Queries.GetTaskById;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Data;

namespace TaskManagament.IntegrationTests;

public class TaskCotrollerTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;

    public TaskCotrollerTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    public void SeedTestData(TaskDbContext context)
    {
        context.Tasks.Add(new TaskItem { Id = 10, Title = "Test Task", Description = "Test Task description", DueAt = new DateTime(2028, 01, 01) });
        context.Tasks.Add(new TaskItem { Id = 9, Title = "Test Task", Description = "Test Task description", DueAt = new DateTime(2028, 01, 01) });
        context.SaveChanges();
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        _client.Dispose();
    }
    
    [Fact]
    public async Task CreateTask_WithInvalidData_DispatcherThrowsValidationException()
    {
        var request = new CreateTaskRequest 
        { 
            Title = "",
            Description = "Test",
            Priority = TaskPriority.Normal,
            DueDate = DateTime.UtcNow.AddDays(1),
        };
    
        var response = await _client.PostAsJsonAsync("/api/tasks", request);
    
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Title is required");
    }
    
    [Fact]
    public async Task CreateTask_WithTooLongTitle_DispatcherValidationFails()
    {
        var request = new CreateTaskRequest 
        { 
            Title = new String('a', 1000),
            Description = "Test",
            Priority = TaskPriority.Normal,
            DueDate = DateTime.UtcNow.AddDays(1),
        };
    
        var response = await _client.PostAsJsonAsync("/api/tasks", request);
    
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Title must not exceed 200 characters");
    }

    [Fact]
    public async Task CreateTask_WithValidData_DispatcherExecutesSuccessfully()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "Integration Test Task",
            Description = "This is a test description",
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = TaskPriority.High
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateTaskResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        result.Should().NotBe(null);
        result.Id.Should().BeGreaterThan(0);

        var getResponse = await _client.GetAsync($"/api/tasks/{result.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var taskContent = await getResponse.Content.ReadAsStringAsync();
        var task = JsonSerializer.Deserialize<TaskDetailsDto>(taskContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        task.Should().NotBeNull();
        task!.Title.Should().Be(request.Title);
        task.Description.Should().Be(request.Description);
        task.DueAt.Should().Be(request.DueDate);
        task.IsCompleted.Should().BeFalse();
        task.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task CreateTask_WithNullDescription_CreatesTaskSuccessfully()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "Task without description",
            Description = "",
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = TaskPriority.High
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", request);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateTaskResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        result.Should().NotBe(null);
        result.Id.Should().BeGreaterThan(0);

        var getResponse = await _client.GetAsync($"/api/tasks/{result.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var taskContent = await getResponse.Content.ReadAsStringAsync();
        var task = JsonSerializer.Deserialize<TaskDetailsDto>(taskContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        task.Should().NotBeNull();
        task!.Title.Should().Be(request.Title);
        task.Description.Should().Be(request.Description);
        task.DueAt.Should().Be(request.DueDate);
        task.IsCompleted.Should().BeFalse();
        task.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetTaskById_ExistingTask_QueryDispatcherReturnsTask()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        SeedTestData(context);
        
        // Act
        var getResponse = await _client.GetAsync($"/api/tasks/10");
        
        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var taskContent = await getResponse.Content.ReadAsStringAsync();
        var task = JsonSerializer.Deserialize<TaskDetailsDto>(taskContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        task.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetTaskById_NonExistingTask_QueryDispatcherThrowsNotFound()
    {
        // Arrange
        var id = 99;
        // Act
        var getResponse = await _client.GetAsync($"/api/tasks/{id}");
        
        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await getResponse.Content.ReadAsStringAsync();
        content.Should().Contain($"Task with ID {id} not found");
    }
    
    [Fact]
    public async Task CompleteTask_ExistingTask_DispatcherUpdatesSuccessfully()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        SeedTestData(context);
        var id = 10;
        // Act
        var response = await _client.PutAsync($"/api/tasks/{id}/complete", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/tasks/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var taskContent = await getResponse.Content.ReadAsStringAsync();
        var task = JsonSerializer.Deserialize<TaskDetailsDto>(taskContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        task.IsCompleted.Should().BeTrue();
        task.Status.Should().Be("Completed");
    }
    
    [Fact]
    public async Task CompleteTask_NonExistingTask_DispatcherHandlesNotFound()
    {
        // Arrange
        var id = 99;
        // Act
        var response = await _client.PutAsync($"/api/tasks/{id}/complete", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain($"Task with ID {id} not found");
    }
    
    [Fact]
    public async Task DeleteTask_ExistingTask_DispatcherDeletesSuccessfully()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        SeedTestData(context);
        var id = 10;
        // Act
        var response = await _client.DeleteAsync($"/api/tasks/{id}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/tasks/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var getContent = await getResponse.Content.ReadAsStringAsync();
        getContent.Should().Contain($"Task with ID {id} not found");
    }
    
    [Fact]
    public async Task DeleteTask_NonExistingTask_DispatcherHandlesNotFound()
    {
        // Arrange
        var id = 99;
        // Act
        var response = await _client.DeleteAsync($"/api/tasks/{id}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain($"Task with ID {id} not found");
    }

    [Fact]
    public async Task CreateGetCompleteDelete_FullFlow_AllDispatchersWork()
    {
        var request = new CreateTaskRequest
        {
            Title = "Integration Test Task",
            Description = "This is a test description",
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = TaskPriority.High
        };
        
        var response = await _client.PostAsJsonAsync("/api/tasks", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateTaskResponse>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        result.Should().NotBe(null);
        result.Id.Should().BeGreaterThan(0);

        var getResponse = await _client.GetAsync($"/api/tasks/{result.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var taskContent = await getResponse.Content.ReadAsStringAsync();
        var task = JsonSerializer.Deserialize<TaskDetailsDto>(taskContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        task.Should().NotBeNull();
        task!.Title.Should().Be(request.Title);
        task.Description.Should().Be(request.Description);
        task.DueAt.Should().Be(request.DueDate);
        task.IsCompleted.Should().BeFalse();
        task.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
        var completeResponse = await _client.PutAsync($"/api/tasks/{result.Id}/complete", null);
        
        completeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var completeGetResponse = await _client.GetAsync($"/api/tasks/{result.Id}");
        completeGetResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        taskContent = await completeGetResponse.Content.ReadAsStringAsync();
        task = JsonSerializer.Deserialize<TaskDetailsDto>(taskContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        task.IsCompleted.Should().BeTrue();
        task.Status.Should().Be("Completed");
        
        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{task.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getDeleteResponse = await _client.GetAsync($"/api/tasks/{task.Id}");
        getDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var getContent = await getDeleteResponse.Content.ReadAsStringAsync();
        getContent.Should().Contain($"Task with ID {task.Id} not found");
    }
    
    [Fact]
    public async Task GetAllTasks_ReturnsListOfTasks()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        SeedTestData(context);
        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var taskContents = await response.Content.ReadAsStringAsync();
        var tasks = JsonSerializer.Deserialize<List<TaskDto>>(taskContents,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        tasks.Should().NotBeNull();
        tasks.Count.Should().Be(2);
        tasks.Should().Contain(t => t.Id == 9);
        tasks.Should().Contain(t => t.Id == 10);
    }
}