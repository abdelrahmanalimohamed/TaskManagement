namespace TaskManagement.Application.DTO;
public class CreateTaskDTOValidator : AbstractValidator<CreateTaskDTO>
{
	public CreateTaskDTOValidator() 
	{
		RuleFor(x => x.Title)
		.NotEmpty().WithMessage("The task name is required.");
	}
}