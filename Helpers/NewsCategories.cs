namespace E_project_DVD_Shop.Helpers;

/// <summary>Industry news categories synced from RSS (IGN, Pitchfork, Deadline).</summary>
public static class NewsCategories
{
    public const string Gaming = "Games";
    public const string Music = "Music";
    public const string Film = "Movies";

    public static readonly string[] All = [Gaming, Music, Film];

    public static readonly (string Value, string Label)[] Filters =
    [
        (Gaming, "Gaming Industry"),
        (Music, "Music Industry"),
        (Film, "Film Industry")
    ];

    public static bool IsIndustryCategory(string? category) =>
        !string.IsNullOrEmpty(category) && All.Contains(category, StringComparer.Ordinal);

    public static string GetDisplayName(string? category) =>
        category switch
        {
            Gaming => "Gaming Industry",
            Music => "Music Industry",
            Film => "Film Industry",
            _ => category ?? "News"
        };
}
