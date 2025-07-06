namespace TaskManagement.Application.Features.tasks.Create;
public class CreateTaskCommand : IRequest<GetTasksDTO>
{
	public CreateTaskDTO Task { get; }
	public CreateTaskCommand(CreateTaskDTO TaskDTO)
	{
		Task = TaskDTO;
	}
}
