using CommunityEventsApi.DTOs.Auth;

namespace CommunityEventsApi.BAL.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> LoginAsync(LoginRequestDto loginDto);
    Task<TokenResponseDto> SignupAsync(SignupRequestDto signupDto);
    Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string userId);
}
