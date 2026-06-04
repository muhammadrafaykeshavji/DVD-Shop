namespace E_project_DVD_Shop.Data;

/// <summary>Verified working cover/poster URLs (Wikipedia REST + Steam CDN).</summary>
public static class CatalogCoverUrls
{
    private static readonly Dictionary<string, string> ByTitle = new(StringComparer.OrdinalIgnoreCase)
    {
        // Music
        ["1989"] = "https://upload.wikimedia.org/wikipedia/en/f/f6/Taylor_Swift_-_1989.png",
        ["After Hours"] = "https://upload.wikimedia.org/wikipedia/en/c/c1/The_Weeknd_-_After_Hours.png",
        ["Lemonade"] = "https://upload.wikimedia.org/wikipedia/en/5/53/Beyonce_-_Lemonade_%28Official_Album_Cover%29.png",
        ["Views"] = "https://upload.wikimedia.org/wikipedia/en/a/af/Drake_-_Views_cover.jpg",
        ["21"] = "https://upload.wikimedia.org/wikipedia/en/1/1b/Adele_-_21.png",
        ["Night Visions"] = "https://upload.wikimedia.org/wikipedia/en/3/3f/Night_Visions_Album_Cover.jpeg",
        ["Kind of Blue"] = "https://upload.wikimedia.org/wikipedia/en/1/10/Miles_Davis_-_Kind_of_Blue_album_cover.jpg",
        ["The Eminem Show"] = "https://upload.wikimedia.org/wikipedia/en/3/35/The_Eminem_Show.jpg",

        // Games
        ["Grand Theft Auto V"] = "https://upload.wikimedia.org/wikipedia/en/a/a5/Grand_Theft_Auto_V.png",
        ["Red Dead Redemption 2"] = "https://upload.wikimedia.org/wikipedia/en/4/44/Red_Dead_Redemption_II.jpg",
        ["The Legend of Zelda: Breath of the Wild"] = "https://upload.wikimedia.org/wikipedia/en/c/c6/The_Legend_of_Zelda_Breath_of_the_Wild.jpg",
        ["Elden Ring"] = "https://upload.wikimedia.org/wikipedia/en/b/b9/Elden_Ring_Box_art.jpg",
        ["The Witcher 3: Wild Hunt"] = "https://upload.wikimedia.org/wikipedia/en/0/0c/Witcher_3_cover_art.jpg",
        ["God of War"] = "https://upload.wikimedia.org/wikipedia/en/a/a7/God_of_War_4_cover.jpg",
        ["Halo Infinite"] = "https://upload.wikimedia.org/wikipedia/en/1/14/Halo_Infinite.png",
        ["Half-Life 2"] = "https://upload.wikimedia.org/wikipedia/en/2/25/Half-Life_2_cover.jpg",
        ["Assassin's Creed Valhalla"] = "https://upload.wikimedia.org/wikipedia/en/f/ff/Assassin%27s_Creed_Valhalla_cover.jpg",
        ["The Elder Scrolls V: Skyrim"] = "https://upload.wikimedia.org/wikipedia/en/1/15/The_Elder_Scrolls_V_Skyrim_cover.png",
        ["Cyberpunk 2077"] = "https://upload.wikimedia.org/wikipedia/en/9/9f/Cyberpunk_2077_box_art.jpg",
        ["Minecraft"] = "https://cdn.akamai.steamstatic.com/steam/apps/322330/header.jpg",
        ["Persona 5 Royal"] = "https://upload.wikimedia.org/wikipedia/en/b/b0/Persona_5_cover_art.jpg",
        ["Resident Evil 4"] = "https://upload.wikimedia.org/wikipedia/en/d/df/Resident_Evil_4_remake_cover_art.jpg",
        ["Baldur's Gate 3"] = "https://upload.wikimedia.org/wikipedia/en/1/12/Baldur%27s_Gate_3_cover_art.jpg",
        ["Horizon Forbidden West"] = "https://upload.wikimedia.org/wikipedia/en/6/69/Horizon_Forbidden_West_cover_art.jpg",

        // Movies
        ["The Dark Knight"] = "https://upload.wikimedia.org/wikipedia/en/1/1c/The_Dark_Knight_%282008_film%29.jpg",
        ["Inception"] = "https://upload.wikimedia.org/wikipedia/en/2/2e/Inception_%282010%29_theatrical_poster.jpg",
        ["Avengers: Endgame"] = "https://upload.wikimedia.org/wikipedia/en/0/0d/Avengers_Endgame_poster.jpg",
        ["Star Wars: Episode IV – A New Hope"] = "https://upload.wikimedia.org/wikipedia/en/8/87/StarWarsMoviePoster1977.jpg",
        ["Jurassic Park"] = "https://upload.wikimedia.org/wikipedia/en/e/e7/Jurassic_Park_poster.jpg",
        ["The Shawshank Redemption"] = "https://upload.wikimedia.org/wikipedia/en/8/81/ShawshankRedemptionMoviePoster.jpg",
        ["Interstellar"] = "https://upload.wikimedia.org/wikipedia/en/b/bc/Interstellar_film_poster.jpg"
    };

