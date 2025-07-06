namespace TaskManagement.Domain.Common;
public class PagedResponse<T>
{
	public IEnumerable<T> Items { get; set; }
	public MetaData MetaData { get; set; }
}