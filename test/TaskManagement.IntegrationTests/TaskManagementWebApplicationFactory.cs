using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagement.API;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.IntegrationTests;

public class TaskManagementWebApplicationFactory : WebApplicationFactory<Program>
{
	private SqliteConnection _sqliteConnection;
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureTestServices(services =>
		{
			services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

			var _sqliteConnection = new SqliteConnection("DataSource=:memory:");
			_sqliteConnection.Open();

			services.AddDbContext<AppDbContext>(options =>
				options.UseSqlite(_sqliteConnection));

			var serviceProvider = services.BuildServiceProvider();
			using (var scope = serviceProvider.CreateScope())
			{
				var scopedServices = scope.ServiceProvider;
				var dbContext = scopedServices.GetRequiredService<AppDbContext>();

				dbContext.Database.Migrate();
			}
		});
	}
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_sqliteConnection?.Close(); 
			_sqliteConnection?.Dispose(); 
		}
		base.Dispose(disposing); // Call the base class Dispose method
	}
}
