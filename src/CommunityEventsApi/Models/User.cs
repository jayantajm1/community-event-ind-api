using System.ComponentModel.DataAnnotations;

namespace CommunityEventsApi.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    public string? ProfileImageUrl { get; set; }

    [Required]
    [StringLength(50)]
    public string Role { get; set; } = "User";

    public bool IsEmailVerified { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Event> CreatedEvents { get; set; } = new List<Event>();
    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
}
