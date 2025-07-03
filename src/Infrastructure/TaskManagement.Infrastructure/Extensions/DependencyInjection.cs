namespace TaskManagement.Infrastructure.Extensions;
public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services , 
		IConfiguration configuration)
	{
		services.AddDbContext<AppDbContext>(options =>
	                   options.UseSqlite(configuration["ConnectionStrings:SQLiteDefault"]));

		services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<ITaskRepository, TaskRepository>();

		return services;
	}
}