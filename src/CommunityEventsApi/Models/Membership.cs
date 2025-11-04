using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityEventsApi.Models;

public class Membership
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid CommunityId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Role { get; set; } = "Member";

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LeftAt { get; set; }

    // Navigation properties
    [ForeignKey("CommunityId")]
    public virtual Community Community { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
