using System.Net;
using System.Net.Http.Json;
using TaskManagement.Application.DTO;

namespace TaskManagement.IntegrationTests;
public class TasksControllerTests : IClassFixture<TaskManagementWebApplicationFactory>
{
	private readonly HttpClient _client;
	public TasksControllerTests(TaskManagementWebApplicationFactory factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task CreateTask_ShouldReturnOk()
	{
		// Arrange
		var newTask = new CreateTaskDTO("TestTask");

		// Act
		var response = await _client.PostAsJsonAsync("/api/tasks/create", newTask);

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var result = await response.Content.ReadFromJsonAsync<object>();
		Assert.NotNull(result);
	}

	[Fact]
	public async Task GetAllTasks_ShouldReturnOk()
	{
		// Act
		var response = await _client.GetAsync("/api/tasks/get-all");

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var result = await response.Content.ReadFromJsonAsync<object>();
		Assert.NotNull(result);
	}
}