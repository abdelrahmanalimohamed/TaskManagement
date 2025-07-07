namespace TaskManagement.UnitTests.Application.Features.user;
public class CreateUserHandlerTests
{
	private readonly Mock<IUserRepository> _userRepoMock = new();
	private readonly Mock<ITaskRepository> _taskRepoMock = new();
	private readonly Mock<ITaskAssignmentHistoryRepository> _historyRepoMock = new();
	private readonly Mock<ITaskDomainService> _taskDomainServiceMock = new();
	private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
	private readonly Mock<IMapper> _mapperMock = new();

	private readonly CreateUserHandler _handler;
	public CreateUserHandlerTests()
	{
		_handler = new CreateUserHandler(
			_userRepoMock.Object,
			_taskRepoMock.Object,
			_unitOfWorkMock.Object,
			_historyRepoMock.Object,
			_taskDomainServiceMock.Object,
			_mapperMock.Object);
	}

	[Fact]
	public async Task Handle_ShouldThrowException_IfUserExists()
	{
		var command = new CreateUserCommand(new CreateUserDTO ("ExistingUser" ));

		_userRepoMock.Setup(r => r.ExistsAnyAsync(It.IsAny<Expression<Func<Users, bool>>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		await Assert.ThrowsAsync<CustomDuplicateNameException>(() =>
			_handler.Handle(command, CancellationToken.None));
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
	}
}
