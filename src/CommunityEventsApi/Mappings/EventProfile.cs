using AutoMapper;
using CommunityEventsApi.DTOs.Events;
using CommunityEventsApi.Models;

namespace CommunityEventsApi.Mappings;

public class EventProfile : Profile
{
    public EventProfile()
    {
        CreateMap<Event, EventDto>()
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.Longitude))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Location.Address))
            .ForMember(dest => dest.CommunityName, opt => opt.MapFrom(src => src.Community.Name))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.Creator.FirstName + " " + src.Creator.LastName));

        CreateMap<CreateEventDto, Event>();
    }
}
