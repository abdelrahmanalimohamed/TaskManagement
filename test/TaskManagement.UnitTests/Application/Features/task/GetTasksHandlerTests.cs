namespace TaskManagement.UnitTests.Application.Features.task;
public class GetTasksHandlerTests
{
	private readonly Mock<ITaskRepository> _taskRepositoryMock;
	private readonly Mock<IMapper> _mapperMock;
	private readonly GetTasksHandler _handler;
	private readonly Mock<ILogger<GetTasksHandler>> _loggerMock;

	public GetTasksHandlerTests()
	{
		_taskRepositoryMock = new Mock<ITaskRepository>();
		_mapperMock = new Mock<IMapper>();
		_loggerMock = new Mock<ILogger<GetTasksHandler>>();

		_handler = new GetTasksHandler(
			_taskRepositoryMock.Object,
			_mapperMock.Object , 
			_loggerMock.Object);
	}

	[Fact]
	public async Task Handle_ShouldReturnPagedTasksDTO()
	{
		// Arrange
		var requestParams = new RequestParameters { PageNumber = 1, PageSize = 10 };

		var tasks = new List<Tasks>
		{
			new Tasks {Title = "Task 1" },
			new Tasks {Title = "Task 2"}
		};

		var pagedTasks = new PagedList<Tasks>(tasks, 2, 1, 10);

		_taskRepositoryMock
			.Setup(r => r.GetAllWithUsersAsync(requestParams, It.IsAny<CancellationToken>()))
			.ReturnsAsync(pagedTasks);

		var mappedDTOs = new List<GetTasksDTO>
		{
			new GetTasksDTO { Title = "Task 1", CreatedDate = DateTime.Now.ToString("yyyy-MM-dd") },
			new GetTasksDTO { Title = "Task 2", CreatedDate = DateTime.Now.ToString("yyyy-MM-dd") }
		};

		_mapperMock
			.Setup(m => m.Map<IEnumerable<GetTasksDTO>>(pagedTasks))
			.Returns(mappedDTOs);

		var query = new GetTasksQuery { Parameters = requestParams };

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		_loggerMock.Verify(
			log => log.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Retrieved") && v.ToString().Contains("tasks")),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Once);
		Assert.NotNull(result);
		Assert.Equal(2, result.Items.Count());
		Assert.Equal(mappedDTOs, result.Items);
		Assert.Equal(pagedTasks.MetaData.TotalCount, result.MetaData.TotalCount);
	}
}
