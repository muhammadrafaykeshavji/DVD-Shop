using E_project_DVD_Shop.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Data;

/// <summary>Backfills real image, trailer, and banner URLs on existing seeded data.</summary>
public static class MediaLinksRepair
{
    public static async Task RepairAsync(ApplicationDbContext context)
    {
        await RepairStudiosAsync(context);
        await RepairMediaItemsAsync(context);
        await RepairAdvertisementsAsync(context);
        await context.SaveChangesAsync();
    }

    private static async Task RepairStudiosAsync(ApplicationDbContext context)
    {
        foreach (var studio in await context.MusicStudios.ToListAsync())
        {
            var image = CatalogCoverUrls.GetMusicStudioImage(studio.Name);
            if (image != null && ShouldReplaceStudioImage(studio.ImageUrl))
                studio.ImageUrl = image;
        }

        foreach (var studio in await context.GameStudios.ToListAsync())
        {
            var image = CatalogCoverUrls.GetGameStudioImage(studio.Name);
            if (image != null && ShouldReplaceStudioImage(studio.ImageUrl))
                studio.ImageUrl = image;
        }

        foreach (var studio in await context.FilmStudios.ToListAsync())
        {
            var image = CatalogCoverUrls.GetFilmStudioImage(studio.Name);
            if (image != null && ShouldReplaceStudioImage(studio.ImageUrl))
                studio.ImageUrl = image;
        }
    }

    private static async Task RepairMediaItemsAsync(ApplicationDbContext context)
    {
        var songs = await context.Songs.Include(s => s.Album).ToListAsync();
        foreach (var song in songs)
        {
            var cover = MediaCatalogLinks.GetCoverForTitle(song.Album.Title);
            if (cover != null)
                song.ImageUrl = cover;

            var preview = MediaCatalogLinks.GetMusicPreview(song.Album.Title);
            song.PreviewUrl = preview;
            if (IsGenericPreview(song.DownloadUrl))
                song.DownloadUrl = preview;
        }

        foreach (var game in await context.Games.ToListAsync())
        {
            var cover = MediaCatalogLinks.GetCoverForTitle(game.Title);
            if (cover != null)
                game.CoverImageUrl = cover;
            var banner = CatalogCoverUrls.GetLandscapeBannerUrl(game.Title);
            if (banner != null)
                game.ImageUrl = banner;
            else if (cover != null)
                game.ImageUrl = cover;

            game.PreviewUrl = MediaCatalogLinks.GetGameTrailer(game.Title);
        }

        foreach (var movie in await context.Movies.ToListAsync())
        {
            var cover = MediaCatalogLinks.GetCoverForTitle(movie.Title);
            if (cover != null)
                movie.CoverImageUrl = cover;
            var banner = CatalogCoverUrls.GetLandscapeBannerUrl(movie.Title);
            if (banner != null)
                movie.ImageUrl = banner;
            else if (cover != null)
                movie.ImageUrl = cover;

            movie.PreviewUrl = MediaCatalogLinks.GetMovieTrailer(movie.Title);
        }
    }

    private static async Task RepairAdvertisementsAsync(ApplicationDbContext context)
    {
        var ads = await context.Advertisements.OrderBy(a => a.AdId).ToListAsync();
        var banners = MediaCatalogLinks.Banners;

        for (var i = 0; i < ads.Count && i < banners.Length; i++)
        {
            ads[i].ImageUrl = banners[i].ImageUrl;
            ads[i].LinkUrl = banners[i].LinkUrl;
            ads[i].Position = banners[i].Position;
            ads[i].IsActive = true;
        }

        for (var i = ads.Count; i < banners.Length; i++)
        {
            context.Advertisements.Add(new Advertisement
            {
                ImageUrl = banners[i].ImageUrl,
                LinkUrl = banners[i].LinkUrl,
                Position = banners[i].Position,
                IsActive = true
            });
        }
    }

    private static bool ShouldReplace(string? url) =>
        ImageUrlHelper.IsBrokenPath(url)
        || ImageUrlHelper.IsLocalPlaceholder(url)
        || (!string.IsNullOrWhiteSpace(url) && url.Contains("picsum.photos", StringComparison.OrdinalIgnoreCase));

    private static bool ShouldReplaceStudioImage(string? url) =>
        ShouldReplace(url)
        || string.IsNullOrWhiteSpace(url)
        || url.Contains("placeholder-album", StringComparison.OrdinalIgnoreCase)
        || url.Contains("steamstatic.com", StringComparison.OrdinalIgnoreCase);

    private static bool IsGenericPreview(string? url) =>
        string.IsNullOrWhiteSpace(url)
        || url.Contains("mov_bbb.mp4", StringComparison.OrdinalIgnoreCase)
        || url.Contains("SoundHelix-Song-1.mp3", StringComparison.OrdinalIgnoreCase);
}
