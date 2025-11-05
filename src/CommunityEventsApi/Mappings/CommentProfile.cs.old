using AutoMapper;
using CommunityEventsApi.DTOs.Comments;
using CommunityEventsApi.Models;

namespace CommunityEventsApi.Mappings;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
            .ForMember(dest => dest.UserProfileImage, opt => opt.MapFrom(src => src.User.ProfileImageUrl));

        CreateMap<CreateCommentDto, Comment>();
    }
}
