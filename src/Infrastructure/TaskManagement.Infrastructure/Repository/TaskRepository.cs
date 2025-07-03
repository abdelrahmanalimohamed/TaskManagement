namespace TaskManagement.Infrastructure.Repository;
public class TaskRepository : BaseRepository<Tasks>, ITaskRepository
{
	public TaskRepository(AppDbContext appDbContext) : base(appDbContext)
	{
	}
	public async Task<PagedList<Tasks>> GetAllWithUsersAsync(RequestParameters requestParameters, CancellationToken cancellationToken)
	{
		var x = await GetAllAsync(requestParameters, cancellationToken,
			include: q => q.Include(t => t.Users));

		return x;
	}
}
