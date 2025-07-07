namespace TaskManagement.Infrastructure.Tests.BackgroundServices;
public class TaskReassignmentBackgroundServiceTests
{
	private readonly Mock<IServiceProvider> _mockServiceProvider;
	private readonly Mock<ILogger<TaskReassignmentBackgroundService>> _mockLogger;
	private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
	private readonly Mock<IServiceScope> _mockServiceScope;
	private readonly Mock<ITaskReassignmentService> _mockTaskReassignmentService;
	public TaskReassignmentBackgroundServiceTests()
	{
		_mockServiceProvider = new Mock<IServiceProvider>();
		_mockLogger = new Mock<ILogger<TaskReassignmentBackgroundService>>();
		_mockScopeFactory = new Mock<IServiceScopeFactory>();
		_mockServiceScope = new Mock<IServiceScope>();
		_mockTaskReassignmentService = new Mock<ITaskReassignmentService>();

		_mockServiceProvider.Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
							.Returns(_mockScopeFactory.Object);

		_mockScopeFactory.Setup(sf => sf.CreateScope())
						 .Returns(_mockServiceScope.Object);

		_mockServiceScope.Setup(s => s.ServiceProvider.GetService(typeof(ITaskReassignmentService)))
						 .Returns(_mockTaskReassignmentService.Object);
	}

	[Fact]
	public async Task ExecuteAsync_CallsReassignTasksAsync_AndLogsInformation()
	{
		// Arrange
		var service = new TaskReassignmentBackgroundService(_mockServiceProvider.Object, _mockLogger.Object);
		var cancellationTokenSource = new CancellationTokenSource();

		_mockTaskReassignmentService
			.Setup(s => s.ReassignTasksAsync(It.IsAny<CancellationToken>()))
			.Callback(() => cancellationTokenSource.Cancel()) 
			.Returns(Task.CompletedTask);

		// Act
		try
		{
			await Task.Run(() => service.StartAsync(cancellationTokenSource.Token));
		}
		catch (TaskCanceledException)
		{
		}

		// Assert
		_mockTaskReassignmentService.Verify(s => s.ReassignTasksAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);

		_mockLogger.Verify(
			l => l.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Task reassignment completed")),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception, string>>()),
			Times.Once);
	}
	[Fact]
	public async Task ExecuteAsync_LogsError_WhenExceptionOccurs()
	{
		// Arrange
		var service = new TaskReassignmentBackgroundService(_mockServiceProvider.Object, _mockLogger.Object);
		var cancellationTokenSource = new CancellationTokenSource();
		var testException = new InvalidOperationException("Test exception during reassignment.");

		_mockTaskReassignmentService
			.Setup(s => s.ReassignTasksAsync(It.IsAny<CancellationToken>()))
			.Callback(() => cancellationTokenSource.Cancel()) // Cancel after the first call
			.Throws(testException);

		// Act
		try
		{
			await Task.Run(() => service.StartAsync(cancellationTokenSource.Token));
		}
        catch (TaskCanceledException)
		{
		}

		// Assert
		_mockTaskReassignmentService.Verify(s => s.ReassignTasksAsync(It.IsAny<CancellationToken>()), Times.Once);

		_mockLogger.Verify(
			l => l.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Error occurred during task reassignment")),
				testException, // Verify that the specific exception was logged
				It.IsAny<Func<It.IsAnyType, Exception, string>>()),
			Times.Once);
	}
}
