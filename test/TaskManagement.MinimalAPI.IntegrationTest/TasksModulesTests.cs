using System.Net;
using System.Net.Http.Json;
using TaskManagement.Application.DTO;

namespace TaskManagement.MinimalAPI.IntegrationTest;
public class TasksModulesTests : IClassFixture<TasksManagementModuleWebApplicationFactory>
{
	private readonly HttpClient _client;
	public TasksModulesTests(TasksManagementModuleWebApplicationFactory factory)
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
		var url = "/api/tasks/get-all?pageNumber=1&pageSize=10";
		var response = await _client.GetAsync(url);

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var result = await response.Content.ReadFromJsonAsync<object>();
		Assert.NotNull(result);
	}
}
