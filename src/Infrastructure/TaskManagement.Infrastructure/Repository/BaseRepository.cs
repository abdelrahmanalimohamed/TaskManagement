namespace TaskManagement.Infrastructure.Repository;
public class BaseRepository<T> : IBaseRepository<T> where T : class
{
	private AppDbContext _appDbContext;
	protected readonly DbSet<T> _dbSet;
	public BaseRepository(AppDbContext appDbContext)
	{
		_appDbContext = appDbContext;
		_dbSet = _appDbContext.Set<T>();
	}
	public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
	{
		await _appDbContext.Set<T>().AddAsync(entity, cancellationToken);
		await _appDbContext.SaveChangesAsync(cancellationToken);
		return entity;
	}
	public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
	{
		_appDbContext.Set<T>().Remove(entity);
		await _appDbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task<bool> ExistsAsync<TProperty>(
		Expression<Func<T, TProperty>> propertySelector, 
		TProperty value)
	{
		var parameter = Expression.Parameter(typeof(T), "x");
		var property = Expression.Invoke(propertySelector, parameter);

		Expression propertyExpr = property;
		Expression valueExpr = Expression.Constant(value, typeof(TProperty));

		if (typeof(TProperty) == typeof(string))
		{
			var trimMethod = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes);
			propertyExpr = Expression.Call(property, trimMethod);

			if (value is string s)
			{
				valueExpr = Expression.Constant(s.Trim(), typeof(TProperty));
			}
		}

		var equality = Expression.Equal(propertyExpr, valueExpr);
		var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

		return await _dbSet.AnyAsync(lambda);
	}
	public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _appDbContext.Set<T>().ToListAsync(cancellationToken);
	}	
	public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
	{
		_appDbContext.Entry(entity).State = EntityState.Modified;
		await _appDbContext.SaveChangesAsync(cancellationToken);
	}
	public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return await _appDbContext.Set<T>().Where(predicate).ToListAsync(cancellationToken);
	}
	public async Task<bool> ExistsAnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return await _dbSet.AnyAsync(predicate, cancellationToken);
	}
	public async Task<PagedList<T>> GetAllAsync(
		RequestParameters requestParameters, 
		CancellationToken cancellationToken = default, 
		Func<IQueryable<T>, IQueryable<T>> include = null)
	{
		IQueryable<T> query = _dbSet;

		if (include != null)
			query = include(query);

		var count = await query.CountAsync(cancellationToken);
		var items = await query
			.Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
			.Take(requestParameters.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<T>(items, count, requestParameters.PageNumber, requestParameters.PageSize);
	}
}