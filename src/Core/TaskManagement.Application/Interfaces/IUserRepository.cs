namespace TaskManagement.Application.Interfaces;
public interface IUserRepository : IBaseRepository<Users>
{
	Task<IEnumerable<Users>> GetUsersWithoutTasksAsync(CancellationToken cancellationToken = default);
}