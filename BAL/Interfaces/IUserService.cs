using CommunityEventsApi.DTOs.Users;

namespace CommunityEventsApi.BAL.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> UpdateProfileAsync(Guid userId, UpdateProfileDto updateDto);
    Task DeleteUserAsync(Guid userId);
}
