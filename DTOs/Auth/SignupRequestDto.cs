using System.ComponentModel.DataAnnotations;

namespace CommunityEventsApi.DTOs.Auth;

public class SignupRequestDto
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;  // Changed from FirstName + LastName

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = null!;

}
