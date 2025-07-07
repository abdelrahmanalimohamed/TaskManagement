namespace TaskManagement.Application.Extensions;
public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		var assembly = typeof(DependencyInjection).Assembly;

		services.AddMediatR(configuration =>
		  configuration.RegisterServicesFromAssembly(assembly));

		services.AddValidatorsFromAssembly(assembly);

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

		services.AddAutoMapper(typeof(MappingProfile));

		services.AddScoped<ITaskReassignmentService, TaskReassignmentService>();

		services.AddScoped<ITaskDomainService, TaskDomainService>();
		return services;
	}
}