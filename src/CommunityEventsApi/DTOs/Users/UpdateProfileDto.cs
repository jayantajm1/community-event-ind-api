using System.ComponentModel.DataAnnotations;

namespace CommunityEventsApi.DTOs.Users;

public class UpdateProfileDto
{
    [StringLength(100)]
    public string? FullName { get; set; }  // Changed from FirstName + LastName

    [Phone]
    public string? PhoneNumber { get; set; }

    public IFormFile? ProfileImage { get; set; }
}
