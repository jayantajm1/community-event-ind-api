using CommunityEventsApi.DAL.Interfaces;
using CommunityEventsApi.Data;
using CommunityEventsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventsApi.DAL.Repositories;

public class CommunityRepository : GenericRepository<Community>, ICommunityRepository
{
    public CommunityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Community>> GetCommunitiesByUserAsync(Guid userId)
    {
        return await _context.Memberships
            .Where(m => m.UserId == userId && m.LeftAt == null)
            .Include(m => m.Community)
            .Select(m => m.Community)
            .ToListAsync();
    }
}
