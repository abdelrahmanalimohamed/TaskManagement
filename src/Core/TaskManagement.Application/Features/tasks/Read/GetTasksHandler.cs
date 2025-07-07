namespace TaskManagement.Application.Features.tasks.Read;
internal class GetTasksHandler : IRequestHandler<GetTasksQuery, PagedResponse<GetTasksDTO>>
{
	private readonly ITaskRepository _tasksRepository;
	private readonly IMapper _mapper;
	public GetTasksHandler(
		ITaskRepository tasksRepository, 
		IMapper mapper)
	{
		_tasksRepository = tasksRepository;
		_mapper = mapper;
	}
	public async Task<PagedResponse<GetTasksDTO>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
	{
		var pagedTasks = await _tasksRepository.GetAllWithUsersAsync(request.Parameters,cancellationToken);
		var tasksDTO = _mapper.Map<IEnumerable<GetTasksDTO>>(pagedTasks);

		return new PagedResponse<GetTasksDTO>
		{
			Items = tasksDTO,
			MetaData = pagedTasks.MetaData
		};
	}
}