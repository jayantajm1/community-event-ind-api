namespace CommunityEventsApi.Helpers;

public static class GeoUtils
{
    private const double EarthRadiusKm = 6371.0;

    /// <summary>
    /// Calculate distance between two geographic coordinates using Haversine formula
    /// </summary>
    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = EarthRadiusKm * c;

        return distance;
    }

    /// <summary>
    /// Check if a point is within a radius of another point
    /// </summary>
    public static bool IsWithinRadius(double lat1, double lon1, double lat2, double lon2, double radiusKm)
    {
        var distance = CalculateDistance(lat1, lon1, lat2, lon2);
        return distance <= radiusKm;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
