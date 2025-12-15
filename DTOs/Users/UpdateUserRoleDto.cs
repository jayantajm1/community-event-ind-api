using System.ComponentModel.DataAnnotations;

namespace CommunityEventsApi.DTOs.Users;

public class UpdateUserRoleDto
{
    [Required]
    [RegularExpression("^(User|CommunityHost|Admin)$", ErrorMessage = "Role must be User, CommunityHost, or Admin")]
    public string Role { get; set; } = string.Empty;
}
