using System.ComponentModel.DataAnnotations;

namespace CommunityEventsApi.DTOs.Auth;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
