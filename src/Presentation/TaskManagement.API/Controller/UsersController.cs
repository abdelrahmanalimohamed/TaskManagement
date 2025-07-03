using TaskManagement.Application.Common;

namespace TaskManagement.API.Controller;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	private IMediator _mediator;
	public UsersController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost("create")]
	public async Task<IActionResult> CreateUser(
		[FromBody] CreateUserDTO createUserDTO, 
		CancellationToken cancellationToken)
	{
		if (createUserDTO == null || string.IsNullOrWhiteSpace(createUserDTO.name))
		{
			return BadRequest("Name cannot be null.");
		}
		var command = new CreateUserCommand(createUserDTO);

		var result = await _mediator.Send(command, cancellationToken);

		return Ok(result);
	}

	[HttpGet("get-all")]
	public async Task<IActionResult> GetAllUsers(
		[FromQuery] RequestParameters parameters, 
		CancellationToken cancellationToken)
	{
		var query = new GetUsersQuery { Parameters = parameters };
		var response = await _mediator.Send(query, cancellationToken);

		return Ok(response);
	}
}
