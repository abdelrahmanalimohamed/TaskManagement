namespace TaskManagement.API;
public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddAuthorization();

		builder.Services.AddControllers();

		builder.Services.AddApplication()
						.AddInfrastructure(builder.Configuration);

		var app = builder.Build();

		// Configure the HTTP request pipeline.

		app.UseHttpsRedirection();

		app.UseAuthorization();
		app.MapControllers();

		app.UseMiddleware<ExceptionHandling>();

		app.Run();
	}
}
