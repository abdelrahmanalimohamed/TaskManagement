namespace TaskManagement.Application.Features.users.Create;
internal class CreateUserHandler : IRequestHandler<CreateUserCommand , GetUsersDTO>
{
	private readonly IBaseRepository<Users>  _baseRepository;
	private readonly IMapper _mapper;
	public CreateUserHandler(IBaseRepository<Users> baseRepository, IMapper mapper)
	{
		_baseRepository = baseRepository;
		_mapper = mapper;
	}
	public async Task<GetUsersDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
	{
		string nameToCheck = request.User.name.Trim().ToLower();

		if (await _baseRepository.ExistsAnyAsync(x => x.Name.ToLower() == nameToCheck))
		{
			throw new CustomDuplicateNameException("User name is duplicated");
		}

		var userEntity = _mapper.Map<Users>(request.User);

		var createdUser = await _baseRepository.AddAsync(userEntity, cancellationToken);

		return _mapper.Map<GetUsersDTO>(createdUser);
	}
}