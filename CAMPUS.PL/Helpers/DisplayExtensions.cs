namespace CAMPUS.PL.Helpers;

public static class DisplayExtensions
{
    public static string OrDash(this string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "—" : value;
    }

    public static string ToDisplayDate(this DateTime value)
    {
        return value.ToString("yyyy-MM-dd HH:mm");
    }

    public static string ToTimeAgo(this DateTime value)
    {
        var span = DateTime.UtcNow - value.ToUniversalTime();
        if (span.TotalMinutes < 1)
            return "just now";
        if (span.TotalHours < 1)
            return $"{(int)span.TotalMinutes}m ago";
        if (span.TotalDays < 1)
            return $"{(int)span.TotalHours}h ago";
        return $"{(int)span.TotalDays}d ago";
    }
}
