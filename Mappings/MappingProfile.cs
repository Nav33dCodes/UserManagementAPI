using AutoMapper;
using UserManagementAPI.Models;
using UserManagementAPI.DTOs;

namespace UserManagementAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity → Response DTO
            CreateMap<User, UserDto>();

            // Create DTO → Entity
            CreateMap<CreateUserDto, User>();

            // Update DTO → Entity
            CreateMap<UpdateUserDto, User>();
        }
    }
}