using E_project_DVD_Shop.Models;

namespace E_project_DVD_Shop.Data;

/// <summary>Real-world catalog names for seed data.</summary>
public static class RealWorldCatalogData
{
    public const string CatalogMarker = "__CineVaultRealWorldCatalog__";
    public const string CatalogSyncMarker = "__CatalogDefinitionsSynced__";
    public const string LegacyFakeMarker = "__CineVaultHeavySeedComplete__";

    public static readonly string[] LegacyFakeArtistNames =
    [
        "The Soundwaves", "Luna Eclipse", "The Blue Notes", "Epic Games Studio",
        "Pixel Raiders", "Cinema Classics", "Neon Horizon Pictures", "Documentary Now",
        "Street Poets", "Symphony Hall", "Retro Arcade", "Midnight Reels"
    ];

    /// <summary>Music artists (singers / composers) per project spec.</summary>
    public static readonly (string Name, string Bio, string? ImageUrl)[] Artists =
    [
        ("Taylor Swift", "American singer-songwriter; Grammy-winning pop and country artist.", CatalogCoverUrls.GetArtistPhoto("Taylor Swift")),
        ("The Weeknd", "Canadian R&B and pop artist; After Hours and Blinding Lights.", CatalogCoverUrls.GetArtistPhoto("The Weeknd")),
        ("Beyoncé", "American singer and performer; landmark R&B and pop albums.", CatalogCoverUrls.GetArtistPhoto("Beyoncé")),
        ("Drake", "Canadian rapper and singer; multi-platinum hip-hop artist.", CatalogCoverUrls.GetArtistPhoto("Drake")),
        ("Adele", "British singer-songwriter; powerful vocals and bestselling albums.", CatalogCoverUrls.GetArtistPhoto("Adele")),
        ("Imagine Dragons", "American rock band — Radioactive, Demons, Believer.", CatalogCoverUrls.GetArtistPhoto("Imagine Dragons")),
        ("Miles Davis", "Jazz trumpeter; Kind of Blue is among the greatest jazz albums.", CatalogCoverUrls.GetArtistPhoto("Miles Davis")),
        ("Eminem", "American rapper; The Eminem Show and Lose Yourself.", CatalogCoverUrls.GetArtistPhoto("Eminem"))
    ];

    public static readonly (string Name, string Bio, string? ImageUrl)[] MusicStudios =
    [
        ("Atlantic Records", "Major label — pop, rock, and hip-hop.", null),
        ("Columbia Records", "Historic Sony label.", null),
        ("Sony Music Entertainment", "Global music company.", null),
        ("Universal Music Group", "World's largest music corporation.", null),
        ("Republic Records", "Pop and hip-hop label.", null),
        ("Capitol Records", "Iconic Hollywood label.", null),
        ("Warner Records", "Warner Music Group flagship label.", null),
        ("Interscope Records", "Dr. Dre, Eminem, Lady Gaga, and more.", null),
        ("Def Jam Recordings", "Hip-hop and urban music label.", null),
        ("Blue Note Records", "Legendary jazz label.", null)
    ];

