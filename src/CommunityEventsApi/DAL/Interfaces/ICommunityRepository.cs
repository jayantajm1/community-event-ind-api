using CommunityEventsApi.Models;

namespace CommunityEventsApi.DAL.Interfaces;

public interface ICommunityRepository : IGenericRepository<Community>
{
    Task<IEnumerable<Community>> GetCommunitiesByUserAsync(Guid userId);
}
