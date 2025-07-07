namespace TaskManagement.UnitTests.Application.Services;
public class TaskReassignmentServiceTests
{
	private readonly Mock<ITaskRepository> _taskRepositoryMock = new();
	private readonly Mock<IUserRepository> _userRepositoryMock = new();
	private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
	private readonly Mock<ITaskAssignmentHistoryRepository> _historyRepositoryMock = new();
	private readonly Mock<ITaskDomainService> _taskDomainServiceMock = new();
	private readonly Mock<ILogger<TaskReassignmentService>> _loggerMock = new();

	private TaskReassignmentService CreateService() =>
		new TaskReassignmentService(
			_taskRepositoryMock.Object,
			_userRepositoryMock.Object,
			_unitOfWorkMock.Object,
			_historyRepositoryMock.Object,
			_taskDomainServiceMock.Object,
			_loggerMock.Object);

	[Fact]
	public async Task ReassignTasksAsync_NoPendingTasks_LogsInformationAndReturns()
	{
		_taskRepositoryMock.Setup(r => r.GetAllPendingTasksAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Tasks>());

		var service = CreateService();

		await service.ReassignTasksAsync();

		_loggerMock.Verify(l => l.Log(
			LogLevel.Information,
			It.IsAny<EventId>(),
			It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("No pending tasks found")),
			null,
			It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
	}

	[Fact]
	public async Task ReassignTasksAsync_NoUsers_LogsWarningAndReturns()
	{
		var someTask = new Tasks { Title = "Task 1" };

		_taskRepositoryMock.Setup(r => r.GetAllPendingTasksAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Tasks> { someTask });

		_userRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Users>());

		var service = CreateService();

		await service.ReassignTasksAsync();

		_loggerMock.Verify(l => l.Log(
			LogLevel.Warning,
			It.IsAny<EventId>(),
			It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("No users found")),
			null,
			It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
	}

	[Fact]
	public async Task ReassignTasksAsync_TaskAssignedToAllUsers_MarksCompletedAndUpdates()
	{
		var task = new Tasks { Title = "Task 2" };
		var users = new List<Users>
		{
			new Users { Name = "User1" },
			new Users { Name = "User2" }
		};

		_taskRepositoryMock.Setup(r => r.GetAllPendingTasksAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Tasks> { task });

		_userRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(users);

		_taskDomainServiceMock.Setup(s => s.HasBeenAssignedToAllUsers(task, It.IsAny<List<Guid>>()))
			.Returns(true);

		var service = CreateService();

		await service.ReassignTasksAsync();

		_taskDomainServiceMock.Verify(s => s.MarkAsCompleted(task), Times.Once);
		_taskRepositoryMock.Verify(r => r.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Once);
		_unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task ReassignTasksAsync_EligibleUserFound_TaskAssignedAndHistoryAdded()
	{
		var task = new Tasks { Title = "Task 3" };
		var user = new Users { Name = "EligibleUser" };
		var users = new List<Users> { user };

		var historyEntry = new TaskAssignmentHistory();

		_taskRepositoryMock.Setup(r => r.GetAllPendingTasksAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Tasks> { task });

		_userRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(users);

		_taskDomainServiceMock.Setup(s => s.HasBeenAssignedToAllUsers(task, It.IsAny<List<Guid>>()))
			.Returns(false);

		_taskDomainServiceMock.Setup(s => s.GetEligibleUsers(task, users))
			.Returns(users);

		_taskDomainServiceMock.Setup(s => s.SelectRandomUser(users))
			.Returns(user);

		_taskDomainServiceMock.Setup(s => s.CreateAssignmentHistory(task, user))
			.Returns(historyEntry);

		var service = CreateService();

		await service.ReassignTasksAsync();

		_historyRepositoryMock.Verify(r => r.AddAsync(historyEntry, It.IsAny<CancellationToken>()), Times.Once);
		_taskRepositoryMock.Verify(r => r.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Once);
		_unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
	}
}
