using CommunityEventsApi.DAL.Interfaces;
using CommunityEventsApi.Data;
using CommunityEventsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventsApi.DAL.Repositories
{
    public class CommunityRepository : GenericRepository<Community>, ICommunityRepository
    {
        public CommunityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Community>> GetCommunitiesByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(c => c.Users.Any(u => u.Id == userId))
                .Include(c => c.CreatedByNavigation)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

    }
}
