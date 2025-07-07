namespace TaskManagement.Application.Interfaces;
public interface IBaseRepository <T> where T : class
{
	Task<PagedList<T>> GetAllWithPagingAsync(
		RequestParameters requestParameters,
		Func<IQueryable<T>, IQueryable<T>> include,
		CancellationToken cancellationToken = default );
	Task<PagedList<T>> GetAllWithPagingAsync(
		RequestParameters requestParameters,
		CancellationToken cancellationToken = default);
	Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<bool> ExistsAnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
	Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
	Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
}