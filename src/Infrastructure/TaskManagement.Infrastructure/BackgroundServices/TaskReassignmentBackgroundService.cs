namespace TaskManagement.Infrastructure.BackgroundServices;
public class TaskReassignmentBackgroundService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<TaskReassignmentBackgroundService> _logger;
	private readonly TimeSpan _interval = TimeSpan.FromMinutes(2);
	public TaskReassignmentBackgroundService(
		IServiceProvider serviceProvider,
		ILogger<TaskReassignmentBackgroundService> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using var scope = _serviceProvider.CreateScope();
				var reassignmentService = scope.ServiceProvider.GetRequiredService<ITaskReassignmentService>();

				await reassignmentService.ReassignTasksAsync(stoppingToken);

				_logger.LogInformation("Task reassignment completed at {Time}", DateTime.UtcNow);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred during task reassignment");
			}

			await Task.Delay(_interval, stoppingToken);
		}
	}
}