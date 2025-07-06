namespace TaskManagement.Domain.Base;
public abstract class BaseEntity
{
	public Guid Id { get; private set; } = Guid.NewGuid();
	public DateTime CreatedDate { get; private set; } = DateTime.Now;
}