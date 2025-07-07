namespace TaskManagement.UnitTests.Infrastructure;
public class BaseRepositoryTests
{
	private async Task<AppDbContext> GetInMemoryDbContext()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
			.Options;

		var context = new AppDbContext(options);

		// Seed data
		context.Tasks.AddRange(new List<Tasks>
		{
			new Tasks { Title = "Task 1"  },
			new Tasks { Title = "Task 2" },
			new Tasks { Title = "Task 3" },
			new Tasks { Title = "Task 4" },
		});

		await context.SaveChangesAsync();
		return context;
	}

	[Fact]
	public async Task GetAllWithPagingAsync_ReturnsPagedData()
	{
		// Arrange
		var context = await GetInMemoryDbContext();
		var repository = new BaseRepository<Tasks>(context);

		var requestParams = new RequestParameters
		{
			PageNumber = 1,
			PageSize = 2
		};

		// Act
		var pagedResult = await repository.GetAllWithPagingAsync(requestParams);

		// Assert
		Assert.NotNull(pagedResult);
		Assert.Equal(2, pagedResult.Count);
		Assert.Equal(4, pagedResult.MetaData.TotalCount);
	}
}
