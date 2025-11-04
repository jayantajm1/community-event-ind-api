using CommunityEventsApi.Models;

namespace CommunityEventsApi.BAL.Interfaces;

public interface ICommunityService
{
    Task<IEnumerable<Community>> GetAllCommunitiesAsync();
    Task<Community?> GetCommunityByIdAsync(Guid id);
    Task<Community> CreateCommunityAsync(Community community);
    Task<Community> UpdateCommunityAsync(Guid id, Community community);
    Task DeleteCommunityAsync(Guid id);
    Task<IEnumerable<Community>> GetUserCommunitiesAsync(Guid userId);
}
