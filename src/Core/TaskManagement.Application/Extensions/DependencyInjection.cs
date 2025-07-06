

namespace TaskManagement.Application.Extensions;
public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		var assembly = typeof(DependencyInjection).Assembly;

		services.AddMediatR(configuration =>
		  configuration.RegisterServicesFromAssembly(assembly));

		services.AddAutoMapper(typeof(MappingProfile));
		services.AddScoped<ITaskReassignmentService , TaskReassignmentService>();
		return services;
	}
}