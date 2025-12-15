namespace CommunityEventsApi.DTOs.Events;

public class EventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public int MaxAttendees { get; set; }
    public int CurrentAttendees { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
    public Guid CommunityId { get; set; }
    public string CommunityName { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
