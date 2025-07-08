namespace TaskManagement.Application.Features.tasks.Read;
internal class GetTasksHandler : IRequestHandler<GetTasksQuery, PagedResponse<GetTasksDTO>>
{
	private readonly ITaskRepository _tasksRepository;
	private readonly IMapper _mapper;
	private readonly ILogger<GetTasksHandler> _logger;
	public GetTasksHandler(
		ITaskRepository tasksRepository, 
		IMapper mapper ,
		ILogger<GetTasksHandler> logger)
	{
		_tasksRepository = tasksRepository;
		_mapper = mapper;
		_logger = logger;
	}
	public async Task<PagedResponse<GetTasksDTO>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
	{
		var pagedTasks = await _tasksRepository.GetAllWithUsersAsync(request.Parameters,cancellationToken);
		var tasksDTO = _mapper.Map<IEnumerable<GetTasksDTO>>(pagedTasks);

		_logger.LogInformation("Retrieved {Count} tasks from database.", pagedTasks.Count());

		return new PagedResponse<GetTasksDTO>
		{
			Items = tasksDTO,
			MetaData = pagedTasks.MetaData
		};
	}
}