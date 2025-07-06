namespace TaskManagement.Application.DTO;
public class CreateUserDTOValidator : AbstractValidator<CreateUserDTO>
{
	public CreateUserDTOValidator()
	{
		RuleFor(x => x.name)
			.NotEmpty().WithMessage("The user name is required.");
	}
}