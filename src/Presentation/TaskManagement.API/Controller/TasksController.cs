

namespace TaskManagement.API.Controller;

[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
	private readonly IMediator _mediator;
	public TasksController(IMediator mediator)
	{
		_mediator = mediator;
	}
	[HttpPost]
	[Route("create")]
	public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO createTaskDTO, CancellationToken cancellationToken)
	{
		if (createTaskDTO == null)
		{
			return BadRequest("task details cannot be null");
		}
		var command = new CreateTaskCommand(createTaskDTO);
		var result = await _mediator.Send(command, cancellationToken);

		return Ok(result);
	}

	[HttpGet]
	[Route("get-all")]
	public async Task<IActionResult> GetAllUsers(
			[FromQuery] RequestParameters parameters,
			CancellationToken cancellationToken)
	{
		var query = new GetTasksQuery { Parameters = parameters };
		var response = await _mediator.Send(query, cancellationToken);

		return Ok(response);
	}
}