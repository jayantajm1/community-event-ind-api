using System;
using System.Collections.Generic;

namespace CommunityEventsApi.Models;

public partial class ViewUpcomingEvent
{
    public Guid? Id { get; set; }

    public string? Title { get; set; }

    public DateTime? StartDatetime { get; set; }

    public string? CommunityName { get; set; }

    public string? Organizer { get; set; }
}
