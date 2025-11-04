namespace CommunityEventsApi.Helpers;

public static class AppConstants
{
    // Roles
    public const string AdminRole = "Admin";
    public const string UserRole = "User";
    public const string ModeratorRole = "Moderator";

    // Event Status
    public const string EventStatusUpcoming = "Upcoming";
    public const string EventStatusOngoing = "Ongoing";
    public const string EventStatusCompleted = "Completed";
    public const string EventStatusCancelled = "Cancelled";

    // Default Values
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;
    public const double DefaultSearchRadius = 10.0; // km

    // File Upload
    public const string UploadsFolder = "uploads";
    public const string EventImagesFolder = "events";
    public const string UserImagesFolder = "users";
}
