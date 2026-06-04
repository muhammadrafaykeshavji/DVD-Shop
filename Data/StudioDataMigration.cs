using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Data;

/// <summary>Removes legacy studio-only artist rows after music catalog uses Artists.</summary>
public static class StudioDataMigration
{
    public const string MigrationMarker = "__StudioTablesMigrated__";

    public static async Task MigrateAsync(ApplicationDbContext context)
    {
        if (await context.News.AnyAsync(n => n.Title == MigrationMarker))
            return;

        var musicArtistIds = await context.Albums
            .Where(a => a.ArtistId != null)
            .Select(a => a.ArtistId!.Value)
            .Distinct()
            .ToListAsync();

        var studioOnlyArtists = await context.Artists
            .Where(a => !musicArtistIds.Contains(a.ArtistId))
            .ToListAsync();

        var gameStudioNames = await context.GameStudios.Select(s => s.Name).ToListAsync();
        var filmStudioNames = await context.FilmStudios.Select(s => s.Name).ToListAsync();

        foreach (var artist in studioOnlyArtists)
        {
            if (gameStudioNames.Contains(artist.Name) || filmStudioNames.Contains(artist.Name))
                context.Artists.Remove(artist);
        }

        await context.SaveChangesAsync();

        context.News.Add(new News
        {
            Title = MigrationMarker,
            Content = "Legacy artist/studio split completed.",
            Category = "System",
            Type = NewsType.National,
            IsActive = false,
            CreatedDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }
}
