using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.DAL.Interfaces;
using CommunityEventsApi.DTOs.Auth;
using CommunityEventsApi.Helpers;
using CommunityEventsApi.Models;

namespace CommunityEventsApi.BAL.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly TokenGenerator _tokenGenerator;

    public AuthService(IUserRepository userRepository, TokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<TokenResponseDto> LoginAsync(LoginRequestDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);

        if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var accessToken = _tokenGenerator.GenerateJwtToken(user.Id.ToString(), user.Email, user.Role);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    public async Task<TokenResponseDto> SignupAsync(SignupRequestDto signupDto)
    {
        if (await _userRepository.EmailExistsAsync(signupDto.Email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = signupDto.FirstName,
            LastName = signupDto.LastName,
            Email = signupDto.Email,
            PhoneNumber = signupDto.PhoneNumber,
            PasswordHash = PasswordHasher.HashPassword(signupDto.Password),
            Role = AppConstants.UserRole,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        var accessToken = _tokenGenerator.GenerateJwtToken(user.Id.ToString(), user.Email, user.Role);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
    {
        // TODO: Implement refresh token validation and storage
        throw new NotImplementedException();
    }

    public async Task LogoutAsync(string userId)
    {
        // TODO: Implement token revocation/blacklisting
        await Task.CompletedTask;
    }
}
