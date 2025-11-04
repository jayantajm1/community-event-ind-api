namespace CommunityEventsApi.Utils.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength];
    }

    public static string ToSlug(this string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;

        return value
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-");
    }
}
