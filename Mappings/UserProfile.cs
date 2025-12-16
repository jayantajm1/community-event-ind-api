using AutoMapper;
using CommunityEventsApi.DTOs.Auth;
using CommunityEventsApi.DTOs.Users;
using CommunityEventsApi.Models;

namespace CommunityEventsApi.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FullName.Split(new[] { ' ' }).FirstOrDefault() ?? ""))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => string.Join(" ", src.FullName.Split(new[] { ' ' }).Skip(1))))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.AvatarUrl));

        CreateMap<SignupRequestDto, User>();
    }
}
