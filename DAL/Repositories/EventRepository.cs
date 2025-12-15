using CommunityEventsApi.DAL.Interfaces;
using CommunityEventsApi.Data;
using CommunityEventsApi.Helpers;
using CommunityEventsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventsApi.DAL.Repositories
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(ApplicationDbContext context) : base(context)
        {
        }

        // 🔹 Events by Community
        public async Task<IEnumerable<Event>> GetEventsByCommunityAsync(Guid communityId)
        {
            return await _dbSet
                .Include(e => e.Community)
                .Include(e => e.Organizer)
                .Where(e => e.CommunityId == communityId)
                .OrderBy(e => e.StartDatetime)
                .ToListAsync();
        }

        // 🔹 Upcoming Events (Global)
        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
        {
            return await _dbSet
                .Include(e => e.Community)
                .Include(e => e.Organizer)
                .Where(e =>
                    e.StartDatetime > DateTime.UtcNow &&
                    e.Status == AppConstants.EventStatusUpcoming)
                .OrderBy(e => e.StartDatetime)
                .ToListAsync();
        }

        // 🔹 Nearby Events (uses LocationName + Address only)
        public async Task<IEnumerable<Event>> GetNearbyEventsAsync(
            double latitude,
            double longitude,
            double radiusKm)
        {
            // ⚠ Your entity does NOT store lat/long
            // So nearby logic cannot be DB-side

            var events = await _dbSet
                .Include(e => e.Community)
                .Include(e => e.Organizer)
                .Where(e => e.StartDatetime > DateTime.UtcNow)
                .ToListAsync();

            // ❗ Placeholder until geo fields exist
            return events
                .OrderBy(e => e.StartDatetime);
        }

        // 🔹 Events registered by a User
        public async Task<IEnumerable<Event>> GetEventsByUserAsync(Guid userId)
        {
            return await _context.Registrations
                .Where(r => r.UserId == userId)
                .Include(r => r.Event)
                    .ThenInclude(e => e.Community)
                .Include(r => r.Event)
                    .ThenInclude(e => e.Organizer)
                .Select(r => r.Event)
                .OrderBy(e => e.StartDatetime)
                .ToListAsync();
        }
    }
}
