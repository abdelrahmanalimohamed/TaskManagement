namespace TaskManagement.UnitTests.Infrastructure;
public class TaskRepositoryTests
{
	private async Task<AppDbContext> GetInMemoryDbContext()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		var context = new AppDbContext(options);

		var userOne = new Users { Name = "UserOne" };
		var userTwo = new Users { Name = "UserTwo" };

		var tasks = new List<Tasks>
		{
			new Tasks
			{
				Title = "Waiting Task 1",
				State = TaskState.Waiting,
				Users = userOne,
				AssignmentHistory = new List<TaskAssignmentHistory>
				{
					new TaskAssignmentHistory
					{
						User = userOne
					}
				}
			},
			new Tasks
			{
				Title = "Completed Task",
				State = TaskState.Completed,
				Users = userTwo,
				AssignmentHistory = new List<TaskAssignmentHistory>
				{
				    new TaskAssignmentHistory { User = userTwo }
				}
			},
			new Tasks
			{
				Title = "Waiting Task 2",
				State = TaskState.Waiting,
				Users = userTwo,
				AssignmentHistory = new List<TaskAssignmentHistory>
				{
					new TaskAssignmentHistory { User = userTwo }
				}
			}
		};

		await context.Users.AddRangeAsync(userOne, userTwo);
		await context.Tasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();

		return context;
	}

	[Fact]
	public async Task GetAllPendingTasksAsync_ReturnsOnlyPendingTasksWithAssignmentHistory()
	{
		// Arrange
		var context = await GetInMemoryDbContext();
		var repository = new TaskRepository(context);

		// Act
		var result = await repository.GetAllPendingTasksAsync(CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.All(result, t => Assert.NotEqual(TaskState.Completed, t.State));
		Assert.Contains(result, t => t.AssignmentHistory != null && t.AssignmentHistory.Any());
	}

	[Fact]
	public async Task GetAllWithUsersAsync_ReturnsPagedTasksIncludingUsers()
	{
		// Arrange
		var context = await GetInMemoryDbContext();
		var repository = new TaskRepository(context);

		var requestParams = new RequestParameters
		{
			PageNumber = 1,
			PageSize = 10
		};

		// Act
		var pagedResult = await repository.GetAllWithUsersAsync(requestParams, CancellationToken.None);

		// Assert
		Assert.NotNull(pagedResult);
		Assert.True(pagedResult.Count > 0);
		Assert.All(pagedResult, t => Assert.NotNull(t.Users));
	}
}
