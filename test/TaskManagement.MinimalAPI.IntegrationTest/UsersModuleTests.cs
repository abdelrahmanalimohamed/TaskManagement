using System.Net;
using System.Net.Http.Json;
using TaskManagement.Application.DTO;

namespace TaskManagement.MinimalAPI.IntegrationTest;
public class UsersModuleTests : IClassFixture<TasksManagementModuleWebApplicationFactory>
{
	private readonly HttpClient _client;
	public UsersModuleTests(TasksManagementModuleWebApplicationFactory factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task CreateUser_ShouldReturnOk()
	{
		// Arrange
		var newTask = new CreateUserDTO("TestUser");

		// Act
		var response = await _client.PostAsJsonAsync("/api/users/create", newTask);

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var result = await response.Content.ReadFromJsonAsync<object>();
		Assert.NotNull(result);
	}

	[Fact]
	public async Task GetAllUsers_ShouldReturnOk()
	{
		// Act
		var url = "/api/users/get-all?pageNumber=1&pageSize=10";
		var response = await _client.GetAsync(url);

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var result = await response.Content.ReadFromJsonAsync<object>();
		Assert.NotNull(result);
	}
}
