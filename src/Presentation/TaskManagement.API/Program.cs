

namespace TaskManagement.API;
public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddAuthorization();

		builder.Services.AddControllers();

		builder.Services.AddApplication()
						.AddInfrastructure(builder.Configuration);

		builder.Services.AddCarter();

		var app = builder.Build();

		using (var scope = app.Services.CreateScope())
		{
			var services = scope.ServiceProvider;
			await SeedData.InitializeAsync(services);
		}

		app.UseHttpsRedirection();

		app.UseAuthorization();
		app.MapControllers();

		app.MapCarter();
		app.UseMiddleware<ExceptionHandling>();

		app.Run();
	}
}
