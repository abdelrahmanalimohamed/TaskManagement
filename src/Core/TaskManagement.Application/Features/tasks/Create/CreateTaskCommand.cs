namespace TaskManagement.Application.Features.tasks.Create;
public class CreateTaskCommand : IRequest<GetTasksDTO>
{
	public CreateTaskDTO createTaskDTO { get; }
	public CreateTaskCommand(CreateTaskDTO TaskDTO)
	{
		createTaskDTO = TaskDTO;
	}
}
