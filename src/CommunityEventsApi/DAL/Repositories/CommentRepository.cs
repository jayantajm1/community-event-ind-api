using CommunityEventsApi.DAL.Interfaces;
using CommunityEventsApi.Data;
using CommunityEventsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventsApi.DAL.Repositories;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    public CommentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Comment>> GetCommentsByEventAsync(Guid eventId)
    {
        return await _dbSet
            .Include(c => c.User)
            .Where(c => c.EventId == eventId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetCommentsByUserAsync(Guid userId)
    {
        return await _dbSet
            .Include(c => c.Event)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }
}
