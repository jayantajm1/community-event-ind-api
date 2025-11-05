using System;
using System.Collections.Generic;

namespace CommunityEventsApi.Models;

public partial class Registration
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public Guid UserId { get; set; }

    public Guid CommunityId { get; set; }

    public string Status { get; set; } = null!;

    public string? RegistrationData { get; set; }

    public DateTime? RegisteredAt { get; set; }

    public DateTime? CheckedInAt { get; set; }

    public virtual Community Community { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
