namespace TaskManagement.Infrastructure.Repository;
public class UserRepository : BaseRepository<Users>, IUserRepository
{
	public UserRepository(AppDbContext appDbContext) 
		: base(appDbContext)
	{
	}
	public async Task<IEnumerable<Users>> GetUsersWithoutTasksAsync(CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.Where(u => !u.Tasks.Any(t => t.State == TaskState.InProgress))
			.ToListAsync(cancellationToken);
	}
}