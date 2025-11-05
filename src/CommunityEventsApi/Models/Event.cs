using System;
using System.Collections.Generic;

namespace CommunityEventsApi.Models;

public partial class Event
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid CommunityId { get; set; }

    public Guid OrganizerId { get; set; }

    public string EventType { get; set; } = null!;

    public List<string>? Tags { get; set; }

    public DateTime StartDatetime { get; set; }

    public DateTime EndDatetime { get; set; }

    public string? LocationName { get; set; }

    public string? Address { get; set; }

    public int? MaxParticipants { get; set; }

    public string RegistrationMode { get; set; } = null!;

    public string Visibility { get; set; } = null!;

    public string Status { get; set; } = null!;

    public List<string>? Images { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Community Community { get; set; } = null!;

    public virtual User Organizer { get; set; } = null!;

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
