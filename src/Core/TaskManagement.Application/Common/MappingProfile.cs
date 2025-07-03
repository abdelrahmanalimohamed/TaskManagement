namespace TaskManagement.Application.Common;
public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<Users, GetUsersDTO>()
		   .ForMember(dest => dest.CreatedDate,
					  opt => opt.MapFrom(src => src.CreatedDate.ToString("yyyy-MM-dd HH:mm")));

		CreateMap<CreateUserDTO, Users>();
		CreateMap<CreateTaskDTO, Tasks>();

		CreateMap<Tasks, GetTasksDTO>()
			  .ForMember(dest => dest.CreatedDate,
						 opt => opt.MapFrom(src => src.CreatedDate.ToString("yyyy-MM-dd HH:mm")))
			  .ForMember(dest => dest.State,
						 opt => opt.MapFrom(src => src.State.ToString()))
				  .ForMember(dest => dest.UserName, 
							opt => opt.MapFrom(src => src.Users != null ? src.Users.Name : string.Empty));
	}
}