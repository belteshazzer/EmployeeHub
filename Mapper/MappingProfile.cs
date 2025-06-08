using AutoMapper;
using EmployeeHub.Models.Dtos;
using EmployeeHub.Models.Entities;

namespace EmployeeHub.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map RegisterRequestDto to User entity
            CreateMap<User, RegisterRequestDto>().ReverseMap();

            // Map ChatDto to Chat entity
            CreateMap<Chat, ChatDto>().ReverseMap();

            // Map Chat entity to ChatDto
            CreateMap<ChatHistory, ChatHistoryDto>().ReverseMap();

            CreateMap<Departments, DepartmentDto>().ReverseMap();

            CreateMap<Roles, RoleDto>().ReverseMap();

        }
    }
}