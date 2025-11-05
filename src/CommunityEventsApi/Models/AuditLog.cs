using System;
using System.Collections.Generic;

namespace CommunityEventsApi.Models;

public partial class AuditLog
{
    public Guid Id { get; set; }

    public Guid? ActorUserId { get; set; }

    public string ActionType { get; set; } = null!;

    public string? TargetType { get; set; }

    public Guid? TargetId { get; set; }

    public string? Metadata { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? ActorUser { get; set; }
}
