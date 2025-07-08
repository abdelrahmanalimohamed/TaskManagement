namespace TaskManagement.UnitTests.Application.Features.task
{
	public class CreateTaskHandlerTests
	{
		private readonly Mock<ITaskRepository> _taskRepoMock = new();
		private readonly Mock<IUserRepository> _userRepoMock = new();
		private readonly Mock<ITaskAssignmentHistoryRepository> _historyRepoMock = new();
		private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
		private readonly Mock<ITaskDomainService> _taskDomainServiceMock = new();
		private readonly Mock<IMapper> _mapperMock = new();
		private readonly Mock<ILogger<CreateTaskHandler>> _loggerMock = new();

		private readonly CreateTaskHandler _handler;

		public CreateTaskHandlerTests()
		{
			_handler = new CreateTaskHandler(
				_taskRepoMock.Object,
				_historyRepoMock.Object,
				_unitOfWorkMock.Object,
				_mapperMock.Object,
				_userRepoMock.Object , 
				_taskDomainServiceMock.Object ,
				_loggerMock.Object);
		}

		[Fact]
		public async Task Handle_ShouldThrowException_IfTaskTitleExists()
		{
			var command = new CreateTaskCommand(new CreateTaskDTO ("Test" ));

			_taskRepoMock.Setup(r => r.ExistsAnyAsync(It.IsAny<Expression<Func<Tasks, bool>>>(), default))
				.ReturnsAsync(true);

			await Assert.ThrowsAsync<CustomDuplicateNameException>(() =>
				_handler.Handle(command, CancellationToken.None));

			_loggerMock.Verify(
				log => log.Log(
					LogLevel.Warning,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("duplicated")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);
		}

		[Fact]
		public async Task Handle_ShouldCreateTask_WithAssignedUser_IfUserExists()
		{
			// Arrange
			var dto = new CreateTaskDTO ("New Task");
			var command = new CreateTaskCommand(dto);

			var user = new Users { Name = "User1" };
			var taskEntity = new Tasks();

			// Mocking the repository methods
			_taskRepoMock.Setup(r => r.ExistsAnyAsync(It.IsAny<Expression<Func<Tasks, bool>>>(), default))
				.ReturnsAsync(false);

			_userRepoMock.Setup(r => r.GetUsersWithoutTasksAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<Users> { user });

			_mapperMock.Setup(m => m.Map<Tasks>(dto)).Returns(taskEntity);

			_taskRepoMock.Setup(r => r.AddAsync(taskEntity, It.IsAny<CancellationToken>()))
				.ReturnsAsync(taskEntity);

			_mapperMock.Setup(m => m.Map<GetTasksDTO>(It.IsAny<Tasks>()))
				.Returns(new GetTasksDTO());

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			_historyRepoMock.Verify(r => r.AddAsync(It.IsAny<TaskAssignmentHistory>(), It.IsAny<CancellationToken>()), Times.Once);

			_loggerMock.Verify(l => l.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("assigned to user")),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

			_loggerMock.Verify(l => l.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Task created")),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

			_loggerMock.Verify(l => l.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Inserting assignment history")),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

			_loggerMock.Verify(l => l.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("history inserted")),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
		}

		[Fact]
		public async Task Handle_ShouldCreateTask_WithWaitingState_IfNoUsersExist()
		{
			var dto = new CreateTaskDTO ("New Task");
			var command = new CreateTaskCommand(dto);
			var taskEntity = new Tasks();

			_taskRepoMock.Setup(r => r.ExistsAnyAsync(It.IsAny<Expression<Func<Tasks, bool>>>(), default))
				.ReturnsAsync(false);

			_userRepoMock.Setup(r => r.GetUsersWithoutTasksAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<Users>());

			_mapperMock.Setup(m => m.Map<Tasks>(dto)).Returns(taskEntity);
			_taskRepoMock.Setup(r => r.AddAsync(taskEntity, It.IsAny<CancellationToken>()))
				.ReturnsAsync(taskEntity);

			_mapperMock.Setup(m => m.Map<GetTasksDTO>(It.IsAny<Tasks>()))
				.Returns(new GetTasksDTO());

			var result = await _handler.Handle(command, CancellationToken.None);

			Assert.Equal(TaskState.Waiting, taskEntity.State);
			_historyRepoMock.Verify(r => r.AddAsync(It.IsAny<TaskAssignmentHistory>(), It.IsAny<CancellationToken>()), Times.Never);

			_loggerMock.Verify(l => l.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No available users")),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

			_loggerMock.Verify(l => l.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Task created")),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
		}
	}

}
