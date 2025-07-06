namespace TaskManagement.Domain.Entities;
public class TaskAssignmentHistory : BaseEntity
{
	public Guid TaskId { get; set; }
	public Tasks Task { get; set; }

	public Guid UserId { get; set; }
	public Users User { get; set; }
}