    public static readonly (string Name, string Bio, string? ImageUrl)[] FilmStudios =
    [
        ("Warner Bros. Pictures",
            "One of the traditional \"Big Five\" major American film studios, operating under Warner Bros. Discovery with a massive historical library and some of the highest-grossing films in history. Key franchises: Harry Potter, DC Films (The Dark Knight trilogy, Superman), The Matrix, Dune (distributed), and Christopher Nolan films including Inception and Interstellar.",
            null),
        ("Marvel Studios",
            "Originally a Marvel Entertainment subsidiary, now part of The Walt Disney Company. Revolutionized modern blockbuster cinema by popularizing the shared cinematic universe model. Key franchises: the Marvel Cinematic Universe (MCU) — Avengers, Iron Man, Captain America, Black Panther, and Guardians of the Galaxy.",
            null),
        ("Lucasfilm",
            "Founded by George Lucas in 1971 and acquired by Disney in 2012. A pioneer in visual effects (Industrial Light & Magic) and sound design beyond filmmaking. Key franchises: Star Wars and Indiana Jones.",
            null),
        ("Universal Pictures",
            "Owned by Comcast through NBCUniversal; the oldest surviving film studio in the United States. Key franchises: Jurassic Park and Jurassic World, Fast & Furious, Despicable Me (Illumination), Bourne, and classic Universal Monsters (Dracula, Frankenstein).",
            null),
        ("Paramount Pictures",
            "The second oldest US film studio, based in Hollywood and a subsidiary of Paramount Global. Key franchises: Mission: Impossible, Transformers, Star Trek, Top Gun, early Indiana Jones distribution, and classics such as The Godfather.",
            null),
        ("Sony Pictures",
            "The American film and television arm of Sony; Columbia Pictures is its primary film label. Key franchises: Spider-Man (with Marvel), Ghostbusters, Jumanji, Men in Black, and recent James Bond distribution.",
            null),
        ("20th Century Studios",
            "Formerly 20th Century Fox until Disney acquired it in 2019; a major theatrical label for releases outside the core Disney family brand. Key franchises: Avatar, X-Men, Planet of the Apes, Alien, Predator, and Die Hard.",
            null),
        ("Walt Disney Pictures",
            "Flagship live-action and animation studio of The Walt Disney Studios, focused on family-friendly entertainment. Key franchises: Pirates of the Caribbean, National Treasure, live-action remakes (The Lion King, Aladdin), and fairy-tale adaptations such as Mary Poppins.",
            null),
        ("Pixar Animation Studios",
            "Pioneer in CGI animation, originally backed by Steve Jobs and acquired by Disney in 2006, renowned for emotional storytelling. Key franchises: Toy Story, Finding Nemo, The Incredibles, Up, Cars, Monsters, Inc., and Inside Out.",
            null),
        ("Legendary Entertainment",
            "Major US production company that partners with studios such as Warner Bros. and Universal to co-produce large-scale event films rather than self-distribute. Key franchises: the MonsterVerse (Godzilla, Kong: Skull Island), Denis Villeneuve's Dune, Pacific Rim, and Enola Holmes.",
            null)
    ];

    /// <summary>Major platform holders and AAA publishers only (admin catalog).</summary>
    public static readonly (string Name, string Bio, string? ImageUrl)[] GameStudios =
    [
        ("Nintendo", "Platform holder — Mario, Zelda, Pokémon.", null),
        ("Sony Interactive Entertainment", "PlayStation — God of War, Horizon, Spider-Man.", null),
        ("Microsoft Game Studios", "Xbox — Halo, Minecraft, Forza.", null),
        ("Rockstar Games", "Take-Two — Grand Theft Auto, Red Dead.", null),
        ("Electronic Arts", "FIFA / EA Sports, Battlefield, The Sims, Apex.", null),
        ("Activision Blizzard", "Call of Duty, World of Warcraft, Diablo, Overwatch.", null),
        ("Ubisoft", "Assassin's Creed, Far Cry, Rainbow Six.", null),
        ("Bethesda Game Studios", "The Elder Scrolls, Fallout (Microsoft).", null),
        ("Valve", "Steam, Half-Life, Portal, Counter-Strike.", null),
        ("Epic Games", "Fortnite, Unreal Engine, Epic Games Store.", null),
        ("Square Enix", "Final Fantasy, Kingdom Hearts, Dragon Quest.", null),
        ("Capcom", "Resident Evil, Street Fighter, Monster Hunter.", null),
        ("SEGA", "Sonic, Like a Dragon, Atlus titles.", null),
        ("Bandai Namco Entertainment", "Tekken, Pac-Man, Elden Ring (publisher).", null),
        ("Tencent Games", "Global investor and publisher — Honor of Kings, Riot stake.", null)
    ];