    private static readonly string[] BannerUrls =
    [
        "https://upload.wikimedia.org/wikipedia/en/1/1c/The_Dark_Knight_%282008_film%29.jpg",
        "https://upload.wikimedia.org/wikipedia/en/b/b9/Elden_Ring_Box_art.jpg",
        "https://upload.wikimedia.org/wikipedia/en/f/f6/Taylor_Swift_-_1989.png",
        "https://upload.wikimedia.org/wikipedia/en/a/a5/Grand_Theft_Auto_V.png",
        "https://upload.wikimedia.org/wikipedia/en/0/0d/Avengers_Endgame_poster.jpg",
        "https://upload.wikimedia.org/wikipedia/en/c/c1/The_Weeknd_-_After_Hours.png"
    ];

    /// <summary>Wide landscape art for product page backgrounds (Steam headers, hero promos).</summary>
    private static readonly Dictionary<string, string> LandscapeBannerByTitle = new(StringComparer.OrdinalIgnoreCase)
    {
        // Music — wide promo / artist hero (fallback: cover still works with CSS)
        ["1989"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b1/Taylor_Swift_at_the_2023_MTV_Video_Music_Awards_%283%29.png/1280px-Taylor_Swift_at_the_2023_MTV_Video_Music_Awards_%283%29.png",
        ["After Hours"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a0/The_Weeknd_Portrait_by_Brian_Ziff.jpg/1280px-The_Weeknd_Portrait_by_Brian_Ziff.jpg",
        ["Lemonade"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b7/Beyonc%C3%A9_-_Tottenham_Hotspur_Stadium_-_1st_June_2023_%2810_of_118%29_%2852946364598%29_%28best_crop%29.jpg/1280px-Beyonc%C3%A9_-_Tottenham_Hotspur_Stadium_-_1st_June_2023_%2810_of_118%29_%2852946364598%29_%28best_crop%29.jpg",
        ["Views"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/15/Drake_at_The_Carter_Effect_2017_%2836818935200%29_%28cropped%29.jpg/1280px-Drake_at_The_Carter_Effect_2017_%2836818935200%29_%28cropped%29.jpg",
        ["21"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/7c/Adele_2016.jpg/1280px-Adele_2016.jpg",
        ["Night Visions"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a2/Imagine_Dragons_-_Uncasville_CT_-_November_2017_-_2.jpg/1280px-Imagine_Dragons_-_Uncasville_CT_-_November_2017_-_2.jpg",
        ["Kind of Blue"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/24/Miles_Davis_by_Palumbo_cropped.jpg/1280px-Miles_Davis_by_Palumbo_cropped.jpg",
        ["The Eminem Show"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/0f/Eminem_2021_Color_Corrected.jpg/1280px-Eminem_2021_Color_Corrected.jpg",

        // Games — Steam capsule headers (460×215, landscape)
        ["Grand Theft Auto V"] = "https://cdn.akamai.steamstatic.com/steam/apps/271590/header.jpg",
        ["Red Dead Redemption 2"] = "https://cdn.akamai.steamstatic.com/steam/apps/1174180/header.jpg",
        ["The Legend of Zelda: Breath of the Wild"] = "https://cdn.akamai.steamstatic.com/steam/apps/489830/header.jpg",
        ["Elden Ring"] = "https://cdn.akamai.steamstatic.com/steam/apps/1245620/header.jpg",
        ["The Witcher 3: Wild Hunt"] = "https://cdn.akamai.steamstatic.com/steam/apps/292030/header.jpg",
        ["God of War"] = "https://cdn.akamai.steamstatic.com/steam/apps/1593500/header.jpg",
        ["Halo Infinite"] = "https://cdn.akamai.steamstatic.com/steam/apps/1240440/header.jpg",
        ["Half-Life 2"] = "https://cdn.akamai.steamstatic.com/steam/apps/220/header.jpg",
        ["Assassin's Creed Valhalla"] = "https://cdn.akamai.steamstatic.com/steam/apps/2208920/header.jpg",
        ["The Elder Scrolls V: Skyrim"] = "https://cdn.akamai.steamstatic.com/steam/apps/72850/header.jpg",
        ["Cyberpunk 2077"] = "https://cdn.akamai.steamstatic.com/steam/apps/1091500/header.jpg",
        ["Minecraft"] = "https://cdn.akamai.steamstatic.com/steam/apps/322330/header.jpg",
        ["Persona 5 Royal"] = "https://cdn.akamai.steamstatic.com/steam/apps/1687950/header.jpg",
        ["Resident Evil 4"] = "https://cdn.akamai.steamstatic.com/steam/apps/2050650/header.jpg",
        ["Baldur's Gate 3"] = "https://cdn.akamai.steamstatic.com/steam/apps/1086940/header.jpg",
        ["Horizon Forbidden West"] = "https://cdn.akamai.steamstatic.com/steam/apps/1151340/header.jpg",

        // Movies — wide backdrops / hero stills
        ["The Dark Knight"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c1/The_Dark_Knight_%282008_film%29%281%29.jpg/1280px-The_Dark_Knight_%282008_film%29%281%29.jpg",
        ["Inception"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2e/Inception_%282010%29_theatrical_poster.jpg/1280px-Inception_%282010%29_theatrical_poster.jpg",
        ["Avengers: Endgame"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/1b/Avengers_Endgame_poster_artwork.png/1280px-Avengers_Endgame_poster_artwork.png",
        ["Star Wars: Episode IV – A New Hope"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/02/Star_Wars_DVD_release.jpg/1280px-Star_Wars_DVD_release.jpg",
        ["Jurassic Park"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/94/Jurassic_Park_%28film%29.jpg/1280px-Jurassic_Park_%28film%29.jpg",
        ["The Shawshank Redemption"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/81/ShawshankRedemptionMoviePoster.jpg/1280px-ShawshankRedemptionMoviePoster.jpg",
        ["Interstellar"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/bc/Interstellar_film_poster2.jpg/1280px-Interstellar_film_poster2.jpg"
    };

    public static string? GetCoverUrl(string title) =>
        ByTitle.TryGetValue(title, out var url) ? url : null;

    public static string? GetLandscapeBannerUrl(string title)
    {
        if (LandscapeBannerByTitle.TryGetValue(title, out var url))
            return url;
        return GetCoverUrl(title);
    }

    public static bool IsLandscapeBannerUrl(string? url) =>
        !string.IsNullOrWhiteSpace(url)
        && (url.Contains("header.jpg", StringComparison.OrdinalIgnoreCase)
            || url.Contains("steamstatic.com", StringComparison.OrdinalIgnoreCase)
            || url.Contains("/1280px-", StringComparison.OrdinalIgnoreCase)
            || url.Contains("placeholder-banner", StringComparison.OrdinalIgnoreCase));

    public static string? GetBannerUrl(int index) =>
        index >= 0 && index < BannerUrls.Length ? BannerUrls[index] : null;

    public static bool HasCover(string title) => ByTitle.ContainsKey(title);

    private static readonly Dictionary<string, string> ArtistPhotos = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Taylor Swift"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b1/Taylor_Swift_at_the_2023_MTV_Video_Music_Awards_%283%29.png/330px-Taylor_Swift_at_the_2023_MTV_Video_Music_Awards_%283%29.png",
        ["The Weeknd"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a0/The_Weeknd_Portrait_by_Brian_Ziff.jpg/330px-The_Weeknd_Portrait_by_Brian_Ziff.jpg",
        ["Beyoncé"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b7/Beyonc%C3%A9_-_Tottenham_Hotspur_Stadium_-_1st_June_2023_%2810_of_118%29_%2852946364598%29_%28best_crop%29.jpg/330px-Beyonc%C3%A9_-_Tottenham_Hotspur_Stadium_-_1st_June_2023_%2810_of_118%29_%2852946364598%29_%28best_crop%29.jpg",
        ["Drake"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/15/Drake_at_The_Carter_Effect_2017_%2836818935200%29_%28cropped%29.jpg/330px-Drake_at_The_Carter_Effect_2017_%2836818935200%29_%28cropped%29.jpg",
        ["Adele"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/7c/Adele_2016.jpg/330px-Adele_2016.jpg",
        ["Imagine Dragons"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a2/Imagine_Dragons_-_Uncasville_CT_-_November_2017_-_2.jpg/330px-Imagine_Dragons_-_Uncasville_CT_-_November_2017_-_2.jpg",
        ["Miles Davis"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/24/Miles_Davis_by_Palumbo_cropped.jpg/330px-Miles_Davis_by_Palumbo_cropped.jpg",
        ["Eminem"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/0f/Eminem_2021_Color_Corrected.jpg/330px-Eminem_2021_Color_Corrected.jpg"
    };

    public static string? GetArtistPhoto(string name) =>
        ArtistPhotos.TryGetValue(name, out var url) ? url : null;

    private static readonly Dictionary<string, string> MusicStudioImages = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Atlantic Records"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a2/Atlantic_Records_new_2025_logo.svg/330px-Atlantic_Records_new_2025_logo.svg.png",
        ["Columbia Records"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a9/Columbia_Records_logo.svg/330px-Columbia_Records_logo.svg.png",
        ["Sony Music Entertainment"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b9/Sony_Music_Entertainment_Logo_2023.svg/330px-Sony_Music_Entertainment_Logo_2023.svg.png",
        ["Universal Music Group"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/88/Umgheadquarters.jpg/330px-Umgheadquarters.jpg",
        ["Republic Records"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/98/Republic_Records_logo.svg/330px-Republic_Records_logo.svg.png",
        ["Capitol Records"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f8/Capitol-Records-Logo.svg/330px-Capitol-Records-Logo.svg.png",
        ["Warner Records"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3f/Warner_Music_Group_logo.svg/330px-Warner_Music_Group_logo.svg.png",
        ["Interscope Records"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6e/Interscope_Records.svg/330px-Interscope_Records.svg.png",
        ["Def Jam Recordings"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Def_Jam_Recordings.svg/330px-Def_Jam_Recordings.svg.png",
        ["Blue Note Records"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a5/Blue_Note_Records.svg/330px-Blue_Note_Records.svg.png"
    };

    private static readonly Dictionary<string, string> FilmStudioLogoImages = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Warner Bros. Pictures"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6a/Warner_Bros_Pictures_2023_Print_color.svg/330px-Warner_Bros_Pictures_2023_Print_color.svg.png",
        ["Marvel Studios"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2a/Marvel_Studios_logo.svg/330px-Marvel_Studios_logo.svg.png",
        ["Lucasfilm"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/10/Lucasfilm_logo.svg/330px-Lucasfilm_logo.svg.png",
        ["Universal Pictures"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b6/Universal_Pictures_logo.svg/330px-Universal_Pictures_logo.svg.png",
        ["Paramount Pictures"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2a/Paramount_Pictures_with_Skydance_byline.svg/330px-Paramount_Pictures_with_Skydance_byline.svg.png",
        ["Sony Pictures"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/8d/Sony_Pictures_Inc._logo.svg/330px-Sony_Pictures_Inc._logo.svg.png",
        ["20th Century Studios"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/22/20th_Century_Studios_%282020%29.svg/330px-20th_Century_Studios_%282020%29.svg.png",
        ["Walt Disney Pictures"] = "https://upload.wikimedia.org/wikipedia/en/thumb/5/57/Walt_Disney_Pictures_2011_logo.svg/330px-Walt_Disney_Pictures_2011_logo.svg.png",
        ["Pixar Animation Studios"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/0c/Pixar_animation_studios1.png/330px-Pixar_animation_studios1.png",
        ["Legendary Entertainment"] = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a8/Legendary_Entertainment_logo_variant.svg/330px-Legendary_Entertainment_logo_variant.svg.png"
    };

    public static string? GetGameStudioImage(string name) => GameStudioLogoUrls.GetLogo(name);

    public static string? GetFilmStudioImage(string name)
    {
        if (FilmStudioLogoImages.TryGetValue(name, out var logo))
            return logo;

        return MediaCatalogLinks.GetStudioLogo(name);
    }

    public static string? GetMusicStudioImage(string name) =>
        MusicStudioImages.TryGetValue(name, out var url) ? url : null;
}
