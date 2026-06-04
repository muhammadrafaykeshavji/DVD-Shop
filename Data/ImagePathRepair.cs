using E_project_DVD_Shop.Models;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Data;

/// <summary>Fixes invalid paths and backfills sample https image URLs for seeded data.</summary>
public static class ImagePathRepair
{
    public static async Task RepairAsync(ApplicationDbContext context)
    {
        var albums = await context.Albums.ToListAsync();
        foreach (var album in albums)
        {
            var catalogUrl = CatalogCoverUrls.GetCoverUrl(album.Title);
            if (!string.IsNullOrEmpty(catalogUrl))
                album.CoverImageUrl = catalogUrl;
            else if (HasValidExternalImage(album.CoverImageUrl)) continue;
            else if (ImageUrlHelper.IsBrokenPath(album.CoverImageUrl) || ImageUrlHelper.IsLocalPlaceholder(album.CoverImageUrl))
            {
                var release = RealWorldCatalogData.Releases.FirstOrDefault(r => r.Title == album.Title);
                album.CoverImageUrl = release.CoverUrl ?? ImageUrlHelper.SampleAlbum(album.Title, MediaType.Music);
            }
        }

        var artists = await context.Artists.ToListAsync();
        foreach (var artist in artists)
        {
            var artistUrl = CatalogCoverUrls.GetArtistPhoto(artist.Name);
            if (!string.IsNullOrEmpty(artistUrl))
                artist.ImageUrl = artistUrl;
            else if (HasValidExternalImage(artist.ImageUrl)) continue;
            else if (ImageUrlHelper.IsBrokenPath(artist.ImageUrl) || ImageUrlHelper.IsLocalPlaceholder(artist.ImageUrl))
            {
                var catalog = RealWorldCatalogData.Artists.FirstOrDefault(a => a.Name == artist.Name);
                artist.ImageUrl = catalog.ImageUrl ?? ImageUrlHelper.SampleArtist(artist.Name);
            }
        }

        var songs = await context.Songs.Include(s => s.Album).ToListAsync();
        foreach (var song in songs)
        {
            var cover = MediaCatalogLinks.GetCoverForTitle(song.Album.Title);
            if (cover != null && (string.IsNullOrWhiteSpace(song.ImageUrl) || ImageUrlHelper.IsLocalPlaceholder(song.ImageUrl) || song.ImageUrl.Contains("picsum.photos", StringComparison.OrdinalIgnoreCase)))
                song.ImageUrl = cover;
        }

        foreach (var game in await context.Games.ToListAsync())
        {
            var cover = MediaCatalogLinks.GetCoverForTitle(game.Title);
            if (cover != null && (string.IsNullOrWhiteSpace(game.ImageUrl) || ImageUrlHelper.IsLocalPlaceholder(game.ImageUrl) || game.ImageUrl.Contains("picsum.photos", StringComparison.OrdinalIgnoreCase)))
            {
                game.CoverImageUrl = cover;
                game.ImageUrl = cover;
            }
        }

        foreach (var movie in await context.Movies.ToListAsync())
        {
            var cover = MediaCatalogLinks.GetCoverForTitle(movie.Title);
            if (cover != null && (string.IsNullOrWhiteSpace(movie.ImageUrl) || ImageUrlHelper.IsLocalPlaceholder(movie.ImageUrl) || movie.ImageUrl.Contains("picsum.photos", StringComparison.OrdinalIgnoreCase)))
            {
                movie.CoverImageUrl = cover;
                movie.ImageUrl = cover;
            }
        }

        var users = await context.Users.ToListAsync();
        foreach (var user in users)
        {
            if (string.IsNullOrWhiteSpace(user.ProfileImageUrl) || ImageUrlHelper.IsLocalPlaceholder(user.ProfileImageUrl))
                user.ProfileImageUrl = ImageUrlHelper.SampleAvatar(user.UserName ?? user.Email ?? "user");
        }

        await context.SaveChangesAsync();
    }

    private static bool HasValidExternalImage(string? url) =>
        !string.IsNullOrWhiteSpace(url)
        && url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
        && !url.Contains("picsum.photos", StringComparison.OrdinalIgnoreCase);
}
