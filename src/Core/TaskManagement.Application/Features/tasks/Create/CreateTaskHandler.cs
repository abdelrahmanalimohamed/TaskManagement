namespace TaskManagement.Application.Features.tasks.Create;
internal class CreateTaskHandler : IRequestHandler<CreateTaskCommand , GetTasksDTO>
{
	private readonly ITaskRepository _taskRepository;
	private readonly IBaseRepository<TaskAssignmentHistory> _historyRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserRepository _userRepository;
	private IMapper _mapper;
	public CreateTaskHandler(
		ITaskRepository taskRepository, 
		IBaseRepository<TaskAssignmentHistory> historyRepository,
		IUnitOfWork unitOfWork,
		IMapper mapper , 
		IUserRepository userRepository)
	{
		_taskRepository = taskRepository;
		_unitOfWork = unitOfWork;
		_historyRepository = historyRepository;
		_mapper = mapper;
		_userRepository = userRepository;
	}
	public async Task<GetTasksDTO> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
	{
		var titleToCheck = request.createTaskDTO.Title.Trim().ToLower();

		if (await _taskRepository.ExistsAnyAsync(x => x.Title.ToLower() == titleToCheck))
		{
			throw new CustomDuplicateNameException("Task Title is duplicated");
		}

		var users = await _userRepository.GetUsersWithoutTasksAsync(cancellationToken);

		var task = _mapper.Map<Tasks>(request.createTaskDTO);
		Users? selectedUser = null;

		if (users.Count() == 0)
		{
			task.State = TaskState.Waiting;
		}
		else
		{
			selectedUser = users.First();
			
			task.UserId = selectedUser.Id;
			task.State = TaskState.InProgress;
		}
		var result = await _taskRepository.AddAsync(task, cancellationToken);
		await _unitOfWork.CommitAsync(cancellationToken);

		if (result.UserId.HasValue)
		{
		   await InsertIntoAssigmentHistory(result, selectedUser , cancellationToken);
		}
		return _mapper.Map<GetTasksDTO>(result);
	}
	private async Task InsertIntoAssigmentHistory(Tasks createdTask, Users users , CancellationToken cancellationToken)
	{
		var historyEntry = createdTask.CreateAssignmentHistory(users);
		await _historyRepository.AddAsync(historyEntry, cancellationToken);
		await _unitOfWork.CommitAsync(cancellationToken);
	}
}