namespace TaskManagement.UnitTests.Infrastructure;
public class UserRepositoryTests
{
	private async Task<AppDbContext> GetInMemoryDbContext()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
			.Options;

		var context = new AppDbContext(options);
		var user1 = new Users
		{
			Name = "User 1",
			Tasks = new List<Tasks>
		{
			new Tasks {Title = "Test task 1" , State = TaskState.Completed }
		}
		};
		var user2 = new Users
		{
			Name = "User 2",
			Tasks = new List<Tasks>
		{
			new Tasks { Title = "Test task 2" ,State = TaskState.InProgress }
		}
		};
		var user3 = new Users
		{
			Name = "User 3"
		};

		context.Users.AddRange(user1, user2, user3);

		await context.SaveChangesAsync();
		return context;
	}
	[Fact]
	public async Task GetUsersWithoutTasksAsync_ReturnsOnlyUsersWithoutInProgressTasks()
	{
		// Arrange
		using var context = await GetInMemoryDbContext();

		// Act & Assert
	
		var repository = new UserRepository(context);
		var usersWithoutInProgress = await repository.GetUsersWithoutTasksAsync();

		Assert.Contains(usersWithoutInProgress, u => u.Name == "User 1");
		Assert.Contains(usersWithoutInProgress, u => u.Name == "User 3");
		Assert.DoesNotContain(usersWithoutInProgress, u => u.Name == "User 2");
	}
}
