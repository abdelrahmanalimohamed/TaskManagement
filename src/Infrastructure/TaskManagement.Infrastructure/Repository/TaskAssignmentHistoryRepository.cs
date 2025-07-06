namespace TaskManagement.Infrastructure.Repository;
public class TaskAssignmentHistoryRepository : BaseRepository<TaskAssignmentHistory>, ITaskAssignmentHistoryRepository
{
	public TaskAssignmentHistoryRepository(AppDbContext appDbContext) : base(appDbContext)
	{
	}
}