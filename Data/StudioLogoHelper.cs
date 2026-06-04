namespace E_project_DVD_Shop.Data;

/// <summary>Builds displayable PNG thumbnails from Wikimedia Commons logo URLs.</summary>
public static class StudioLogoHelper
{
    /// <summary>Converts a Commons SVG/PNG URL to a 330px PNG thumbnail.</summary>
    public static string? ToThumbUrl(string? commonsUrl)
    {
        if (string.IsNullOrWhiteSpace(commonsUrl))
            return null;

        if (commonsUrl.Contains("/thumb/", StringComparison.OrdinalIgnoreCase))
            return commonsUrl;

        const string marker = "/wikipedia/commons/";
        var idx = commonsUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
            return commonsUrl;

        var path = commonsUrl[(idx + marker.Length)..];
        var file = path.Split('/').Last();
        return $"https://upload.wikimedia.org/wikipedia/commons/thumb/{path}/330px-{file}.png";
    }
}
