namespace TaskManagement.Infrastructure.Seed;
public static class SeedData
{
	public static async Task InitializeAsync(IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var scopedServices = scope.ServiceProvider;

		var unitOfWork = scopedServices.GetRequiredService<IUnitOfWork>();
		var userRepository = scopedServices.GetRequiredService<IUserRepository>();
		var taskRepository = scopedServices.GetRequiredService<ITaskRepository>();

		var cancellationToken = CancellationToken.None;

		if (!await userRepository.ExistsAnyAsync(_ => true, cancellationToken))
		{
			var users = new List<Users>
			{
				new Users { Name = "User 1" },
				new Users { Name = "User 2" }
			};

			foreach (var user in users)
			{
				await userRepository.AddAsync(user, cancellationToken);
			}

			await unitOfWork.CommitAsync(cancellationToken);
		}

		if (!await taskRepository.ExistsAnyAsync(_ => true, cancellationToken))
		{
				var tasks = new List<Tasks>
				{
					new Tasks { Title = "First Task", State = TaskState.Waiting},
					new Tasks { Title = "Second Task", State = TaskState.Waiting}
				};

				foreach (var task in tasks)
				{
					await taskRepository.AddAsync(task, cancellationToken);
				}

				await unitOfWork.CommitAsync(cancellationToken);
		}
	}
}