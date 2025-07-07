namespace TaskManagement.Infrastructure.Repository;
public class BaseRepository<T> : IBaseRepository<T> where T : class
{
	private DbContext _appDbContext;
	protected readonly DbSet<T> _dbSet;
	public BaseRepository(DbContext appDbContext)
	{
		_appDbContext = appDbContext;
		_dbSet = _appDbContext.Set<T>();
	}
	public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
	{
		await _dbSet.AddAsync(entity, cancellationToken);
		return entity;
	}
	public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _dbSet.ToListAsync(cancellationToken);
	}	
	public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
	{
		_dbSet.Update(entity);
		return Task.CompletedTask;
	}
	public async Task<bool> ExistsAnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return await _dbSet.AnyAsync(predicate, cancellationToken);
	}
	public async Task<PagedList<T>> GetAllWithPagingAsync(
		RequestParameters requestParameters,
		Func<IQueryable<T>, IQueryable<T>> include,
		CancellationToken cancellationToken = default)
	{
		IQueryable<T> query = _dbSet;

		query = include(query);

		var count = await query.CountAsync(cancellationToken);
		var items = await query
			.Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
			.Take(requestParameters.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<T>(items, count, requestParameters.PageNumber, requestParameters.PageSize);
	}
	public async Task<PagedList<T>> GetAllWithPagingAsync(RequestParameters requestParameters, CancellationToken cancellationToken = default)
	{
		IQueryable<T> query = _dbSet;

		var count = await query.CountAsync(cancellationToken);
		var items = await query
			.Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
			.Take(requestParameters.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<T>(items, count, requestParameters.PageNumber, requestParameters.PageSize);
	}
}