namespace TaskManagement.Application.Features.users.Create;
internal class CreateUserHandler : IRequestHandler<CreateUserCommand , GetUsersDTO>
{
	private readonly IUserRepository  _userRepository;
	private readonly ITaskRepository _taskRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ITaskAssignmentHistoryRepository _historyRepository;
	private readonly ITaskDomainService _taskDomainService;
	private readonly IMapper _mapper;
	private readonly ILogger<CreateUserHandler> _logger;
	public CreateUserHandler(
		IUserRepository userRepository,
		ITaskRepository taskRepository,
		IUnitOfWork unitOfWork,
		ITaskAssignmentHistoryRepository historyRepository,
		ITaskDomainService taskDomainService,
		IMapper mapper,
		ILogger<CreateUserHandler> logger)
	{
		_userRepository = userRepository;
		_taskRepository = taskRepository;
		_unitOfWork = unitOfWork;
		_historyRepository = historyRepository;
		_taskDomainService = taskDomainService;
		_mapper = mapper;
		_logger = logger;
	}
	public async Task<GetUsersDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
	{
		string nameToCheck = request.User.name.ToNormalizedLower();

		if (await _userRepository.ExistsAnyAsync(x => x.Name.ToLower() == nameToCheck , cancellationToken))
		{
			_logger.LogWarning("User name '{UserName}' is duplicated.", request.User.name);
			throw new CustomDuplicateNameException("User name is duplicated");
		}

		var userEntity = _mapper.Map<Users>(request.User);
		var createdUser = await _userRepository.AddAsync(userEntity, cancellationToken);
		await _unitOfWork.CommitAsync(cancellationToken);
		_logger.LogInformation("User created with ID: {UserId}", createdUser.Id);

		await AutoAssignPendingTasksForUserAsync(createdUser, cancellationToken);

		return _mapper.Map<GetUsersDTO>(createdUser);
	}

	private async Task AutoAssignPendingTasksForUserAsync(Users createdUser , CancellationToken cancellationToken)
	{
		_logger.LogInformation("Auto-assigning pending tasks to user ID: {UserId}", createdUser.Id);
		var pendingTasks = await _taskRepository.GetAllPendingTasksAsync(cancellationToken);

		foreach (var task in pendingTasks)
		{
			bool alreadyAssigned = task.AssignmentHistory.Any(h => h.UserId == createdUser.Id);
			if (!alreadyAssigned)
			{
				if (task.State == TaskState.Waiting)
				{
					_logger.LogInformation("Assigning task ID: {TaskId} to user ID: {UserId}", task.Id, createdUser.Id);
					_taskDomainService.AssignToUser(task, createdUser.Id);
				}

				var historyEntry = _taskDomainService.CreateAssignmentHistory(task , createdUser);

				await _historyRepository.AddAsync(historyEntry, cancellationToken);
				await _taskRepository.UpdateAsync(task, cancellationToken);
			}
			else
			{
				_logger.LogDebug("Task ID: {TaskId} already assigned to user ID: {UserId}, skipping.", task.Id, createdUser.Id);
			}
		}
		await _unitOfWork.CommitAsync(cancellationToken);
		_logger.LogInformation("Pending task assignment process completed for user ID: {UserId}", createdUser.Id);
	}
}