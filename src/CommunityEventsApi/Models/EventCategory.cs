using System.ComponentModel.DataAnnotations;

namespace CommunityEventsApi.Models;

public class EventCategory
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public string? IconUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
