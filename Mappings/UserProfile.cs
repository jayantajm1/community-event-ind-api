using AutoMapper;
using CommunityEventsApi.DTOs.Auth;
using CommunityEventsApi.DTOs.Users;
using CommunityEventsApi.Models;

namespace CommunityEventsApi.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<SignupRequestDto, User>();
    }
}
