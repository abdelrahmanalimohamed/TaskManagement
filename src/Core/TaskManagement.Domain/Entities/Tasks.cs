namespace TaskManagement.Domain.Entities;
public class Tasks : BaseEntity
{
	public string Title { get; set; }
	public TaskState State { get; set; }
	public Guid? UserId { get; set; }
	public Users? Users { get; set; } 
}