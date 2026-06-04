using E_project_DVD_Shop.Models;

namespace E_project_DVD_Shop.Data;

/// <summary>Genre categories per media type. Music / Games / Movies are <see cref="MediaType"/>, not DB categories.</summary>
public static class CatalogCategories
{
    public static readonly (string Name, string Description)[] MusicGenres =
    [
        ("Pop", "Pop and chart hits"),
        ("Rock", "Rock and alternative"),
        ("Jazz & Blues", "Jazz, blues, and soul")
    ];

    public static readonly (string Name, string Description)[] GameGenres =
    [
        ("Action & RPG", "Action and role-playing games")
    ];

    public static readonly (string Name, string Description)[] MovieGenres =
    [
        ("Sci-Fi & Fantasy", "Science fiction and fantasy films"),
        ("Documentary", "Documentary and educational"),
        ("Classics", "Classic cinema and award-winning films")
    ];

    /// <summary>Old top-level rows that duplicated media type — migrated to a genre on startup.</summary>
    public static readonly Dictionary<string, string> ObsoleteMediaTypeCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Music"] = "Pop",
        ["Games"] = "Action & RPG",
        ["Movies"] = "Classics"
    };

    public static readonly string[] AllGenreNames =
        MusicGenres.Select(g => g.Name)
            .Concat(GameGenres.Select(g => g.Name))
            .Concat(MovieGenres.Select(g => g.Name))
            .ToArray();

    public static IReadOnlyList<string> GetGenreNames(MediaType? mediaType) => mediaType switch
    {
        MediaType.Music => MusicGenres.Select(g => g.Name).ToList(),
        MediaType.Game => GameGenres.Select(g => g.Name).ToList(),
        MediaType.Movie => MovieGenres.Select(g => g.Name).ToList(),
        _ => AllGenreNames.ToList()
    };

    public static IEnumerable<(string Name, string Description)> GetGenreDefinitions(MediaType? mediaType) =>
        mediaType switch
        {
            MediaType.Music => MusicGenres,
            MediaType.Game => GameGenres,
            MediaType.Movie => MovieGenres,
            _ => MusicGenres.Concat(GameGenres).Concat(MovieGenres)
        };
}
