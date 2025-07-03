namespace TaskManagement.Application.Features.users.Read;
public class GetUsersQuery : IRequest<PagedResponse<GetUsersDTO>>
{
	public RequestParameters Parameters { get; set; }
}