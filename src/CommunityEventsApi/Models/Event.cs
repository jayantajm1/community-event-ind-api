using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityEventsApi.Models;

public class Event
{
    [Key]
    public Guid Id { get; set; }

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

    public string? ImageUrl { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Upcoming";

    [Required]
    public int MaxAttendees { get; set; }

    public int CurrentAttendees { get; set; } = 0;

    [Required]
    public Guid CommunityId { get; set; }

    public Guid? CategoryId { get; set; }

    [Required]
    public Guid LocationId { get; set; }

    [Required]
    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("CommunityId")]
    public virtual Community Community { get; set; } = null!;

    [ForeignKey("CategoryId")]
    public virtual EventCategory? Category { get; set; }

    [ForeignKey("LocationId")]
    public virtual Location Location { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    public virtual User Creator { get; set; } = null!;

    public virtual ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
