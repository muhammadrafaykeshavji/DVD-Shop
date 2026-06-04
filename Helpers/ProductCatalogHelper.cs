using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Helpers;

/// <summary>Unified access to shop catalog items (music album, game, or movie).</summary>
public static class ProductCatalogHelper
{
    public static MediaType GetMediaType(Product product)
    {
        if (product.GameId.HasValue) return MediaType.Game;
        if (product.MovieId.HasValue) return MediaType.Movie;
        return MediaType.Music;
    }

    public static string GetTitle(Product product) =>
        product.Album?.Title ?? product.Game?.Title ?? product.Movie?.Title ?? string.Empty;

    public static string? GetCoverUrl(Product product) =>
        product.Album?.CoverImageUrl
        ?? product.Game?.CoverImageUrl ?? product.Game?.ImageUrl
        ?? product.Movie?.CoverImageUrl ?? product.Movie?.ImageUrl;

    /// <summary>Wide landscape image for blurred product hero background.</summary>
    public static string? GetBannerUrl(Product product)
    {
        if (!string.IsNullOrWhiteSpace(product.Album?.BannerImageUrl))
            return product.Album.BannerImageUrl;
        var title = GetTitle(product);
        var landscape = CatalogCoverUrls.GetLandscapeBannerUrl(title);
        if (!string.IsNullOrWhiteSpace(product.Game?.ImageUrl)
            && CatalogCoverUrls.IsLandscapeBannerUrl(product.Game.ImageUrl))
            return product.Game.ImageUrl;
        if (!string.IsNullOrWhiteSpace(product.Movie?.ImageUrl)
            && CatalogCoverUrls.IsLandscapeBannerUrl(product.Movie.ImageUrl))
            return product.Movie.ImageUrl;
        if (!string.IsNullOrWhiteSpace(landscape))
            return landscape;
        return GetCoverUrl(product);
    }

    /// <summary>0 = barely visible, 100 = strongest banner presence.</summary>
    public static int GetBannerVisibilityPercent(Product product) =>
        Math.Clamp(product.Album?.BannerVisibility ?? 50, 0, 100);

    public static bool UsesLandscapeBanner(Product product, string? bannerUrl)
    {
        if (!string.IsNullOrWhiteSpace(product.Album?.BannerImageUrl))
            return true;
        if (CatalogCoverUrls.IsLandscapeBannerUrl(bannerUrl))
            return true;
        var cover = GetCoverUrl(product);
        return !string.IsNullOrWhiteSpace(bannerUrl)
            && !string.IsNullOrWhiteSpace(cover)
            && !string.Equals(bannerUrl, cover, StringComparison.OrdinalIgnoreCase);
    }

    public static string? GetDescription(Product product) =>
        product.Album?.Description ?? product.Game?.Description ?? product.Movie?.Description;

    public static DateTime GetReleaseDate(Product product) =>
        product.Album?.ReleaseDate ?? product.Game?.ReleaseDate ?? product.Movie?.ReleaseDate ?? DateTime.MinValue;

    public static string GetCategoryName(Product product) =>
        product.Album?.Category?.Name ?? product.Game?.Category?.Name ?? product.Movie?.Category?.Name ?? "—";

    public static int? GetCategoryId(Product product) =>
        product.Album?.CategoryId ?? product.Game?.CategoryId ?? product.Movie?.CategoryId;

    public static string GetCreditName(Product product) => product switch
    {
        { Album: { } a } => a.Artist?.Name ?? a.MusicStudio?.Name ?? "—",
        { Game: { } g } => g.GameStudio?.Name ?? "—",
        { Movie: { } m } => m.FilmStudio?.Name ?? "—",
        _ => "—"
    };

    public static string GetCreditName(Album album) =>
        album.Artist?.Name ?? album.MusicStudio?.Name ?? "—";

    public static string? GetPreviewUrl(Product product, IEnumerable<Song>? songs = null)
    {
        if (!string.IsNullOrWhiteSpace(product.Game?.PreviewUrl))
            return product.Game.PreviewUrl;
        if (!string.IsNullOrWhiteSpace(product.Movie?.PreviewUrl))
            return product.Movie.PreviewUrl;
        return songs?.Select(s => s.PreviewUrl).FirstOrDefault(u => !string.IsNullOrWhiteSpace(u))
            ?? product.Album?.Songs?.Select(s => s.PreviewUrl).FirstOrDefault(u => !string.IsNullOrWhiteSpace(u));
    }

    public static string GetPreviewKind(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return "none";
        if (MediaCatalogLinks.IsYouTubeUrl(url)) return "youtube";
        if (url.Contains(".mp3", StringComparison.OrdinalIgnoreCase)
            || url.Contains(".wav", StringComparison.OrdinalIgnoreCase))
            return "audio";
        return "video";
    }

    public static IQueryable<Product> WithCatalogIncludes(this IQueryable<Product> query) =>
        query
            .Include(p => p.Album!).ThenInclude(a => a.Artist)
            .Include(p => p.Album!).ThenInclude(a => a.MusicStudio)
            .Include(p => p.Album!).ThenInclude(a => a.Category)
            .Include(p => p.Album!).ThenInclude(a => a.Songs)
            .Include(p => p.Game!).ThenInclude(g => g.GameStudio)
            .Include(p => p.Game!).ThenInclude(g => g.Category)
            .Include(p => p.Movie!).ThenInclude(m => m.FilmStudio)
            .Include(p => p.Movie!).ThenInclude(m => m.Category);
}
