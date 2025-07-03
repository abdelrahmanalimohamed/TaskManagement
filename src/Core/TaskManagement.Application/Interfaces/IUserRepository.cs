namespace TaskManagement.Application.Interfaces;
public interface IUserRepository : IBaseRepository<Users>
{
	Task<IEnumerable<GetUsersDTO>> GetUsersWithoutTasksAsync(CancellationToken cancellationToken = default);
}