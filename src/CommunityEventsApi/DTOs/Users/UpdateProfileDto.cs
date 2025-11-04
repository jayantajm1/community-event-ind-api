using System.ComponentModel.DataAnnotations;

namespace CommunityEventsApi.DTOs.Users;

public class UpdateProfileDto
{
    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    public IFormFile? ProfileImage { get; set; }
}
