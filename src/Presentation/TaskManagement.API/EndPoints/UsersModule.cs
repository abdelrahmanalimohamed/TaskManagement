namespace TaskManagement.API.EndPoints;
public class UsersModule : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("api/users/create", async (
		IMediator mediator,
		CreateUserDTO createUserDTO,
		CancellationToken cancellationToken) =>
		{
			var command = new CreateUserCommand(createUserDTO);
			var result = await mediator.Send(command, cancellationToken);
			return Results.Ok(result);
		});

		app.MapGet("api/users/get-all", async (
			IMediator mediator,
			[AsParameters] RequestParameters parameters,
			CancellationToken cancellationToken) =>
		{
			var query = new GetUsersQuery { Parameters = parameters };
			var response = await mediator.Send(query, cancellationToken);
			return Results.Ok(response);
		});
	}
}
