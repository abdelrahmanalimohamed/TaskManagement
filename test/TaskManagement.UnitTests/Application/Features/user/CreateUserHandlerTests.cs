namespace TaskManagement.UnitTests.Application.Features.user;
public class CreateUserHandlerTests
{
	private readonly Mock<IUserRepository> _userRepoMock = new();
	private readonly Mock<ITaskRepository> _taskRepoMock = new();
	private readonly Mock<ITaskAssignmentHistoryRepository> _historyRepoMock = new();
	private readonly Mock<ITaskDomainService> _taskDomainServiceMock = new();
	private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
	private readonly Mock<IMapper> _mapperMock = new();
	private readonly Mock<ILogger<CreateUserHandler>> _loggerMock = new();

	private readonly CreateUserHandler _handler;
	public CreateUserHandlerTests()
	{
		_handler = new CreateUserHandler(
			_userRepoMock.Object,
			_taskRepoMock.Object,
			_unitOfWorkMock.Object,
			_historyRepoMock.Object,
			_taskDomainServiceMock.Object,
			_mapperMock.Object ,
			_loggerMock.Object);
	}

	[Fact]
	public async Task Handle_ShouldThrowException_IfUserExists()
	{
		var command = new CreateUserCommand(new CreateUserDTO ("ExistingUser" ));

		_userRepoMock.Setup(r => r.ExistsAnyAsync(It.IsAny<Expression<Func<Users, bool>>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		await Assert.ThrowsAsync<CustomDuplicateNameException>(() =>
			_handler.Handle(command, CancellationToken.None));

		VerifyLog(_loggerMock, LogLevel.Warning, "User name", Times.Once);
	}

	[Fact]
	public async Task Handle_ShouldCreateUser_AndAssignPendingTasks()
	{
		var userDto = new CreateUserDTO ("NewUser");
		var createdUser = new Users { Name = "NewUser" };
		var pendingTask = new Tasks { State = TaskState.Waiting, AssignmentHistory = new List<TaskAssignmentHistory>() };

		_userRepoMock.Setup(r => r.ExistsAnyAsync(It.IsAny<Expression<Func<Users, bool>>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		_mapperMock.Setup(m => m.Map<Users>(userDto)).Returns(createdUser);
		_userRepoMock.Setup(r => r.AddAsync(createdUser, It.IsAny<CancellationToken>())).ReturnsAsync(createdUser);

		_taskRepoMock.Setup(r => r.GetAllPendingTasksAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Tasks> { pendingTask });

		_mapperMock.Setup(m => m.Map<GetUsersDTO>(It.IsAny<Users>()))
			.Returns((Users user) => new GetUsersDTO(user.Id, user.Name, user.CreatedDate.ToString("yyyy-MM-dd")));

		var result = await _handler.Handle(new CreateUserCommand(userDto), CancellationToken.None);

		Assert.NotNull(result);
		_taskRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Tasks>(), It.IsAny<CancellationToken>()), Times.Once);
		_historyRepoMock.Verify(r => r.AddAsync(It.IsAny<TaskAssignmentHistory>(), It.IsAny<CancellationToken>()), Times.Once);

		VerifyLog(_loggerMock, LogLevel.Information, "User created with ID", Times.Once);
		VerifyLog(_loggerMock, LogLevel.Information, "Auto-assigning pending tasks", Times.Once);
		VerifyLog(_loggerMock, LogLevel.Information, "Assigning task ID", Times.Once);
		VerifyLog(_loggerMock, LogLevel.Information, "Pending task assignment process completed", Times.Once);
	}

	[Fact]
	public async Task Handle_ShouldSkipAlreadyAssignedTasks()
	{
		var userDto = new CreateUserDTO ("NewUser");
		var createdUser = new Users { Name = "NewUser" };

		var taskWithHistory = new Tasks
		{
			State = TaskState.Waiting,
			AssignmentHistory = new List<TaskAssignmentHistory>
		{
			new TaskAssignmentHistory { UserId = createdUser.Id }
		}
		};

		_userRepoMock.Setup(r => r.ExistsAnyAsync(It.IsAny<Expression<Func<Users, bool>>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		_mapperMock.Setup(m => m.Map<Users>(userDto)).Returns(createdUser);
		_userRepoMock.Setup(r => r.AddAsync(createdUser, It.IsAny<CancellationToken>())).ReturnsAsync(createdUser);
		_taskRepoMock.Setup(r => r.GetAllPendingTasksAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Tasks> { taskWithHistory });

		_mapperMock.Setup(m => m.Map<GetUsersDTO>(It.IsAny<Users>()))
			.Returns((Users user) => new GetUsersDTO(user.Id, user.Name, user.CreatedDate.ToString("yyyy-MM-dd")));

		var result = await _handler.Handle(new CreateUserCommand(userDto), CancellationToken.None);

		_taskRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Tasks>(), It.IsAny<CancellationToken>()), Times.Never);
		_historyRepoMock.Verify(r => r.AddAsync(It.IsAny<TaskAssignmentHistory>(), It.IsAny<CancellationToken>()), Times.Never);
		VerifyLog(_loggerMock, LogLevel.Debug, "already assigned to user ID", Times.Once);
	}
	private void VerifyLog<T>(Mock<ILogger<T>> loggerMock, LogLevel level, string expectedMessage, Func<Times> times)
	{
		loggerMock.Verify(x =>
			x.Log(
				level,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedMessage)),
				It.IsAny<Exception>(),
				It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
			times);
	}
}
