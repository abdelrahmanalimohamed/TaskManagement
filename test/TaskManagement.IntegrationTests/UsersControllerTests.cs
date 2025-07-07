using System.Net;
using System.Net.Http.Json;
using TaskManagement.Application.DTO;

namespace TaskManagement.IntegrationTests;

public class UsersControllerTests : IClassFixture<TaskManagementWebApplicationFactory>
{
	private readonly HttpClient _client;

	public UsersControllerTests(TaskManagementWebApplicationFactory factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task CreateUser_ShouldReturnOk()
	{
		// Arrange
		var newUser = new CreateUserDTO("TestUser");

		// Act
		var response = await _client.PostAsJsonAsync("/api/users/create", newUser);

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var result = await response.Content.ReadFromJsonAsync<object>();
		Assert.NotNull(result);
	}

	[Fact]
	public async Task GetAllUsers_ShouldReturnOk()
	{
		// Act
		var response = await _client.GetAsync("/api/users/get-all");

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var result = await response.Content.ReadFromJsonAsync<object>();
		Assert.NotNull(result);
	}
}
