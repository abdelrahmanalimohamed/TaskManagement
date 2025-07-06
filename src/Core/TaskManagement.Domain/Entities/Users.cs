namespace TaskManagement.Domain.Entities;
public class Users : BaseEntity
{
	public string Name { get; set; }
	public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
	public ICollection<TaskAssignmentHistory> AssignmentHistory { get; set; } = new List<TaskAssignmentHistory>();
}