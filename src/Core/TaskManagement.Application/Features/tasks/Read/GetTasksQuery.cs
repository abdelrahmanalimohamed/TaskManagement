namespace TaskManagement.Application.Features.tasks.Read;
public class GetTasksQuery : IRequest<PagedResponse<GetTasksDTO>>
{
	public RequestParameters Parameters { get; set; }
}