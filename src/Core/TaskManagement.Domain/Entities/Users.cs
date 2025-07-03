namespace TaskManagement.Domain.Entities;
public class Users : BaseEntity
{
	public string Name { get; set; }
	public ICollection<Tasks> Tasks { get; set; } = new HashSet<Tasks>();
}