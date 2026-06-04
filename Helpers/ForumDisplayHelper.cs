namespace E_project_DVD_Shop.Helpers;

public static class ForumDisplayHelper
{
    public static string GetInitials(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "?";
        var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{char.ToUpperInvariant(parts[0][0])}{char.ToUpperInvariant(parts[1][0])}";
        return name.Length >= 2
            ? name[..2].ToUpperInvariant()
            : name.ToUpperInvariant();
    }

    public static int GetAvatarHue(string? name)
    {
        if (string.IsNullOrEmpty(name)) return 220;
        return Math.Abs(name.GetHashCode(StringComparison.Ordinal)) % 360;
    }

    public static string FormatRelativeTime(DateTime utc)
    {
        var local = utc.ToLocalTime();
        var diff = DateTime.Now - local;
        if (diff.TotalMinutes < 1) return "Just now";
        if (diff.TotalHours < 1) return $"{(int)diff.TotalMinutes}m ago";
        if (diff.TotalDays < 1) return $"{(int)diff.TotalHours}h ago";
        if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d ago";
        return local.ToString("MMM dd, yyyy");
    }

    public static string Truncate(string text, int max)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        var oneLine = text.Replace("\r", " ").Replace("\n", " ").Trim();
        return oneLine.Length <= max ? oneLine : oneLine[..max] + "…";
    }
}
