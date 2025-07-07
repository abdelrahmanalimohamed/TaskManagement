using TaskManagement.Infrastructure.Seed;

namespace TaskManagement.Infrastructure.Extensions;
public static class DependencyInjection
{
	public static async Task<IServiceCollection> AddInfrastructure(
		this IServiceCollection services , 
		IConfiguration configuration)
	{
		services.AddDbContext<DbContext, AppDbContext>(options =>
	                   options.UseSqlite(configuration["ConnectionStrings:SQLiteDefault"]));

		services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<ITaskRepository, TaskRepository>();
		services.AddScoped<ITaskAssignmentHistoryRepository, TaskAssignmentHistoryRepository>();
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		services.AddHostedService<TaskReassignmentBackgroundService>();
		await SeedData.InitializeAsync(services.BuildServiceProvider());
		return services;
	}
}