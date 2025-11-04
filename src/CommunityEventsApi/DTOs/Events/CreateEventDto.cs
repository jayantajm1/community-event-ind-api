using System.ComponentModel.DataAnnotations;

namespace CommunityEventsApi.DTOs.Events;

public class CreateEventDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public IFormFile? Image { get; set; }

    [Required]
    [Range(1, 10000)]
    public int MaxAttendees { get; set; }

    [Required]
    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Required]
    [Range(-180, 180)]
    public double Longitude { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Required]
    public Guid CommunityId { get; set; }

    public Guid? CategoryId { get; set; }
}
