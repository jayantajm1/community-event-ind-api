using CommunityEventsApi.DTOs.Events;

namespace CommunityEventsApi.BAL.Interfaces;

public interface IEventService
{
    Task<IEnumerable<EventDto>> GetAllEventsAsync(EventFilterDto filter);
    Task<EventDto?> GetEventByIdAsync(Guid id);
    Task<EventDto> CreateEventAsync(CreateEventDto createDto, Guid userId);
    Task<EventDto> UpdateEventAsync(Guid id, CreateEventDto updateDto, Guid userId);
    Task DeleteEventAsync(Guid id, Guid userId);
    Task<bool> RegisterForEventAsync(Guid eventId, Guid userId);
    Task<bool> UnregisterFromEventAsync(Guid eventId, Guid userId);
    Task<IEnumerable<EventDto>> GetNearbyEventsAsync(double latitude, double longitude, double radiusKm);
}
