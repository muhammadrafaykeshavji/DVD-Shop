using Microsoft.EntityFrameworkCore;
using E_project_DVD_Shop.Data;

namespace E_project_DVD_Shop.Data;

/// <summary>Fixes broken Wikipedia hash paths by applying verified cover URLs.</summary>
public static class CoverImageRepair
{
    public static async Task RepairAsync(ApplicationDbContext context)
    {
        var albums = await context.Albums.ToListAsync();
        foreach (var album in albums)
        {
            var url = CatalogCoverUrls.GetCoverUrl(album.Title);
            if (!string.IsNullOrEmpty(url)
                && (string.IsNullOrWhiteSpace(album.CoverImageUrl)
                    || ImageUrlHelper.IsBrokenPath(album.CoverImageUrl)
                    || ImageUrlHelper.IsLocalPlaceholder(album.CoverImageUrl)))
                album.CoverImageUrl = url;
        }

        var songs = await context.Songs.Include(s => s.Album).ToListAsync();
        foreach (var song in songs)
        {
            var url = CatalogCoverUrls.GetCoverUrl(song.Album.Title);
            if (!string.IsNullOrEmpty(url)
                && (string.IsNullOrWhiteSpace(song.ImageUrl)
                    || ImageUrlHelper.IsBrokenPath(song.ImageUrl)
                    || ImageUrlHelper.IsLocalPlaceholder(song.ImageUrl)))
                song.ImageUrl = url;
        }

        foreach (var game in await context.Games.ToListAsync())
        {
            var url = CatalogCoverUrls.GetCoverUrl(game.Title);
            if (!string.IsNullOrEmpty(url)
                && (string.IsNullOrWhiteSpace(game.CoverImageUrl)
                    || ImageUrlHelper.IsBrokenPath(game.CoverImageUrl)
                    || ImageUrlHelper.IsLocalPlaceholder(game.CoverImageUrl)))
            {
                game.CoverImageUrl = url;
                game.ImageUrl = url;
            }
        }

        foreach (var movie in await context.Movies.ToListAsync())
        {
            var url = CatalogCoverUrls.GetCoverUrl(movie.Title);
            if (!string.IsNullOrEmpty(url)
                && (string.IsNullOrWhiteSpace(movie.CoverImageUrl)
                    || ImageUrlHelper.IsBrokenPath(movie.CoverImageUrl)
                    || ImageUrlHelper.IsLocalPlaceholder(movie.CoverImageUrl)))
            {
                movie.CoverImageUrl = url;
                movie.ImageUrl = url;
            }
        }

        var artists = await context.Artists.ToListAsync();
        foreach (var artist in artists)
        {
            var url = CatalogCoverUrls.GetArtistPhoto(artist.Name);
            if (!string.IsNullOrEmpty(url))
                artist.ImageUrl = url;
        }

        var ads = await context.Advertisements.OrderBy(a => a.AdId).ToListAsync();
        for (var i = 0; i < ads.Count; i++)
        {
            var banner = CatalogCoverUrls.GetBannerUrl(i);
            if (!string.IsNullOrEmpty(banner))
                ads[i].ImageUrl = banner;
        }

        foreach (var studio in await context.MusicStudios.ToListAsync())
        {
            if (!NeedsStudioImageRepair(studio.ImageUrl))
                continue;
            var url = CatalogCoverUrls.GetMusicStudioImage(studio.Name);
            if (!string.IsNullOrEmpty(url))
                studio.ImageUrl = url;
        }

        foreach (var studio in await context.GameStudios.ToListAsync())
        {
            if (!NeedsStudioImageRepair(studio.ImageUrl))
                continue;
            var url = CatalogCoverUrls.GetGameStudioImage(studio.Name);
            if (!string.IsNullOrEmpty(url))
                studio.ImageUrl = url;
        }

        foreach (var studio in await context.FilmStudios.ToListAsync())
        {
            if (!NeedsStudioImageRepair(studio.ImageUrl))
                continue;
            var url = CatalogCoverUrls.GetFilmStudioImage(studio.Name);
            if (!string.IsNullOrEmpty(url))
                studio.ImageUrl = url;
        }

        await context.SaveChangesAsync();
    }

    private static bool IsBadStudioImage(string? url) =>
        string.IsNullOrWhiteSpace(url)
        || url.Contains("placeholder-album", StringComparison.OrdinalIgnoreCase)
        || url.Contains("picsum.photos", StringComparison.OrdinalIgnoreCase)
        || url.Contains("steamstatic.com", StringComparison.OrdinalIgnoreCase)
        || url.Contains("/wikipedia/en/", StringComparison.OrdinalIgnoreCase);

    /// <summary>Only replace studio images that are missing or known-bad — not admin-set URLs.</summary>
    private static bool NeedsStudioImageRepair(string? url) => IsBadStudioImage(url);
}
