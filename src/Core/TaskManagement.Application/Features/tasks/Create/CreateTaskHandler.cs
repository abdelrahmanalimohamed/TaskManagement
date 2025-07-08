namespace TaskManagement.Application.Features.tasks.Create;
internal class CreateTaskHandler : IRequestHandler<CreateTaskCommand , GetTasksDTO>
{
	private readonly ITaskRepository _taskRepository;
	private readonly ITaskAssignmentHistoryRepository _historyRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserRepository _userRepository;
	private readonly ITaskDomainService _taskDomainService;
	private IMapper _mapper;
	private readonly ILogger<CreateTaskHandler> _logger;
	public CreateTaskHandler(
		ITaskRepository taskRepository,
		ITaskAssignmentHistoryRepository historyRepository,
		IUnitOfWork unitOfWork,
		IMapper mapper , 
		IUserRepository userRepository ,
		ITaskDomainService taskDomainService ,
		ILogger<CreateTaskHandler> logger)
	{
		_taskRepository = taskRepository;
		_unitOfWork = unitOfWork;
		_historyRepository = historyRepository;
		_mapper = mapper;
		_userRepository = userRepository;
		_taskDomainService = taskDomainService;
		_logger = logger;
	}
	public async Task<GetTasksDTO> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
	{
		var titleToCheck = request.Task.Title.ToNormalizedLower();

		if (await _taskRepository.ExistsAnyAsync(x => x.Title.ToLower() == titleToCheck))
		{
			_logger.LogWarning("Task title '{Title}' is duplicated.", request.Task.Title);
			throw new CustomDuplicateNameException("Task Title is duplicated");
		}

		var users = await _userRepository.GetUsersWithoutTasksAsync(cancellationToken);

		var task = _mapper.Map<Tasks>(request.Task);
		Users? selectedUser = null;

		if (users.Count() == 0)
		{
			task.State = TaskState.Waiting;
			_logger.LogInformation("No available users. Task set to Waiting.");
		}
		else
		{
			selectedUser = users.First();
			
			task.UserId = selectedUser.Id;
			task.State = TaskState.InProgress;
			_logger.LogInformation("Task assigned to user with ID: {UserId}", selectedUser.Id);
		}
		var result = await _taskRepository.AddAsync(task, cancellationToken);
		await _unitOfWork.CommitAsync(cancellationToken);
		_logger.LogInformation("Task created with ID: {TaskId}", result.Id);

		if (result.UserId.HasValue)
		{
		   await InsertIntoAssigmentHistory(result, selectedUser , cancellationToken);
		}
		return _mapper.Map<GetTasksDTO>(result);
	}
	private async Task InsertIntoAssigmentHistory(
		Tasks createdTask, 
		Users users , 
		CancellationToken cancellationToken)
	{
		_logger.LogInformation("Inserting assignment history for task ID: {TaskId}, user ID: {UserId}", createdTask.Id, users.Id);
		var historyEntry = _taskDomainService.CreateAssignmentHistory(createdTask , users);
		await _historyRepository.AddAsync(historyEntry, cancellationToken);
		await _unitOfWork.CommitAsync(cancellationToken);
		_logger.LogInformation("Assignment history inserted successfully.");
	}
}