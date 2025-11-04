using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityEventsApi.Models;

public class EventRegistration
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid EventId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Registered";

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public DateTime? CancelledAt { get; set; }

    // Navigation properties
    [ForeignKey("EventId")]
    public virtual Event Event { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
