namespace TaskManagement.Infrastructure.Repository;
public class TaskRepository : BaseRepository<Tasks>, ITaskRepository
{
	public TaskRepository(DbContext appDbContext) 
		: base(appDbContext)
	{
	}
	public async Task<List<Tasks>> GetAllPendingTasksAsync(CancellationToken cancellationToken)
	{
		return await _dbSet
			.Include(t => t.AssignmentHistory)
			.Where(t => t.State != TaskState.Completed)
			.ToListAsync(cancellationToken);
	}
	public async Task<PagedList<Tasks>> GetAllWithUsersAsync(
		RequestParameters requestParameters, 
		CancellationToken cancellationToken)
	{
		return await GetAllWithPagingAsync(
			requestParameters, 
			include: q => q.Include(t => t.Users), 
			cancellationToken);
	}
}