    /// <summary>Maps removed / dev studio names to a giant publisher when relinking games.</summary>
    public static readonly Dictionary<string, string> GameStudioAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Take-Two Interactive"] = "Rockstar Games",
        ["Activision"] = "Activision Blizzard",
        ["Blizzard Entertainment"] = "Activision Blizzard",
        ["FromSoftware"] = "Bandai Namco Entertainment",
        ["CD Projekt Red"] = "Tencent Games",
        ["Mojang Studios"] = "Microsoft Game Studios",
        ["Atlus"] = "SEGA",
        ["Larian Studios"] = "Tencent Games",
        ["Guerrilla Games"] = "Sony Interactive Entertainment",
        ["Santa Monica Studio"] = "Sony Interactive Entertainment",
        ["343 Industries"] = "Microsoft Game Studios",
        ["Konami"] = "Square Enix",
        ["Warner Bros. Games"] = "Electronic Arts",
        ["Riot Games"] = "Tencent Games",
        ["miHoYo"] = "Tencent Games"
    };

    public static readonly (string Title, string Artist, string Category, MediaType Media, int MonthsAgo, decimal Price, string Description, string CoverUrl)[] Releases =
    [
        ("1989", "Taylor Swift", "Pop", MediaType.Music, 120, 22.99m, "Taylor Swift's fifth studio album on DVD with music videos.", "https://upload.wikimedia.org/wikipedia/en/9/9f/Taylor_Swift_-_1989.png"),
        ("After Hours", "The Weeknd", "Pop", MediaType.Music, 48, 19.99m, "Features Blinding Lights and Save Your Tears.", "https://upload.wikimedia.org/wikipedia/en/4/4a/The_Weeknd_-_After_Hours.png"),
        ("Lemonade", "Beyoncé", "Pop", MediaType.Music, 96, 24.99m, "Visual album — R&B and pop.", "https://upload.wikimedia.org/wikipedia/en/1/1d/Beyonce_-_Lemonade_%28Official_Album_Cover%29.png"),
        ("Views", "Drake", "Pop", MediaType.Music, 72, 21.99m, "Hip-hop and R&B — One Dance, Hotline Bling.", "https://upload.wikimedia.org/wikipedia/en/1/19/Drake_-_Views.png"),
        ("21", "Adele", "Pop", MediaType.Music, 180, 18.99m, "Someone Like You, Rolling in the Deep.", "https://upload.wikimedia.org/wikipedia/en/B/Bb/Adele_-_21.png"),
        ("Night Visions", "Imagine Dragons", "Rock", MediaType.Music, 144, 17.99m, "Radioactive and Demons.", "https://upload.wikimedia.org/wikipedia/en/9/9b/Imagine_Dragons_-_Night_Visions.png"),
        ("Kind of Blue", "Miles Davis", "Jazz & Blues", MediaType.Music, 600, 16.99m, "1959 modal jazz masterpiece.", "https://upload.wikimedia.org/wikipedia/en/9/9c/MilesDavisKindofBlue.jpg"),
        ("The Eminem Show", "Eminem", "Pop", MediaType.Music, 260, 19.99m, "Lose Yourself and Without Me.", "https://upload.wikimedia.org/wikipedia/en/3/35/Eminem_-_The_Eminem_Show.jpg"),
        ("Grand Theft Auto V", "Rockstar Games", "Action & RPG", MediaType.Game, 36, 29.99m, "Open-world action — premium DVD edition.", "https://upload.wikimedia.org/wikipedia/en/a/a5/Grand_Theft_Auto_V.png"),
        ("Red Dead Redemption 2", "Rockstar Games", "Action & RPG", MediaType.Game, 48, 34.99m, "Western action-adventure epic.", "https://upload.wikimedia.org/wikipedia/en/4/44/Red_Dead_Redemption_II.jpg"),
        ("The Legend of Zelda: Breath of the Wild", "Nintendo", "Action & RPG", MediaType.Game, 60, 27.99m, "Adventure — collector's guide DVD.", "https://upload.wikimedia.org/wikipedia/en/c/c6/The_Legend_of_Zelda_Breath_of_the_Wild.jpg"),
        ("Elden Ring", "Bandai Namco Entertainment", "Action & RPG", MediaType.Game, 24, 32.99m, "Action RPG — behind-the-scenes DVD.", "https://upload.wikimedia.org/wikipedia/en/b/b9/Elden_Ring_cover_art.jpg"),
        ("The Witcher 3: Wild Hunt", "Tencent Games", "Action & RPG", MediaType.Game, 72, 24.99m, "Complete edition bonus content.", "https://upload.wikimedia.org/wikipedia/en/0/0c/Witcher_3_cover_art.jpg"),
        ("God of War", "Sony Interactive Entertainment", "Action & RPG", MediaType.Game, 48, 31.99m, "Norse saga — collector's DVD extras.", "https://upload.wikimedia.org/wikipedia/en/a/a7/God_of_War_4_cover_art.jpg"),
        ("Halo Infinite", "Microsoft Game Studios", "Action & RPG", MediaType.Game, 30, 26.99m, "Master Chief returns — multiplayer guide DVD.", "https://upload.wikimedia.org/wikipedia/en/8/8f/Halo_Infinite.png"),
        ("Half-Life 2", "Valve", "Action & RPG", MediaType.Game, 240, 14.99m, "Classic FPS — making-of DVD.", "https://upload.wikimedia.org/wikipedia/en/2/25/Half-Life_2_cover.jpg"),
        ("Assassin's Creed Valhalla", "Ubisoft", "Action & RPG", MediaType.Game, 42, 27.99m, "Viking-era open world.", "https://upload.wikimedia.org/wikipedia/en/f/fc/Assassin%27s_Creed_Valhalla_cover.jpg"),
        ("The Elder Scrolls V: Skyrim", "Bethesda Game Studios", "Action & RPG", MediaType.Game, 156, 19.99m, "Legendary edition bonus disc.", "https://upload.wikimedia.org/wikipedia/en/1/15/The_Elder_Scrolls_V_Skyrim_cover.png"),
        ("Cyberpunk 2077", "Tencent Games", "Action & RPG", MediaType.Game, 36, 28.99m, "Night City — behind-the-scenes DVD.", "https://upload.wikimedia.org/wikipedia/en/9/9f/Cyberpunk_2077_box_art.jpg"),
        ("Minecraft", "Microsoft Game Studios", "Action & RPG", MediaType.Game, 120, 17.99m, "Beginner's guide and crafting reference DVD.", "https://upload.wikimedia.org/wikipedia/en/5/51/Minecraft_cover.png"),
        ("Persona 5 Royal", "SEGA", "Action & RPG", MediaType.Game, 48, 29.99m, "JRPG — art book and soundtrack DVD.", "https://upload.wikimedia.org/wikipedia/en/b/bc/Persona_5_Royal_cover_art.jpg"),
        ("Resident Evil 4", "Capcom", "Action & RPG", MediaType.Game, 12, 33.99m, "Remake — survival horror DVD edition.", "https://upload.wikimedia.org/wikipedia/en/1/12/Resident_Evil_4_remake_cover_art.jpg"),
        ("Baldur's Gate 3", "Tencent Games", "Action & RPG", MediaType.Game, 18, 35.99m, "D&D RPG — companion guide DVD.", "https://upload.wikimedia.org/wikipedia/en/1/12/Baldur%27s_Gate_3_cover_art.jpg"),
        ("Horizon Forbidden West", "Sony Interactive Entertainment", "Action & RPG", MediaType.Game, 28, 30.99m, "Sequel — world guide DVD.", "https://upload.wikimedia.org/wikipedia/en/6/69/Horizon_Forbidden_West_cover_art.jpg"),
        ("The Dark Knight", "Warner Bros. Pictures", "Classics", MediaType.Movie, 192, 14.99m, "Christopher Nolan — Heath Ledger as the Joker.", "https://upload.wikimedia.org/wikipedia/en/1/1c/The_Dark_Knight_%282008_film%29.jpg"),
        ("Inception", "Warner Bros. Pictures", "Sci-Fi & Fantasy", MediaType.Movie, 168, 13.99m, "Sci-fi thriller directed by Christopher Nolan.", "https://upload.wikimedia.org/wikipedia/en/7/7f/Inception_%282010%29_theatrical_poster.jpg"),
        ("Avengers: Endgame", "Marvel Studios", "Sci-Fi & Fantasy", MediaType.Movie, 48, 15.99m, "MCU finale — epic superhero film.", "https://upload.wikimedia.org/wikipedia/en/0/0d/Avengers_Endgame_poster.jpg"),
        ("Star Wars: Episode IV – A New Hope", "Lucasfilm", "Sci-Fi & Fantasy", MediaType.Movie, 540, 12.99m, "1977 original — restored DVD.", "https://upload.wikimedia.org/wikipedia/en/8/87/StarWarsMoviePoster1977.jpg"),
        ("Jurassic Park", "Universal Pictures", "Sci-Fi & Fantasy", MediaType.Movie, 360, 11.99m, "Steven Spielberg dinosaur classic.", "https://upload.wikimedia.org/wikipedia/en/e/e7/Jurassic_Park_poster.jpg"),
        ("The Shawshank Redemption", "Warner Bros. Pictures", "Classics", MediaType.Movie, 360, 11.99m, "Stephen King drama — critically acclaimed.", "https://upload.wikimedia.org/wikipedia/en/8/81/ShawshankRedemptionMoviePoster.jpg"),
        ("Interstellar", "Warner Bros. Pictures", "Sci-Fi & Fantasy", MediaType.Movie, 96, 14.99m, "Space exploration epic by Nolan.", "https://upload.wikimedia.org/wikipedia/en/g/g7/Interstellar_film_poster.jpg")
    ];

    public static readonly Dictionary<string, string[]> MusicTracks = new(StringComparer.OrdinalIgnoreCase)
    {
        ["1989"] = ["Welcome to New York", "Blank Space", "Style", "Shake It Off", "Bad Blood"],
        ["After Hours"] = ["Alone Again", "Blinding Lights", "In Your Eyes", "Save Your Tears"],
        ["Lemonade"] = ["Pray You Catch Me", "Hold Up", "Sorry", "Formation", "All Night"],
        ["Views"] = ["Keep the Family Close", "One Dance", "Hotline Bling", "Too Good"],
        ["21"] = ["Rolling in the Deep", "Rumour Has It", "Turning Tables", "Someone Like You", "Set Fire to the Rain"],
        ["Night Visions"] = ["Radioactive", "Tiptoe", "Demons", "On Top of the World", "It's Time"],
        ["Kind of Blue"] = ["So What", "Freddie Freeloader", "Blue in Green", "All Blues"],
        ["The Eminem Show"] = ["White America", "Cleanin' Out My Closet", "Without Me", "Sing for the Moment", "Lose Yourself"]
    };

    public static readonly (string Title, string Content, string Category)[] NewsArticles =
    [
        ("Taylor Swift 1989 DVD Back in Stock", "Collector's edition available while supplies last.", "Releases"),
        ("GTA V & Red Dead 2 Bundle Sale", "Rockstar Games DVD bundles this weekend.", "Games"),
        ("Marvel Blockbusters on DVD", "Avengers: Endgame and more in the movie aisle.", "Movies"),
        ("Kind of Blue Restocked", "Miles Davis jazz classic for collectors.", "Releases"),
        ("Elden Ring Behind-the-Scenes DVD", "FromSoftware title with bonus content.", "Games"),
        ("Star Wars: A New Hope Remaster", "Lucasfilm original with updated packaging.", "Movies"),
        ("Member Weekend: 15% Off Music", "Discount on all music category DVDs.", "Promotions"),
        ("The Witcher 3 Complete Edition", "CD Projekt Red RPG now listed.", "Games")
    ];

    /// <summary>Sample #qna threads: member question + official admin answer.</summary>
    public static readonly (string Title, string Question, string AdminAnswer)[] ForumQnaThreads =
    [
        (
            "International shipping to Pakistan",
            "I live in Karachi - how long does delivery usually take after I place an order? Do you ship outside major cities?",
            "Yes, we ship nationwide. Orders are packed within 1-2 business days. After dispatch, expect about 3-5 business days in Karachi, Lahore, and Islamabad, and up to 7-10 business days for other areas. You will receive a tracking email once the parcel leaves our warehouse."
        ),
        (
            "Is Taylor Swift 1989 (Taylor's Version) deluxe worth it?",
            "The standard DVD is cheaper - does the deluxe actually add enough for a collector?",
            "If you care about bonus material, the deluxe is worth it: extra acoustic sessions, behind-the-scenes footage, and a booklet with liner notes. For playback only, the standard disc is fine. All copies we stock are region-free for Pakistan."
        ),
        (
            "Star Wars: original vs special edition on DVD",
            "Which cut of A New Hope do you recommend for someone buying their first physical copy?",
            "For most buyers we suggest the latest remastered edition - best picture and sound on modern TVs. Purists who want the theatrical cut should check the product description for the \"Original Trilogy\" label; stock varies, so message us before ordering if that matters to you."
        ),
        (
            "Elden Ring vs The Witcher 3 - collector editions",
            "I want one premium RPG box for my shelf. Which has better extras in your stock?",
            "Both are strong picks. Elden Ring collector sets usually include an art book, map, and steelbook; Witcher 3 complete editions add soundtrack discs and expanded lore booklets. If you play more than you display, Witcher 3 has more hours of content; Elden Ring wins on art and packaging."
        ),
        (
            "The Dark Knight vs Inception - best Nolan DVD?",
            "I can only grab one Christopher Nolan title this month. Which transfer is better?",
            "Dark Knight is the fan-favourite for rewatch value and IMAX sequences; Inception has a cleaner, more consistent 1080p transfer on most presses. For a first Nolan purchase we usually recommend Dark Knight; Inception is the better pick if you already own Batman Begins."
        ),
        (
            "How do I add money to my CineVault wallet?",
            "I see checkout uses account balance. How do members top up?",
            "Sign in, open your profile dashboard, and use **Add funds** (demo accounts may already include a starting balance). In production this would link to your payment provider; for now ask your instructor or use the admin test wallet amounts shown in the project guide."
        ),
        (
            "Damaged disc or wrong title received",
            "My order arrived with a cracked case and the disc skips. What should I do?",
            "Sorry about that - contact us within 7 days with your order number and photos of the case and disc. We will arrange a replacement or wallet refund once verified. Do not discard the packaging until we confirm, as it helps with the courier claim."
        ),
        (
            "Do you offer student or bundle discounts?",
            "Buying three music DVDs for a class project - any promo codes or bundles?",
            "Check the **Promotions** page and your cart - active site-wide codes apply automatically when valid. Music bundles (e.g. jazz or soundtrack sets) are listed under Promotions; students in the demo can use the sample code from the seed data on orders over the minimum shown at checkout."
        )
    ];

    /// <summary>Sample #general chat lines in display order (Author username, message).</summary>
    public static readonly (string Author, string Content)[] ForumGeneralChat =
    [
        ("admin", "Welcome to CineVault community chat - say hi, share pickups, and talk physical media. For order help use #qna."),
        ("jsmith", "Hey everyone - just got The Dark Knight on DVD. Picture quality is sharp on my 4K TV."),
        ("mchen", "Nice pickup! Is the Inception steelbook still listed? I want it for the shelf."),
        ("klee", "I saw Witcher 3 Complete Edition on sale yesterday - worth it if you do not have the extras yet."),
        ("awalker", "First order here - shipped to Lahore in four days. Tracking email was accurate."),
        ("admin", "Glad delivery went smoothly. We pack most orders within 1-2 business days after checkout."),
        ("rbrown", "The jazz aisle is slept on. Kind of Blue remaster sounds amazing through my home theater."),
        ("mchen", "1989 Taylor's Version deluxe arrived today - booklet and acoustic sessions are worth it."),
        ("jsmith", "Wallet checkout is quick once your balance is loaded. Cart + Place Order and done."),
        ("klee", "Anyone collecting Nolan on disc? Debating Dark Knight vs Inception for one purchase."),
        ("awalker", "Star Wars trilogy - went with the latest remaster. Colors pop compared to my old DVD."),
        ("admin", "Reminder: active promos are on the Promotions page - music bundles update every few weeks."),
        ("rbrown", "Left a review on the GTA V bundle. Bonus documentary disc was a surprise."),
        ("jsmith", "Elden Ring collector map is huge - does not fit standard shelves but it looks great."),
        ("mchen", "If a disc skips, #qna has the returns steps. They replaced my case fast last month."),
        ("klee", "Weekend watch list: Dark Knight, one music concert DVD, and a game making-of."),
        ("awalker", "Signed up as member mainly for wishlist + forum. Shop UI is clean."),
        ("admin", "New here? Browse Music / Games / Movies from the nav - product pages show trailers when available."),
        ("rbrown", "Pro tip: check New Releases on the home page before buying - restocks land there first."),
        ("jsmith", "See you all in chat - happy collecting.")
    ];

    public static readonly (string Name, string Contact, string Address, bool Active)[] Suppliers =
    [
        ("Sony Music Entertainment", "wholesale@sonymusic.com", "550 Madison Ave, New York, NY", true),
        ("Universal Music Group", "distribution@umusic.com", "2220 Colorado Ave, Santa Monica, CA", true),
        ("Warner Bros. Home Entertainment", "b2b@warnerbros.com", "4000 Warner Blvd, Burbank, CA", true),
        ("Nintendo of America", "supply@nintendo.com", "4600 150th Ave NE, Redmond, WA", true),
        ("Take-Two Interactive", "orders@take2games.com", "110 West 44th St, New York, NY", true)
    ];

    public static readonly (string Name, string Contact, string Address, bool Active)[] Producers =
    [
        ("Atlantic Records", "licensing@atlanticrecords.com", "1290 Avenue of the Americas, NY", true),
        ("Columbia Records", "archive@columbiarecords.com", "550 Madison Ave, New York, NY", true),
        ("Columbia Pictures", "homevideo@columbiapictures.com", "10202 W Washington Blvd, Culver City", true),
        ("Marvel Studios", "media@marvel.com", "500 S Buena Vista St, Burbank, CA", true),
        ("Lucasfilm Ltd.", "licensing@lucasfilm.com", "Letterman Digital Arts Center, SF", true)
    ];
}
