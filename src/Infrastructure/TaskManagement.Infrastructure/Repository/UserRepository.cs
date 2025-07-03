

namespace TaskManagement.Infrastructure.Repository;
public class UserRepository : BaseRepository<Users>, IUserRepository
{
	private readonly IMapper _mapper;
	public UserRepository(AppDbContext appDbContext, IMapper mapper) : base(appDbContext)
	{
		_mapper = mapper;
	}
	public async Task<IEnumerable<GetUsersDTO>> GetUsersWithoutTasksAsync(CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.Where(u => !u.Tasks.Any())
			.ProjectTo<GetUsersDTO>(_mapper.ConfigurationProvider)
			.ToListAsync(cancellationToken);
	}
}