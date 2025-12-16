using AutoMapper;
using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.DAL.Interfaces;
using CommunityEventsApi.DTOs.Users;

namespace CommunityEventsApi.BAL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> UpdateProfileAsync(Guid userId, UpdateProfileDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (!string.IsNullOrEmpty(updateDto.FullName))
            user.FullName = updateDto.FullName;  // Changed to match scaffolded model

        if (!string.IsNullOrEmpty(updateDto.PhoneNumber))
            user.Phone = updateDto.PhoneNumber;  // Changed PhoneNumber to Phone

        // TODO: Handle profile image upload
        if (updateDto.ProfileImage != null)
        {
            // Save image and update user.AvatarUrl (changed from ProfileImageUrl)
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        await _userRepository.DeleteAsync(user);
    }
}
