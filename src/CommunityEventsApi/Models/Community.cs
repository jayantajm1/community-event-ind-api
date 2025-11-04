using System.ComponentModel.DataAnnotations;

namespace CommunityEventsApi.Models;

public class Community
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    [Required]
    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
}
