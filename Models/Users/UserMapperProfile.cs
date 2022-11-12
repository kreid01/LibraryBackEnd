using AutoMapper;

namespace LibrayBackEnd.Models.Users
{
    public class UserMapperProfile: Profile
    {
        public UserMapperProfile()
        {
            CreateMap<User, UserResponseDto>();
        }
    }
}
