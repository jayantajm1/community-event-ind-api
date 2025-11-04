using CommunityEventsApi.Models;

namespace CommunityEventsApi.DAL.Interfaces;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<IEnumerable<Comment>> GetCommentsByEventAsync(Guid eventId);
    Task<IEnumerable<Comment>> GetCommentsByUserAsync(Guid userId);
}
