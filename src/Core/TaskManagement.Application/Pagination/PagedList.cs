namespace TaskManagement.Application.Pagination;
public class PagedList<T> : List<T>
{
	public MetaData MetaData { get; private set; }
	public PagedList(List<T> items, int count, int pageNumber, int pageSize)
	{
		MetaData = new MetaData
		{
			TotalCount = count,
			CurrentPage = pageNumber,
			PageSize = pageSize,
			TotalPages = (int)Math.Ceiling(count / (double)pageSize)
		};

		AddRange(items);
	}
}