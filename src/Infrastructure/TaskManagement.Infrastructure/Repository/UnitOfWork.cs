namespace TaskManagement.Infrastructure.Repository;
public class UnitOfWork : IUnitOfWork
{
	private readonly DbContext _appDbContext;
	public UnitOfWork (DbContext appDbContext)
	{
		_appDbContext = appDbContext;
	}
	public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
	{
		return await _appDbContext.SaveChangesAsync(cancellationToken);
	}
	public void Dispose()
	{
		_appDbContext.Dispose();
	}
}