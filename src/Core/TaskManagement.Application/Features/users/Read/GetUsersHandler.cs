namespace TaskManagement.Application.Features.users.Read;
internal class GetUsersHandler : IRequestHandler<GetUsersQuery, PagedResponse<GetUsersDTO>>
{
	private readonly IUserRepository _usersRepository;
	private readonly IMapper _mapper;
	private readonly ILogger<GetUsersHandler> _logger;
	public GetUsersHandler(
		IUserRepository usersRepository, 
		IMapper mapper ,
		ILogger<GetUsersHandler> logger)
	{
		_usersRepository = usersRepository;
		_mapper = mapper;
		_logger = logger;
	}

	public async Task<PagedResponse<GetUsersDTO>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
	{
		var pagedUsers = await _usersRepository.GetAllWithPagingAsync(request.Parameters ,cancellationToken);
		var userDtos = _mapper.Map<IEnumerable<GetUsersDTO>>(pagedUsers);

		_logger.LogInformation("Retrieved {Count} users from database.", pagedUsers.Count());

		return new PagedResponse<GetUsersDTO>
		{
			Items = userDtos,
			MetaData = pagedUsers.MetaData
		};
	}
}