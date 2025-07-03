namespace TaskManagement.Application.Features.users.Create;
public class CreateUserCommand : IRequest<GetUsersDTO>
{
	public CreateUserDTO User { get; }
	public CreateUserCommand(CreateUserDTO user)
	{
		User = user;
	}
}