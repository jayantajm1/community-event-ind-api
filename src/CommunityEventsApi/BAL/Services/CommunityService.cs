using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.DAL.Interfaces;
using CommunityEventsApi.Models;

namespace CommunityEventsApi.BAL.Services;

public class CommunityService : ICommunityService
{
    private readonly ICommunityRepository _communityRepository;

    public CommunityService(ICommunityRepository communityRepository)
    {
        _communityRepository = communityRepository;
    }

    public async Task<IEnumerable<Community>> GetAllCommunitiesAsync()
    {
        return await _communityRepository.GetAllAsync();
    }

    public async Task<Community?> GetCommunityByIdAsync(Guid id)
    {
        return await _communityRepository.GetByIdAsync(id);
    }

    public async Task<Community> CreateCommunityAsync(Community community)
    {
        community.Id = Guid.NewGuid();
        community.CreatedAt = DateTime.UtcNow;
        return await _communityRepository.AddAsync(community);
    }

    public async Task<Community> UpdateCommunityAsync(Guid id, Community community)
    {
        var existingCommunity = await _communityRepository.GetByIdAsync(id);

        if (existingCommunity == null)
        {
            throw new KeyNotFoundException("Community not found");
        }

        existingCommunity.Name = community.Name;
        existingCommunity.Description = community.Description;
        existingCommunity.ImageUrl = community.ImageUrl;
        existingCommunity.UpdatedAt = DateTime.UtcNow;

        await _communityRepository.UpdateAsync(existingCommunity);
        return existingCommunity;
    }

    public async Task DeleteCommunityAsync(Guid id)
    {
        var community = await _communityRepository.GetByIdAsync(id);

        if (community == null)
        {
            throw new KeyNotFoundException("Community not found");
        }

        await _communityRepository.DeleteAsync(community);
    }

    public async Task<IEnumerable<Community>> GetUserCommunitiesAsync(Guid userId)
    {
        return await _communityRepository.GetCommunitiesByUserAsync(userId);
    }
}
