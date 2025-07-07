namespace TaskManagement.API.EndPoints;
public class TasksModule : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("api/tasks/create", async (
		IMediator mediator,
		CreateTaskDTO createTaskDTO,
		CancellationToken cancellationToken) =>
		{
			var command = new CreateTaskCommand(createTaskDTO);
			var result = await mediator.Send(command, cancellationToken);
			return Results.Ok(result);
		});

		app.MapGet("api/tasks/get-all", async (
			IMediator mediator,
			[AsParameters] RequestParameters parameters,
			CancellationToken cancellationToken) =>
		{
			var query = new GetTasksQuery { Parameters = parameters };
			var response = await mediator.Send(query, cancellationToken);
			return Results.Ok(response);
		});
	}
}
