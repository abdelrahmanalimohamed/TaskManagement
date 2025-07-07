namespace TaskManagement.Application.Services;
public class TaskReassignmentService : ITaskReassignmentService
{
	private readonly ITaskRepository _taskRepository;
	private readonly IUserRepository _userRepository;
	private readonly ITaskAssignmentHistoryRepository _historyRepository;
	private readonly ITaskDomainService _taskDomainService;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<TaskReassignmentService> _logger;
	public TaskReassignmentService(
	 ITaskRepository taskRepository,
	 IUserRepository userRepository,
	 IUnitOfWork unitOfWork,
	 ITaskAssignmentHistoryRepository historyRepository,
	 ITaskDomainService taskDomainService,
	 ILogger<TaskReassignmentService> logger)
	{
		_taskRepository = taskRepository;
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
		_historyRepository = historyRepository;
		_logger = logger;
		_taskDomainService = taskDomainService;
	}
	public async Task ReassignTasksAsync(CancellationToken cancellationToken = default)
	{
		var activeTasks = await _taskRepository.GetAllPendingTasksAsync(cancellationToken); 
		if (activeTasks == null || !activeTasks.Any())
		{
			_logger.LogInformation("No pending tasks found for reassignment.");
			return;
		}

		var allUsers = await _userRepository.GetAllAsync(cancellationToken);
		if (allUsers == null || !allUsers.Any())
		{
			_logger.LogWarning("No users found. Task reassignment aborted.");
			return;
		}

		foreach (var task in activeTasks)
		{
			await ProcessTaskReassignmentAsync(task, allUsers, cancellationToken);
		}
	}
	private async Task ProcessTaskReassignmentAsync(
		Tasks task, 
		IEnumerable<Users> allUsers, 
		CancellationToken cancellationToken)
	{
		var allUserIds = allUsers.Select(u => u.Id).ToList();

		if (_taskDomainService.HasBeenAssignedToAllUsers(task , allUserIds))
		{
			_taskDomainService.MarkAsCompleted(task);
			await _taskRepository.UpdateAsync(task, cancellationToken);
			_logger.LogInformation("Task {TaskId} marked as completed - assigned to all users", task.Id);
		}
		else
		{
			var eligibleUsers = _taskDomainService.GetEligibleUsers(task, allUsers);

			if (!eligibleUsers.Any())
			{
				_taskDomainService.SetToWaiting(task);
				await _taskRepository.UpdateAsync(task, cancellationToken);
				_logger.LogInformation("Task {TaskId} set to waiting - no eligible users", task.Id);
			}
			else
			{
				var selectedUser = _taskDomainService.SelectRandomUser(eligibleUsers);
				if (selectedUser == null)
				{
					_logger.LogWarning("No eligible user could be selected for task {TaskId}", task.Id);
					return;
				}

				_taskDomainService.AssignToUser(task, selectedUser.Id);
				var historyEntry = _taskDomainService.CreateAssignmentHistory(task , selectedUser);

				await _historyRepository.AddAsync(historyEntry, cancellationToken);
				await _taskRepository.UpdateAsync(task, cancellationToken);

				_logger.LogInformation("Task {TaskId} reassigned to User {UserId}", task.Id, selectedUser.Id);
			}
		}
		await _unitOfWork.CommitAsync(cancellationToken);
	}
}