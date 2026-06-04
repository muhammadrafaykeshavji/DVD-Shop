using E_project_DVD_Shop.Models;

namespace E_project_DVD_Shop.Data;

/// <summary>Image URL helpers for placeholders and sample seed data.</summary>
public static class ImageUrlHelper
{
    public const string DefaultAlbum = "/images/placeholder-album.svg";
    public const string DefaultArtist = "/images/placeholder-artist.svg";
    public const string DefaultBanner = "/images/placeholder-banner.svg";

    public static string AlbumCover(MediaType mediaType) => mediaType switch
    {
        MediaType.Music => "/images/covers/music.svg",
        MediaType.Game => "/images/covers/game.svg",
        MediaType.Movie => "/images/covers/movie.svg",
        _ => DefaultAlbum
    };

    public static bool IsBrokenPath(string? url) =>
        string.IsNullOrWhiteSpace(url)
        || url.Contains("placeholder-album.jpg", StringComparison.OrdinalIgnoreCase)
        || url.Contains("placeholder-artist.jpg", StringComparison.OrdinalIgnoreCase)
        || url.Equals("/images/banner.jpg", StringComparison.OrdinalIgnoreCase);

    public static bool IsLocalPlaceholder(string? url) =>
        string.IsNullOrWhiteSpace(url)
        || url.StartsWith("/images/", StringComparison.OrdinalIgnoreCase);

    public static string Resolve(string? imageUrl, string? albumCoverUrl, string fallback) =>
        !string.IsNullOrWhiteSpace(imageUrl) ? imageUrl
        : !string.IsNullOrWhiteSpace(albumCoverUrl) ? albumCoverUrl
        : fallback;

    /// <summary>Deterministic sample photo (requires internet to load in browser).</summary>
    public static string SamplePhoto(string seed, int width = 400, int height = 400)
    {
        var safe = new string(seed.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());
        if (string.IsNullOrEmpty(safe)) safe = "default";
        return $"https://picsum.photos/seed/cinevault-{safe}/{width}/{height}";
    }

    public static string SampleArtist(string name) => SamplePhoto($"artist-{Slug(name)}", 400, 400);

    public static string SampleAlbum(string title, MediaType media) => media switch
    {
        MediaType.Movie => SamplePhoto($"movie-{Slug(title)}", 600, 900),
        MediaType.Game => SamplePhoto($"game-{Slug(title)}", 600, 800),
        _ => SamplePhoto($"album-{Slug(title)}", 600, 600)
    };

    public static string SampleSong(string albumTitle, string songTitle) =>
        SamplePhoto($"song-{Slug(albumTitle)}-{Slug(songTitle)}", 400, 400);

    public static string SampleGame(string title) => SamplePhoto($"game-item-{Slug(title)}", 600, 800);

    public static string SampleMovie(string title) => SamplePhoto($"movie-item-{Slug(title)}", 600, 900);

    public static string SampleBanner(int index) => SamplePhoto($"banner-{index}", 1200, 400);

    public static string SampleAvatar(string username) => SamplePhoto($"user-{Slug(username)}", 256, 256);

    private static string Slug(string text)
    {
        var chars = text.ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : '-')
            .ToArray();
        return new string(chars).Trim('-');
    }
}
