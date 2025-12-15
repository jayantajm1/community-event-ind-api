using System;
using System.Collections.Generic;

namespace CommunityEventsApi.Models;

public partial class Community
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? City { get; set; }

    public string? Region { get; set; }

    public string Visibility { get; set; } = null!;

    public Guid CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
