using CommunityEventsApi.Models;

namespace CommunityEventsApi.DAL.Interfaces;

public interface IEventRepository : IGenericRepository<Event>
{
    Task<IEnumerable<Event>> GetEventsByCommunityAsync(Guid communityId);
    Task<IEnumerable<Event>> GetUpcomingEventsAsync();
    Task<IEnumerable<Event>> GetNearbyEventsAsync(double latitude, double longitude, double radiusKm);
    Task<IEnumerable<Event>> GetEventsByUserAsync(Guid userId);
}
