namespace E_project_DVD_Shop.Data;

/// <summary>Official-style image, trailer, and banner URLs for catalog seeding and repair.</summary>
public static class MediaCatalogLinks
{
    /// <summary>YouTube embed URLs for game trailers (title → embed URL).</summary>
    public static readonly Dictionary<string, string> GameTrailers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Grand Theft Auto V"] = "https://www.youtube.com/embed/QkkoHAzjnUs",
        ["Red Dead Redemption 2"] = "https://www.youtube.com/embed/eaW0tYpxyp0",
        ["The Legend of Zelda: Breath of the Wild"] = "https://www.youtube.com/embed/1rPxiXXxftc",
        ["Elden Ring"] = "https://www.youtube.com/embed/E3Huy2K-0Z4",
        ["The Witcher 3: Wild Hunt"] = "https://www.youtube.com/embed/c0i88t0Kacs",
        ["God of War"] = "https://www.youtube.com/embed/KFuBPMH53y0",
        ["Halo Infinite"] = "https://www.youtube.com/embed/PyHae6_BRZI",
        ["Half-Life 2"] = "https://www.youtube.com/embed/7LHk7XtpCn0",
        ["Assassin's Creed Valhalla"] = "https://www.youtube.com/embed/ssrNcwxALS4",
        ["The Elder Scrolls V: Skyrim"] = "https://www.youtube.com/embed/JSRtqaZm8Z0",
        ["Cyberpunk 2077"] = "https://www.youtube.com/embed/LembwKDoNYA",
        ["Minecraft"] = "https://www.youtube.com/embed/MmB9b5njVbA",
        ["Persona 5 Royal"] = "https://www.youtube.com/embed/x7ScFV6HqKU",
        ["Resident Evil 4"] = "https://www.youtube.com/embed/id0mltJHsic",
        ["Baldur's Gate 3"] = "https://www.youtube.com/embed/uuOXPWChwrk",
        ["Horizon Forbidden West"] = "https://www.youtube.com/embed/_epVgB1vKGs"
    };

    /// <summary>YouTube embed URLs for movie trailers.</summary>
    public static readonly Dictionary<string, string> MovieTrailers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["The Dark Knight"] = "https://www.youtube.com/embed/EXeTwQWrcwY",
        ["Inception"] = "https://www.youtube.com/embed/YoHD9ZFjxh0",
        ["Avengers: Endgame"] = "https://www.youtube.com/embed/TcMBFSGVi1c",
        ["Star Wars: Episode IV – A New Hope"] = "https://www.youtube.com/embed/vZ734NWnAHA",
        ["Jurassic Park"] = "https://www.youtube.com/embed/lc0DNJo8VNk",
        ["The Shawshank Redemption"] = "https://www.youtube.com/embed/6hBqi7zfapE",
        ["Interstellar"] = "https://www.youtube.com/embed/zSWdZVtXT7E"
    };

    /// <summary>Free audio preview clips (SoundHelix) per music album.</summary>
    public static readonly Dictionary<string, string> MusicPreviews = new(StringComparer.OrdinalIgnoreCase)
    {
        ["1989"] = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3",
        ["After Hours"] = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-2.mp3",
        ["Lemonade"] = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-3.mp3",
        ["Views"] = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-4.mp3",
        ["21"] = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-5.mp3",
        ["Night Visions"] = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-6.mp3",
        ["Kind of Blue"] = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-7.mp3",
        ["The Eminem Show"] = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-8.mp3"
    };

    public const string DefaultMusicPreview = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3";
    public const string DefaultGameTrailer = "https://www.youtube.com/embed/QkkoHAzjnUs";
    public const string DefaultMovieTrailer = "https://www.youtube.com/embed/EXeTwQWrcwY";

    /// <summary>Home carousel / sidebar / footer banner images (Wikipedia posters & covers).</summary>
    public static readonly (string ImageUrl, string LinkUrl, Models.AdPosition Position)[] Banners =
    [
        (CatalogCoverUrls.GetBannerUrl(0)!, "/Product?media=Movie", Models.AdPosition.HomeBanner),
        (CatalogCoverUrls.GetBannerUrl(1)!, "/Product?media=Game", Models.AdPosition.HomeBanner),
        (CatalogCoverUrls.GetBannerUrl(2)!, "/Product?media=Music", Models.AdPosition.HomeBanner),
        (CatalogCoverUrls.GetBannerUrl(3)!, "/Product?media=Game", Models.AdPosition.Sidebar),
        (CatalogCoverUrls.GetBannerUrl(4)!, "/Product?media=Movie", Models.AdPosition.Sidebar),
        (CatalogCoverUrls.GetBannerUrl(5)!, "/Product?media=Music", Models.AdPosition.Footer)
    ];

    /// <summary>Studio / label logos (Wikimedia Commons SVG/PNG — direct URLs, not game/movie art).</summary>
    public static readonly Dictionary<string, string> StudioLogos = new(StringComparer.OrdinalIgnoreCase)
    {
        // Major game publishers & developers
        ["Nintendo"] = "https://upload.wikimedia.org/wikipedia/commons/0/0d/Nintendo.svg",
        ["Sony Interactive Entertainment"] = "https://upload.wikimedia.org/wikipedia/commons/7/77/Sony_Interactive_Entertainment_logo_%282016%29.svg",
        ["Microsoft Game Studios"] = "https://upload.wikimedia.org/wikipedia/commons/5/52/Xbox_Game_Studios.svg",
        ["Rockstar Games"] = "https://upload.wikimedia.org/wikipedia/commons/5/53/Rockstar_Games_Logo.svg",
        ["Take-Two Interactive"] = "https://upload.wikimedia.org/wikipedia/commons/4/4f/Take-Two_Interactive_logo.svg",
        ["Electronic Arts"] = "https://upload.wikimedia.org/wikipedia/commons/0/0d/Electronic-Arts-Logo.svg",
        ["Activision Blizzard"] = "https://upload.wikimedia.org/wikipedia/commons/0/01/Activision.svg",
        ["Ubisoft"] = "https://upload.wikimedia.org/wikipedia/commons/7/78/Ubisoft_logo.svg",
        ["Bethesda Game Studios"] = "https://upload.wikimedia.org/wikipedia/commons/4/44/Bethesda_Game_Studios_Logo.svg",
        ["Valve"] = "https://upload.wikimedia.org/wikipedia/commons/8/83/Valve_Corporation_Logo.svg",
        ["Epic Games"] = "https://upload.wikimedia.org/wikipedia/commons/3/31/Epic_Games_logo.svg",
        ["Square Enix"] = "https://upload.wikimedia.org/wikipedia/commons/8/8a/Square_Enix_logo.svg",
        ["Capcom"] = "https://upload.wikimedia.org/wikipedia/commons/0/04/Capcom_logo.svg",
        ["SEGA"] = "https://upload.wikimedia.org/wikipedia/commons/1/13/Sega_logo.svg",
        ["Bandai Namco Entertainment"] = "https://upload.wikimedia.org/wikipedia/commons/8/84/Bandai_Namco_Entertainment_logo.svg",
        ["Konami"] = "https://upload.wikimedia.org/wikipedia/commons/9/9e/Konami_Logo.svg",
        ["Warner Bros. Games"] = "https://upload.wikimedia.org/wikipedia/commons/0/0e/Warner_Bros._Games_logo.svg",
        ["FromSoftware"] = "https://upload.wikimedia.org/wikipedia/commons/7/7c/FromSoftware_logo.svg",
        ["CD Projekt Red"] = "https://upload.wikimedia.org/wikipedia/commons/6/6a/CD_Projekt_Red_logo.svg",
        ["Mojang Studios"] = "https://upload.wikimedia.org/wikipedia/commons/7/7a/Mojang_Studios_logo.svg",
        ["Atlus"] = "https://upload.wikimedia.org/wikipedia/commons/8/8a/Atlus_logo.svg",
        ["Larian Studios"] = "https://upload.wikimedia.org/wikipedia/commons/b/b8/Larian_Studios_logo.svg",
        ["Guerrilla Games"] = "https://upload.wikimedia.org/wikipedia/commons/5/5a/Guerrilla_Games_logo.svg",
        ["Santa Monica Studio"] = "https://upload.wikimedia.org/wikipedia/commons/7/7e/SIE_Santa_Monica_Studio_logo.svg",
        ["343 Industries"] = "https://upload.wikimedia.org/wikipedia/commons/4/4e/343_Industries_logo.svg",
        ["Riot Games"] = "https://upload.wikimedia.org/wikipedia/commons/7/7c/Riot_Games_logo.svg",
        ["Tencent Games"] = "https://upload.wikimedia.org/wikipedia/commons/c/c9/Tencent_Logo.svg",
        ["miHoYo"] = "https://upload.wikimedia.org/wikipedia/commons/3/3a/MiHoYo_logo.svg",
        // Film studios
        ["Warner Bros. Pictures"] = "https://upload.wikimedia.org/wikipedia/commons/6/6c/Warner_Bros._logo.svg",
        ["Marvel Studios"] = "https://upload.wikimedia.org/wikipedia/commons/2/2a/Marvel_Studios_logo.svg",
        ["Lucasfilm"] = "https://upload.wikimedia.org/wikipedia/commons/1/10/Lucasfilm_logo.svg",
        ["Universal Pictures"] = "https://upload.wikimedia.org/wikipedia/commons/7/76/Universal_Pictures_logo.svg",
        ["Sony Pictures"] = "https://upload.wikimedia.org/wikipedia/commons/4/4a/Sony_Pictures_Entertainment_logo.svg",
        // Music labels
        ["Atlantic Records"] = "https://upload.wikimedia.org/wikipedia/commons/5/5f/Atlantic_Records_logo.svg",
        ["Columbia Records"] = "https://upload.wikimedia.org/wikipedia/commons/2/2c/Columbia_Records_logo.svg",
        ["Sony Music Entertainment"] = "https://upload.wikimedia.org/wikipedia/commons/b/bd/Sony_Music_Entertainment_logo.svg",
        ["Universal Music Group"] = "https://upload.wikimedia.org/wikipedia/commons/9/9a/Universal_Music_Group_logo.svg"
    };

    public static string? GetGameTrailer(string title) =>
        GameTrailers.TryGetValue(title, out var url) ? url : DefaultGameTrailer;

    public static string? GetMovieTrailer(string title) =>
        MovieTrailers.TryGetValue(title, out var url) ? url : DefaultMovieTrailer;

    public static string GetMusicPreview(string albumTitle) =>
        MusicPreviews.TryGetValue(albumTitle, out var url) ? url : DefaultMusicPreview;

    public static string? GetStudioLogo(string name) =>
        StudioLogos.TryGetValue(name, out var url) ? url : null;

    public static string? GetCoverForTitle(string title) =>
        CatalogCoverUrls.GetCoverUrl(title);

    public static bool IsYouTubeUrl(string? url) =>
        !string.IsNullOrWhiteSpace(url)
        && (url.Contains("youtube.com", StringComparison.OrdinalIgnoreCase)
            || url.Contains("youtu.be", StringComparison.OrdinalIgnoreCase));

    public static string ToYouTubeEmbed(string url)
    {
        if (url.Contains("/embed/", StringComparison.OrdinalIgnoreCase))
            return url;

        if (url.Contains("watch?v=", StringComparison.OrdinalIgnoreCase))
        {
            var id = url.Split("watch?v=", StringSplitOptions.None)[1].Split('&')[0];
            return $"https://www.youtube.com/embed/{id}";
        }

        if (url.Contains("youtu.be/", StringComparison.OrdinalIgnoreCase))
        {
            var id = url.Split("youtu.be/", StringSplitOptions.None)[1].Split('?')[0];
            return $"https://www.youtube.com/embed/{id}";
        }

        return url;
    }
}
