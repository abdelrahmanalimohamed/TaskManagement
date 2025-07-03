namespace TaskManagement.Application.Interfaces;
public interface IBaseRepository <T> where T : class
{
	Task<PagedList<T>> GetAllAsync(
		RequestParameters requestParameters,
		CancellationToken cancellationToken = default ,
		Func<IQueryable<T>, IQueryable<T>> include = null);

	Task<bool> ExistsAsync<TProperty>(Expression<Func<T, TProperty>> propertySelector, TProperty value);
	Task<bool> ExistsAnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
	Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
	Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
	Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
	Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
