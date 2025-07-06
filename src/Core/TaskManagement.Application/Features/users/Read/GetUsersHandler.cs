namespace TaskManagement.Application.Features.users.Read;
internal class GetUsersHandler : IRequestHandler<GetUsersQuery, PagedResponse<GetUsersDTO>>
{
	private readonly IUserRepository _usersRepository;
	private readonly IMapper _mapper;
	public GetUsersHandler(IUserRepository usersRepository, IMapper mapper)
	{
		_usersRepository = usersRepository;
		_mapper = mapper;
	}

	public async Task<PagedResponse<GetUsersDTO>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
	{
		var pagedUsers = await _usersRepository.GetAllWithPagingAsync(request.Parameters ,cancellationToken);
		var userDtos = _mapper.Map<IEnumerable<GetUsersDTO>>(pagedUsers);

		return new PagedResponse<GetUsersDTO>
		{
			Items = userDtos,
			MetaData = pagedUsers.MetaData
		};
	}
}