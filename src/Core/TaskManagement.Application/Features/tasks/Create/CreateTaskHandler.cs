namespace TaskManagement.Application.Features.tasks.Create;
internal class CreateTaskHandler : IRequestHandler<CreateTaskCommand , GetTasksDTO>
{
	private readonly IBaseRepository<Tasks> _baseRepository;
	private readonly IUserRepository _userRepository;
	private IMapper _mapper;
	public CreateTaskHandler(
		IBaseRepository<Tasks> baseRepository, 
		IMapper mapper , 
		IUserRepository userRepository)
	{
		_baseRepository = baseRepository;
		_mapper = mapper;
		_userRepository = userRepository;
	}
	public async Task<GetTasksDTO> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
	{
		var titleToCheck = request.createTaskDTO.Title.Trim().ToLower();

		if (await _baseRepository.ExistsAnyAsync(x => x.Title.ToLower() == titleToCheck))
		{
			throw new CustomDuplicateNameException("Task Title is duplicated");
		}

		var users = await _userRepository.GetUsersWithoutTasksAsync(cancellationToken);

		var task = _mapper.Map<Tasks>(request.createTaskDTO);

		if (users.Count() == 0)
		{
			task.State = TaskState.Waiting;
		}
		else
		{
			var user = users.FirstOrDefault();
			
			task.UserId = user.Id;
			task.State = TaskState.InProgress;
		}
		var result = await _baseRepository.AddAsync(task, cancellationToken);

		return _mapper.Map<GetTasksDTO>(result);
	}
}
