namespace TaskManagement.Application.Interfaces;
public interface ITaskRepository : IBaseRepository<Tasks>
{
	Task<PagedList<Tasks>> GetAllWithUsersAsync(RequestParameters requestParameters, CancellationToken cancellationToken);
}