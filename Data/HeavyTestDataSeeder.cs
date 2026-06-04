using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Data;

/// <summary>Seeds real-world catalog data and sample orders for admin testing.</summary>
public static class HeavyTestDataSeeder
{
    private const string MemberPassword = "Member@123";
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        var catalogComplete = await context.News.AnyAsync(n => n.Title == RealWorldCatalogData.CatalogMarker);

        if (catalogComplete)
        {
            if (!await context.News.AnyAsync(n => n.Title == RealWorldCatalogData.CatalogSyncMarker))
            {
                await SyncCatalogDefinitionsAsync(context);
                context.News.Add(new News
                {
                    Title = RealWorldCatalogData.CatalogSyncMarker,
                    Content = "Catalog sync from RealWorldCatalogData completed.",
                    Category = "System",
                    Type = NewsType.National,
                    IsActive = false,
                    CreatedDate = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }

            await StudioDataMigration.MigrateAsync(context);
            await EnforceGameStudioCatalogAsync(context);
            await EnforceFilmStudioCatalogAsync(context);
            return;
        }

        var hasLegacyFake = await context.Artists.AnyAsync(a =>
            RealWorldCatalogData.LegacyFakeArtistNames.Contains(a.Name));

        if (hasLegacyFake || await context.News.AnyAsync(n => n.Title == RealWorldCatalogData.LegacyFakeMarker))
            await ReplaceLegacyCatalogAsync(context);

        await EnsureRolesAsync(roleManager);
        await EnsureCoreUsersAsync(userManager);

        var rng = new Random(2026);
        var categories = await EnsureCategoriesAsync(context);
        var suppliers = await EnsureSuppliersAsync(context);
        var producers = await EnsureProducersAsync(context);
        var artists = await EnsureArtistsAsync(context);
        var musicStudios = await EnsureMusicStudiosAsync(context);
        var gameStudios = await EnsureGameStudiosAsync(context);
        var filmStudios = await EnsureFilmStudiosAsync(context);
        var albums = await EnsureMusicAlbumsAsync(context, categories, artists, rng);
        var games = await EnsureGamesAsync(context, categories, gameStudios, rng);
        var movies = await EnsureMoviesAsync(context, categories, filmStudios, rng);
        var products = await EnsureProductsAsync(context, albums, games, movies, suppliers, producers, rng);
        await EnsureSongsAsync(context, albums);
        await StudioDataMigration.MigrateAsync(context);
        await EnsureNewsPromotionsAdsAsync(context);
        await EnsurePurchasingInvoicesAsync(context, suppliers, rng);
        var members = await EnsureTestMembersAsync(userManager, rng);
        await EnsureOrdersAsync(context, members, products, rng);
        await EnsureReviewsAsync(context, members, products, rng);
        await EnsureFeedbackAsync(context, members, rng);
        await EnsureWishlistsAsync(context, members, products, rng);
        await ForumQnaSeedRepair.EnsureAsync(context, userManager);
        await ForumGeneralSeedRepair.EnsureAsync(context, userManager);

        context.News.Add(new News
        {
            Title = RealWorldCatalogData.CatalogMarker,
            Content = "Internal marker — real-world catalog seed completed.",
            Category = "System",
            Type = NewsType.National,
            IsActive = false,
            CreatedDate = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
    }

    /// <summary>Adds any artists/albums/products/games from <see cref="RealWorldCatalogData"/> missing in the DB.</summary>
    private static async Task SyncCatalogDefinitionsAsync(ApplicationDbContext context)
    {
        var rng = new Random(2026);
        var categories = await EnsureCategoriesAsync(context);
        var suppliers = await EnsureSuppliersAsync(context);
        var producers = await EnsureProducersAsync(context);
        var artists = await EnsureArtistsAsync(context);
        var musicStudios = await EnsureMusicStudiosAsync(context);
        var gameStudios = await EnsureGameStudiosAsync(context);
        var filmStudios = await EnsureFilmStudiosAsync(context);
        var albums = await EnsureMusicAlbumsAsync(context, categories, artists, rng);
        var games = await EnsureGamesAsync(context, categories, gameStudios, rng);
        var movies = await EnsureMoviesAsync(context, categories, filmStudios, rng);
        await EnsureProductsAsync(context, albums, games, movies, suppliers, producers, rng);
        await EnsureSongsAsync(context, albums);
        await StudioDataMigration.MigrateAsync(context);
    }

    private static async Task ReplaceLegacyCatalogAsync(ApplicationDbContext context)
    {
        context.Wishlists.RemoveRange(await context.Wishlists.ToListAsync());
        context.Reviews.RemoveRange(await context.Reviews.ToListAsync());

        var orders = await context.Orders.Include(o => o.OrderItems).ToListAsync();
        foreach (var order in orders)
            context.OrderItems.RemoveRange(order.OrderItems);
        context.Orders.RemoveRange(orders);

        context.Songs.RemoveRange(await context.Songs.ToListAsync());
        context.Games.RemoveRange(await context.Games.ToListAsync());
        context.Movies.RemoveRange(await context.Movies.ToListAsync());
        context.Products.RemoveRange(await context.Products.ToListAsync());
        context.Albums.RemoveRange(await context.Albums.ToListAsync());

        var fakeArtists = await context.Artists
            .Where(a => RealWorldCatalogData.LegacyFakeArtistNames.Contains(a.Name))
            .ToListAsync();
        context.Artists.RemoveRange(fakeArtists);

        var legacyNews = await context.News
            .Where(n => n.Title == RealWorldCatalogData.LegacyFakeMarker
                || n.Title == "CineVault Mega Sale Announced"
                || n.Title.StartsWith("Archived:"))
            .ToListAsync();
        context.News.RemoveRange(legacyNews);

        await context.SaveChangesAsync();

        // Forum replies reference parent posts — delete children before parents
        await DeleteAllForumPostsAsync(context);
    }

    private static async Task DeleteAllForumPostsAsync(ApplicationDbContext context)
    {
        while (await context.ForumPosts.AnyAsync(p => p.ParentPostId != null))
        {
            context.ForumPosts.RemoveRange(
                await context.ForumPosts.Where(p => p.ParentPostId != null).ToListAsync());
            await context.SaveChangesAsync();
        }

        context.ForumPosts.RemoveRange(await context.ForumPosts.ToListAsync());
        await context.SaveChangesAsync();
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in new[] { "Member", "Admin" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task EnsureCoreUsersAsync(UserManager<ApplicationUser> userManager)
    {
        if (await userManager.FindByNameAsync("admin") == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@dvdshop.com",
                EmailConfirmed = true,
                ProfileImageUrl = ImageUrlHelper.SampleAvatar("admin"),
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddMonths(-6)
            };
            await userManager.CreateAsync(admin, "Admin@123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        if (await userManager.FindByNameAsync("member") == null)
        {
            var member = new ApplicationUser
            {
                UserName = "member",
                Email = "member@dvdshop.com",
                EmailConfirmed = true,
                Balance = 500,
                ProfileImageUrl = ImageUrlHelper.SampleAvatar("member"),
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddMonths(-3)
            };
            await userManager.CreateAsync(member, "Member@123");
            await userManager.AddToRoleAsync(member, "Member");
        }
    }

    private static async Task<List<Category>> EnsureCategoriesAsync(ApplicationDbContext context)
    {
        await EnforceCategoryCatalogAsync(context);
        return await context.Categories
            .Where(c => CatalogCategories.AllGenreNames.Contains(c.Name))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    /// <summary>Genre-only categories; migrates away Music/Games/Movies rows. Runs every startup.</summary>
    public static async Task EnforceCategoryCatalogAsync(ApplicationDbContext context)
    {
        var byName = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);

        foreach (var (name, desc) in CatalogCategories.GetGenreDefinitions(mediaType: null))
        {
            var existing = await context.Categories.FirstOrDefaultAsync(c => c.Name == name);
            if (existing != null)
            {
                existing.Description = desc;
                byName[name] = existing;
            }
            else
            {
                var cat = new Category { Name = name, Description = desc };
                context.Categories.Add(cat);
                byName[name] = cat;
            }
        }

        await context.SaveChangesAsync();

        foreach (var cat in await context.Categories.ToListAsync())
            byName.TryAdd(cat.Name, cat);

        foreach (var (obsolete, genreName) in CatalogCategories.ObsoleteMediaTypeCategories)
        {
            if (!byName.TryGetValue(obsolete, out var obsoleteCat))
                continue;
            if (!byName.TryGetValue(genreName, out var target))
                continue;

            foreach (var album in await context.Albums.Where(a => a.CategoryId == obsoleteCat.CategoryId).ToListAsync())
                album.CategoryId = target.CategoryId;
            foreach (var game in await context.Games.Where(g => g.CategoryId == obsoleteCat.CategoryId).ToListAsync())
                game.CategoryId = target.CategoryId;
            foreach (var movie in await context.Movies.Where(m => m.CategoryId == obsoleteCat.CategoryId).ToListAsync())
                movie.CategoryId = target.CategoryId;

            context.Categories.Remove(obsoleteCat);
            byName.Remove(obsolete);
        }

        await context.SaveChangesAsync();
    }

    private static async Task<List<Supplier>> EnsureSuppliersAsync(ApplicationDbContext context)
    {
        var list = new List<Supplier>();
        foreach (var row in RealWorldCatalogData.Suppliers)
        {
            var existing = await context.Suppliers.FirstOrDefaultAsync(s => s.Name == row.Name);
            if (existing != null) { list.Add(existing); continue; }
            var s = new Supplier { Name = row.Name, Contact = row.Contact, Address = row.Address, IsActive = row.Active };
            context.Suppliers.Add(s);
            list.Add(s);
        }
        await context.SaveChangesAsync();
        return list;
    }

    private static async Task<List<Producer>> EnsureProducersAsync(ApplicationDbContext context)
    {
        var list = new List<Producer>();
        foreach (var row in RealWorldCatalogData.Producers)
        {
            var existing = await context.Producers.FirstOrDefaultAsync(p => p.Name == row.Name);
            if (existing != null) { list.Add(existing); continue; }
            var p = new Producer { Name = row.Name, Contact = row.Contact, Address = row.Address, IsActive = row.Active };
            context.Producers.Add(p);
            list.Add(p);
        }
        await context.SaveChangesAsync();
        return list;
    }

    private static async Task<List<Artist>> EnsureArtistsAsync(ApplicationDbContext context)
    {
        var list = new List<Artist>();
        foreach (var (name, bio, imageUrl) in RealWorldCatalogData.Artists)
        {
            var existing = await context.Artists.FirstOrDefaultAsync(a => a.Name == name);
            if (existing != null)
            {
                existing.Bio = bio;
                var photo = imageUrl ?? CatalogCoverUrls.GetArtistPhoto(name);
                if (!string.IsNullOrEmpty(photo))
                    existing.ImageUrl = photo;
                else if (ImageUrlHelper.IsLocalPlaceholder(existing.ImageUrl))
                    existing.ImageUrl = ImageUrlHelper.SampleArtist(name);
                list.Add(existing);
            }
            else
            {
                var artist = new Artist
                {
                    Name = name,
                    Bio = bio,
                    ImageUrl = imageUrl ?? CatalogCoverUrls.GetArtistPhoto(name) ?? ImageUrlHelper.SampleArtist(name)
                };
                context.Artists.Add(artist);
                list.Add(artist);
            }
        }

        await context.SaveChangesAsync();
        return list;
    }

    private static bool ShouldReplaceStudioImageUrl(string? url) =>
        string.IsNullOrWhiteSpace(url)
        || ImageUrlHelper.IsLocalPlaceholder(url)
        || url.Contains("placeholder-album", StringComparison.OrdinalIgnoreCase)
        || url.Contains("picsum.photos", StringComparison.OrdinalIgnoreCase);

    private static async Task<List<MusicStudio>> EnsureMusicStudiosAsync(ApplicationDbContext context)
    {
        var list = new List<MusicStudio>();
        foreach (var (name, bio, imageUrl) in RealWorldCatalogData.MusicStudios)
        {
            var existing = await context.MusicStudios.FirstOrDefaultAsync(s => s.Name == name);
            if (existing != null)
            {
                existing.Bio = bio;
                if (!string.IsNullOrEmpty(imageUrl))
                    existing.ImageUrl = imageUrl;
                else if (ShouldReplaceStudioImageUrl(existing.ImageUrl))
                {
                    var img = CatalogCoverUrls.GetMusicStudioImage(name);
                    if (!string.IsNullOrEmpty(img))
                        existing.ImageUrl = img;
                }
                list.Add(existing);
            }
            else
            {
                var studio = new MusicStudio { Name = name, Bio = bio, ImageUrl = imageUrl ?? CatalogCoverUrls.GetMusicStudioImage(name) ?? ImageUrlHelper.SampleArtist(name) };
                context.MusicStudios.Add(studio);
                list.Add(studio);
            }
        }
        await context.SaveChangesAsync();
        return list;
    }

    private static async Task<List<GameStudio>> EnsureGameStudiosAsync(ApplicationDbContext context)
    {
        var list = new List<GameStudio>();
        foreach (var (name, bio, imageUrl) in RealWorldCatalogData.GameStudios)
        {
            var existing = await context.GameStudios.FirstOrDefaultAsync(s => s.Name == name);
            if (existing != null)
            {
                existing.Bio = bio;
                if (!string.IsNullOrEmpty(imageUrl))
                    existing.ImageUrl = imageUrl;
                else if (ShouldReplaceStudioImageUrl(existing.ImageUrl))
                {
                    var img = CatalogCoverUrls.GetGameStudioImage(name);
                    if (!string.IsNullOrEmpty(img))
                        existing.ImageUrl = img;
                }
                list.Add(existing);
            }
            else
            {
                var studio = new GameStudio { Name = name, Bio = bio, ImageUrl = imageUrl ?? CatalogCoverUrls.GetGameStudioImage(name) };
                context.GameStudios.Add(studio);
                list.Add(studio);
            }
        }
        await PruneExtraGameStudiosAsync(context);
        await context.SaveChangesAsync();
        return list;
    }

    /// <summary>Refreshes film studio names, bios, and logos from <see cref="RealWorldCatalogData.FilmStudios"/>.</summary>
    public static async Task EnforceFilmStudioCatalogAsync(ApplicationDbContext context) =>
        await EnsureFilmStudiosAsync(context);

    /// <summary>Upserts giant publishers only, removes extras, and re-links games. Runs every startup.</summary>
    public static async Task EnforceGameStudioCatalogAsync(ApplicationDbContext context)
    {
        var studios = await EnsureGameStudiosAsync(context);
        await RelinkOrphanedGameStudiosAsync(context, studios);
        await RelinkGameStudiosAsync(context, studios);
        await context.SaveChangesAsync();
    }

    private static string ResolveGiantStudioName(string name)
    {
        if (RealWorldCatalogData.GameStudioAliases.TryGetValue(name, out var giant))
            return giant;
        return name;
    }

    private static async Task RelinkGameStudiosAsync(ApplicationDbContext context, List<GameStudio> studios)
    {
        var byName = studios.ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);
        foreach (var row in RealWorldCatalogData.Releases.Where(r => r.Media == MediaType.Game))
        {
            var studioName = ResolveGiantStudioName(row.Artist);
            if (!byName.TryGetValue(studioName, out var studio))
                continue;

            var game = await context.Games.FirstOrDefaultAsync(g => g.Title == row.Title);
            if (game != null)
                game.GameStudioId = studio.GameStudioId;
        }
    }

    /// <summary>Re-links any game still tied to a removed studio name via <see cref="RealWorldCatalogData.GameStudioAliases"/>.</summary>
    private static async Task RelinkOrphanedGameStudiosAsync(ApplicationDbContext context, List<GameStudio> giants)
    {
        var byName = giants.ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);
        var allowed = RealWorldCatalogData.GameStudios
            .Select(s => s.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var game in await context.Games.Include(g => g.GameStudio).ToListAsync())
        {
            if (game.GameStudio == null || allowed.Contains(game.GameStudio.Name))
                continue;

            var targetName = ResolveGiantStudioName(game.GameStudio.Name);
            if (byName.TryGetValue(targetName, out var giant))
                game.GameStudioId = giant.GameStudioId;
        }
    }

    /// <summary>Removes game studios not in <see cref="RealWorldCatalogData.GameStudios"/>.</summary>
    private static async Task PruneExtraGameStudiosAsync(ApplicationDbContext context)
    {
        var allowed = RealWorldCatalogData.GameStudios
            .Select(s => s.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var byName = (await context.GameStudios.Where(s => allowed.Contains(s.Name)).ToListAsync())
            .ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);

        var extras = (await context.GameStudios.ToListAsync())
            .Where(s => !allowed.Contains(s.Name))
            .ToList();

        foreach (var studio in extras)
        {
            var games = await context.Games
                .Where(g => g.GameStudioId == studio.GameStudioId)
                .ToListAsync();

            var targetName = ResolveGiantStudioName(studio.Name);
            if (!byName.TryGetValue(targetName, out var fallback))
                fallback = byName.Values.OrderBy(s => s.GameStudioId).FirstOrDefault();

            if (fallback != null)
            {
                foreach (var game in games)
                    game.GameStudioId = fallback.GameStudioId;
            }

            context.GameStudios.Remove(studio);
        }
    }

    private static async Task<List<FilmStudio>> EnsureFilmStudiosAsync(ApplicationDbContext context)
    {
        var list = new List<FilmStudio>();
        foreach (var (name, bio, imageUrl) in RealWorldCatalogData.FilmStudios)
        {
            var existing = await context.FilmStudios.FirstOrDefaultAsync(s => s.Name == name);
            if (existing != null)
            {
                existing.Bio = bio;
                if (!string.IsNullOrEmpty(imageUrl))
                    existing.ImageUrl = imageUrl;
                else if (ShouldReplaceStudioImageUrl(existing.ImageUrl))
                {
                    var img = CatalogCoverUrls.GetFilmStudioImage(name);
                    if (!string.IsNullOrEmpty(img))
                        existing.ImageUrl = img;
                }
                list.Add(existing);
            }
            else
            {
                var studio = new FilmStudio
                {
                    Name = name,
                    Bio = bio,
                    ImageUrl = imageUrl
                        ?? CatalogCoverUrls.GetFilmStudioImage(name)
                        ?? ImageUrlHelper.SampleArtist(name)
                };
                context.FilmStudios.Add(studio);
                list.Add(studio);
            }
        }
        await context.SaveChangesAsync();
        return list;
    }

    private static async Task<List<Album>> EnsureMusicAlbumsAsync(
        ApplicationDbContext context,
        List<Category> categories,
        List<Artist> artists,
        Random rng)
    {
        Category Cat(string name) => categories.First(c => c.Name == name);
        Artist Art(string name) => artists.First(a => a.Name == name);

        var musicReleases = RealWorldCatalogData.Releases.Where(r => r.Media == MediaType.Music).ToList();
        var existingByTitle = await context.Albums
            .Where(a => musicReleases.Select(r => r.Title).Contains(a.Title))
            .ToDictionaryAsync(a => a.Title);

        var list = new List<Album>();
        foreach (var row in musicReleases)
        {
            if (existingByTitle.TryGetValue(row.Title, out var existing))
            {
                existing.CoverImageUrl = CatalogCoverUrls.GetCoverUrl(row.Title) ?? row.CoverUrl;
                existing.Description = row.Description;
                existing.CategoryId = Cat(row.Category).CategoryId;
                existing.ArtistId = Art(row.Artist).ArtistId;
                list.Add(existing);
                continue;
            }

            var album = new Album
            {
                Title = row.Title,
                CategoryId = Cat(row.Category).CategoryId,
                ArtistId = Art(row.Artist).ArtistId,
                ReleaseDate = DateTime.UtcNow.AddMonths(-row.MonthsAgo).AddDays(-rng.Next(0, 28)),
                CoverImageUrl = CatalogCoverUrls.GetCoverUrl(row.Title) ?? row.CoverUrl,
                Description = row.Description
            };
            context.Albums.Add(album);
            list.Add(album);
        }

        await context.SaveChangesAsync();
        return list;
    }

    private static async Task<List<Game>> EnsureGamesAsync(
        ApplicationDbContext context,
        List<Category> categories,
        List<GameStudio> gameStudios,
        Random rng)
    {
        Category Cat(string name) => categories.First(c => c.Name == name);
        GameStudio Studio(string name) => gameStudios.First(s => s.Name == name);

        var gameReleases = RealWorldCatalogData.Releases.Where(r => r.Media == MediaType.Game).ToList();
        var existingByTitle = await context.Games
            .Where(g => gameReleases.Select(r => r.Title).Contains(g.Title))
            .ToDictionaryAsync(g => g.Title);

        var list = new List<Game>();
        foreach (var row in gameReleases)
        {
            if (existingByTitle.TryGetValue(row.Title, out var existing))
            {
                existing.CoverImageUrl = CatalogCoverUrls.GetCoverUrl(row.Title) ?? row.CoverUrl;
                existing.ImageUrl = CatalogCoverUrls.GetLandscapeBannerUrl(row.Title) ?? existing.CoverImageUrl;
                existing.Description = row.Description;
                existing.CategoryId = Cat(row.Category).CategoryId;
                existing.GameStudioId = Studio(row.Artist).GameStudioId;
                list.Add(existing);
                continue;
            }

            var game = new Game
            {
                Title = row.Title,
                CategoryId = Cat(row.Category).CategoryId,
                GameStudioId = Studio(row.Artist).GameStudioId,
                ReleaseDate = DateTime.UtcNow.AddMonths(-row.MonthsAgo).AddDays(-rng.Next(0, 28)),
                CoverImageUrl = CatalogCoverUrls.GetCoverUrl(row.Title) ?? row.CoverUrl,
                ImageUrl = CatalogCoverUrls.GetLandscapeBannerUrl(row.Title) ?? CatalogCoverUrls.GetCoverUrl(row.Title) ?? row.CoverUrl,
                PreviewUrl = MediaCatalogLinks.GetGameTrailer(row.Title),
                Description = row.Description
            };
            context.Games.Add(game);
            list.Add(game);
        }

        await context.SaveChangesAsync();
        return list;
    }

    private static async Task<List<Movie>> EnsureMoviesAsync(
        ApplicationDbContext context,
        List<Category> categories,
        List<FilmStudio> filmStudios,
        Random rng)
    {
        Category Cat(string name) => categories.First(c => c.Name == name);
        FilmStudio Studio(string name) => filmStudios.First(s => s.Name == name);

        var movieReleases = RealWorldCatalogData.Releases.Where(r => r.Media == MediaType.Movie).ToList();
        var existingByTitle = await context.Movies
            .Where(m => movieReleases.Select(r => r.Title).Contains(m.Title))
            .ToDictionaryAsync(m => m.Title);

        var list = new List<Movie>();
        foreach (var row in movieReleases)
        {
            if (existingByTitle.TryGetValue(row.Title, out var existing))
            {
                existing.CoverImageUrl = CatalogCoverUrls.GetCoverUrl(row.Title) ?? row.CoverUrl;
                existing.ImageUrl = CatalogCoverUrls.GetLandscapeBannerUrl(row.Title) ?? existing.CoverImageUrl;
                existing.Description = row.Description;
                existing.CategoryId = Cat(row.Category).CategoryId;
                existing.FilmStudioId = Studio(row.Artist).FilmStudioId;
                list.Add(existing);
                continue;
            }

            var movie = new Movie
            {
                Title = row.Title,
                CategoryId = Cat(row.Category).CategoryId,
                FilmStudioId = Studio(row.Artist).FilmStudioId,
                ReleaseDate = DateTime.UtcNow.AddMonths(-row.MonthsAgo).AddDays(-rng.Next(0, 28)),
                CoverImageUrl = CatalogCoverUrls.GetCoverUrl(row.Title) ?? row.CoverUrl,
                ImageUrl = CatalogCoverUrls.GetLandscapeBannerUrl(row.Title) ?? CatalogCoverUrls.GetCoverUrl(row.Title) ?? row.CoverUrl,
                PreviewUrl = MediaCatalogLinks.GetMovieTrailer(row.Title),
                Description = row.Description
            };
            context.Movies.Add(movie);
            list.Add(movie);
        }

        await context.SaveChangesAsync();
        return list;
    }

    private static async Task<List<Product>> EnsureProductsAsync(
        ApplicationDbContext context,
        List<Album> albums,
        List<Game> games,
        List<Movie> movies,
        List<Supplier> suppliers,
        List<Producer> producers,
        Random rng)
    {
        var activeSuppliers = suppliers.Where(s => s.IsActive).ToList();
        var activeProducers = producers.Where(p => p.IsActive).ToList();
        var list = new List<Product>();
        var stockLevels = new[] { 45, 32, 28, 18, 12, 10, 8, 5, 3, 55, 40, 22, 15, 25, 30 };
        var index = 0;

        foreach (var album in albums)
        {
            var release = RealWorldCatalogData.Releases.First(r => r.Title == album.Title);
            var existing = await context.Products.FirstOrDefaultAsync(p => p.AlbumId == album.AlbumId);
            if (existing != null) { existing.Price = release.Price; list.Add(existing); }
            else
            {
                var product = new Product
                {
                    AlbumId = album.AlbumId,
                    Price = release.Price,
                    Stock = stockLevels[index % stockLevels.Length],
                    IsAvailable = stockLevels[index % stockLevels.Length] > 0,
                    SupplierId = activeSuppliers[rng.Next(activeSuppliers.Count)].SupplierId,
                    ProducerId = activeProducers[rng.Next(activeProducers.Count)].ProducerId
                };
                context.Products.Add(product);
                list.Add(product);
            }
            index++;
        }

        foreach (var game in games)
        {
            var release = RealWorldCatalogData.Releases.First(r => r.Title == game.Title);
            var existing = await context.Products.FirstOrDefaultAsync(p => p.GameId == game.GameId);
            if (existing != null) { existing.Price = release.Price; list.Add(existing); }
            else
            {
                context.Products.Add(new Product
                {
                    GameId = game.GameId,
                    Price = release.Price,
                    Stock = stockLevels[index % stockLevels.Length],
                    IsAvailable = stockLevels[index % stockLevels.Length] > 0,
                    SupplierId = activeSuppliers[rng.Next(activeSuppliers.Count)].SupplierId,
                    ProducerId = activeProducers[rng.Next(activeProducers.Count)].ProducerId
                });
            }
            index++;
        }

        foreach (var movie in movies)
        {
            var release = RealWorldCatalogData.Releases.First(r => r.Title == movie.Title);
            var existing = await context.Products.FirstOrDefaultAsync(p => p.MovieId == movie.MovieId);
            if (existing != null) { existing.Price = release.Price; list.Add(existing); }
            else
            {
                context.Products.Add(new Product
                {
                    MovieId = movie.MovieId,
                    Price = release.Price,
                    Stock = stockLevels[index % stockLevels.Length],
                    IsAvailable = stockLevels[index % stockLevels.Length] > 0,
                    SupplierId = activeSuppliers[rng.Next(activeSuppliers.Count)].SupplierId,
                    ProducerId = activeProducers[rng.Next(activeProducers.Count)].ProducerId
                });
            }
            index++;
        }

        await context.SaveChangesAsync();
        return list;
    }

    private static async Task EnsureSongsAsync(ApplicationDbContext context, List<Album> albums)
    {
        foreach (var album in albums)
        {
            if (await context.Songs.AnyAsync(s => s.AlbumId == album.AlbumId))
                continue;

            if (!RealWorldCatalogData.MusicTracks.TryGetValue(album.Title, out var tracks))
                tracks = ["Track 1", "Track 2", "Track 3"];

            for (var t = 0; t < tracks.Length; t++)
            {
                var preview = MediaCatalogLinks.GetMusicPreview(album.Title);
                context.Songs.Add(new Song
                {
                    AlbumId = album.AlbumId,
                    Title = tracks[t],
                    ImageUrl = album.CoverImageUrl,
                    DurationSeconds = 180 + t * 45,
                    PreviewUrl = preview,
                    DownloadUrl = preview,
                    IsFree = t == 0
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureNewsPromotionsAdsAsync(ApplicationDbContext context)
    {
        if (!await context.Promotions.AnyAsync(p => p.Title == "Music DVD Weekend"))
        {
            context.Promotions.AddRange(
                new Promotion { Title = "Music DVD Weekend", DiscountPercent = 10, StartDate = DateTime.UtcNow.AddDays(-7), EndDate = DateTime.UtcNow.AddDays(30), IsActive = true },
                new Promotion { Title = "New Member Welcome", DiscountPercent = 15, StartDate = DateTime.UtcNow.AddDays(-30), EndDate = DateTime.UtcNow.AddDays(60), IsActive = true },
                new Promotion { Title = "Game Week", DiscountPercent = 20, StartDate = DateTime.UtcNow.AddDays(-3), EndDate = DateTime.UtcNow.AddDays(4), IsActive = true },
                new Promotion { Title = "Movie Marathon", DiscountPercent = 12, StartDate = DateTime.UtcNow.AddDays(5), EndDate = DateTime.UtcNow.AddDays(20), IsActive = true }
            );
        }

        if (!await context.Advertisements.AnyAsync())
        {
            context.Advertisements.AddRange(MediaCatalogLinks.Banners.Select(b => new Advertisement
            {
                ImageUrl = b.ImageUrl,
                LinkUrl = b.LinkUrl,
                Position = b.Position,
                IsActive = true
            }));
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsurePurchasingInvoicesAsync(
        ApplicationDbContext context,
        List<Supplier> suppliers,
        Random rng)
    {
        if (await context.PurchasingInvoices.CountAsync() >= 12)
            return;

        var active = suppliers.Where(s => s.IsActive).ToList();
        for (var i = 0; i < 10; i++)
        {
            var supplier = active[rng.Next(active.Count)];
            context.PurchasingInvoices.Add(new PurchasingInvoice
            {
                SupplierId = supplier.SupplierId,
                InvoiceDate = DateTime.UtcNow.AddDays(-rng.Next(5, 180)),
                Amount = Math.Round(800m + rng.Next(200, 8000) + rng.Next(0, 99) / 100m, 2),
                Notes = $"Restock — {supplier.Name}"
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task<List<ApplicationUser>> EnsureTestMembersAsync(
        UserManager<ApplicationUser> userManager,
        Random rng)
    {
        var usernames = new[] { "jsmith", "awalker", "mchen", "rbrown", "klee" };

        var members = new List<ApplicationUser>();
        var memberUser = await userManager.FindByNameAsync("member");
        if (memberUser != null) members.Add(memberUser);

        foreach (var name in usernames)
        {
            var user = await userManager.FindByNameAsync(name);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = name,
                    Email = $"{name}@example.com",
                    EmailConfirmed = true,
                    Balance = rng.Next(50, 400),
                    ProfileImageUrl = ImageUrlHelper.SampleAvatar(name),
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow.AddDays(-rng.Next(10, 400))
                };
                var result = await userManager.CreateAsync(user, MemberPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "Member");
                else
                    continue;
            }

            if (await userManager.IsInRoleAsync(user, "Member"))
                members.Add(user);
        }

        return members.DistinctBy(u => u.Id).ToList();
    }

    private static async Task EnsureOrdersAsync(
        ApplicationDbContext context,
        List<ApplicationUser> members,
        List<Product> products,
        Random rng)
    {
        if (await context.Orders.CountAsync() >= 20)
            return;

        var available = products.Where(p => p.IsAvailable).ToList();
        if (available.Count == 0 || members.Count == 0)
            return;

        var statuses = new[] { OrderStatus.Pending, OrderStatus.Processing, OrderStatus.Shipped, OrderStatus.Delivered };
        var weights = new[] { 6, 6, 10, 25 };

        for (var i = 0; i < 25; i++)
        {
            var user = members[rng.Next(members.Count)];
            var orderDate = DateTime.UtcNow.AddDays(-rng.Next(0, 180));
            var status = WeightedStatus(rng, statuses, weights);
            var picked = available.OrderBy(_ => rng.Next()).Take(rng.Next(1, 3)).ToList();
            var lineItems = new List<OrderItem>();
            decimal subtotal = 0;

            foreach (var product in picked)
            {
                var qty = 1;
                lineItems.Add(new OrderItem { ProductId = product.ProductId, Quantity = qty, UnitPrice = product.Price });
                subtotal += product.Price * qty;
            }

            context.Orders.Add(new Order
            {
                UserId = user.Id,
                OrderDate = orderDate,
                Status = status,
                TotalAmount = subtotal,
                DiscountApplied = 0,
                OrderItems = lineItems
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureReviewsAsync(
        ApplicationDbContext context,
        List<ApplicationUser> members,
        List<Product> products,
        Random rng)
    {
        if (await context.Reviews.CountAsync() >= 15)
            return;

        var comments = new[]
        {
            "Great DVD quality — Dark Knight looks amazing.",
            "Taylor Swift 1989 deluxe was worth every penny.",
            "GTA V bundle shipped fast.",
            "Kind of Blue — essential for any jazz collection.",
            "Avengers Endgame: perfect for movie night."
        };

        var used = new HashSet<(string, int)>();
        for (var i = 0; i < 20 && used.Count < 20; i++)
        {
            var user = members[rng.Next(members.Count)];
            var product = products[rng.Next(products.Count)];
            if (!used.Add((user.Id, product.ProductId))) continue;

            context.Reviews.Add(new Review
            {
                UserId = user.Id,
                ProductId = product.ProductId,
                Rating = rng.Next(4, 6),
                Comment = comments[rng.Next(comments.Length)],
                CreatedDate = DateTime.UtcNow.AddDays(-rng.Next(1, 90))
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureFeedbackAsync(
        ApplicationDbContext context,
        List<ApplicationUser> members,
        Random rng)
    {
        if (await context.Feedbacks.CountAsync() >= 5)
            return;

        var messages = new[]
        {
            "Do you have Interstellar in 4K DVD?",
            "When will Elden Ring restock?",
            "Love the real music catalog!",
            "Can I order Avengers Endgame with express shipping?"
        };

        foreach (var msg in messages)
        {
            context.Feedbacks.Add(new Feedback
            {
                UserId = members[rng.Next(members.Count)].Id,
                Message = msg,
                CreatedDate = DateTime.UtcNow.AddDays(-rng.Next(1, 30)),
                AdminReply = null
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureWishlistsAsync(
        ApplicationDbContext context,
        List<ApplicationUser> members,
        List<Product> products,
        Random rng)
    {
        if (await context.Wishlists.AnyAsync())
            return;

        var used = new HashSet<(string, int)>();
        for (var i = 0; i < 12; i++)
        {
            var user = members[rng.Next(members.Count)];
            var product = products[rng.Next(products.Count)];
            if (!used.Add((user.Id, product.ProductId))) continue;

            context.Wishlists.Add(new Wishlist
            {
                UserId = user.Id,
                ProductId = product.ProductId,
                AddedDate = DateTime.UtcNow.AddDays(-rng.Next(1, 60))
            });
        }

        await context.SaveChangesAsync();
    }

    private static OrderStatus WeightedStatus(Random rng, OrderStatus[] statuses, int[] weights)
    {
        var total = weights.Sum();
        var roll = rng.Next(total);
        var sum = 0;
        for (var i = 0; i < statuses.Length; i++)
        {
            sum += weights[i];
            if (roll < sum) return statuses[i];
        }

        return OrderStatus.Delivered;
    }
}
