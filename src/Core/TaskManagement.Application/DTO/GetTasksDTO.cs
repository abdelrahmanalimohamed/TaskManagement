namespace TaskManagement.Application.DTO;
public class GetTasksDTO
{
	public string Title { get; init; }
	public string AssignedUser { get; init; }
	public string CreatedDate { get; init; }
	public string State { get; init; }
}