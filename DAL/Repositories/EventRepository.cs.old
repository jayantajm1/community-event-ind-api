using CommunityEventsApi.DAL.Interfaces;
using CommunityEventsApi.Data;
using CommunityEventsApi.Helpers;
using CommunityEventsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventsApi.DAL.Repositories;

public class EventRepository : GenericRepository<Event>, IEventRepository
{
    public EventRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Event>> GetEventsByCommunityAsync(Guid communityId)
    {
        return await _dbSet
            .Include(e => e.Location)
            .Include(e => e.Category)
            .Include(e => e.Creator)
            .Where(e => e.CommunityId == communityId)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
    {
        return await _dbSet
            .Include(e => e.Location)
            .Include(e => e.Category)
            .Include(e => e.Community)
            .Include(e => e.Creator)
            .Where(e => e.StartDate > DateTime.UtcNow && e.Status == AppConstants.EventStatusUpcoming)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetNearbyEventsAsync(double latitude, double longitude, double radiusKm)
    {
        var events = await _dbSet
            .Include(e => e.Location)
            .Include(e => e.Category)
            .Include(e => e.Community)
            .Include(e => e.Creator)
            .Where(e => e.StartDate > DateTime.UtcNow)
            .ToListAsync();

        return events.Where(e =>
            GeoUtils.IsWithinRadius(
                latitude, longitude,
                e.Location.Latitude, e.Location.Longitude,
                radiusKm))
            .OrderBy(e => e.StartDate);
    }

    public async Task<IEnumerable<Event>> GetEventsByUserAsync(Guid userId)
    {
        return await _context.EventRegistrations
            .Where(er => er.UserId == userId && er.Status == "Registered")
            .Include(er => er.Event)
                .ThenInclude(e => e.Location)
            .Include(er => er.Event)
                .ThenInclude(e => e.Community)
            .Select(er => er.Event)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }
}
