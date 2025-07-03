using TaskManagement.Application.Pagination;

namespace TaskManagement.Application.Features.users.Read;
internal class GetUsersHandler : IRequestHandler<GetUsersQuery, PagedResponse<GetUsersDTO>>
{
	private readonly IBaseRepository<Users> _usersRepository;
	private readonly IMapper _mapper;
	public GetUsersHandler(IBaseRepository<Users> usersRepository, IMapper mapper)
	{
		_usersRepository = usersRepository;
		_mapper = mapper;
	}

	public async Task<PagedResponse<GetUsersDTO>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
	{
		var pagedUsers = await _usersRepository.GetAllAsync(request.Parameters ,cancellationToken);
		var userDtos = _mapper.Map<IEnumerable<GetUsersDTO>>(pagedUsers);

		return new PagedResponse<GetUsersDTO>
		{
			Items = userDtos,
			MetaData = pagedUsers.MetaData
		};
	}
}