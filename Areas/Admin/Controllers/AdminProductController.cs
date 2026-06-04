using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminProductController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly FileUploadService _upload;
    private const int PageSize = 15;

    public AdminProductController(ApplicationDbContext context, FileUploadService upload)
    {
        _context = context;
        _upload = upload;
    }

    private async Task LoadDropdownsAsync(Product? current = null, bool forCreate = false)
    {
        var listedAlbumIds = await _context.Products.Where(p => p.AlbumId != null).Select(p => p.AlbumId!.Value).ToListAsync();
        var listedGameIds = await _context.Products.Where(p => p.GameId != null).Select(p => p.GameId!.Value).ToListAsync();
        var listedMovieIds = await _context.Products.Where(p => p.MovieId != null).Select(p => p.MovieId!.Value).ToListAsync();

        var albums = await _context.Albums
            .Include(a => a.Artist)
            .Include(a => a.MusicStudio)
            .OrderBy(a => a.Title)
            .ToListAsync();
        var games = await _context.Games.Include(g => g.GameStudio).OrderBy(g => g.Title).ToListAsync();
        var movies = await _context.Movies.Include(m => m.FilmStudio).OrderBy(m => m.Title).ToListAsync();

        if (forCreate)
        {
            albums = albums.Where(a => !listedAlbumIds.Contains(a.AlbumId) || current?.AlbumId == a.AlbumId).ToList();
            games = games.Where(g => !listedGameIds.Contains(g.GameId) || current?.GameId == g.GameId).ToList();
            movies = movies.Where(m => !listedMovieIds.Contains(m.MovieId) || current?.MovieId == m.MovieId).ToList();
        }

        ViewBag.MusicAlbums = albums;
        ViewBag.Games = games;
        ViewBag.Movies = movies;
        ViewBag.CatalogKey = current switch
        {
            { AlbumId: not null } => $"a:{current.AlbumId}",
            { GameId: not null } => $"g:{current.GameId}",
            { MovieId: not null } => $"m:{current.MovieId}",
            _ => ""
        };
        ViewBag.ProducerId = new SelectList(_context.Producers.Where(p => p.IsActive), "ProducerId", "Name");
        ViewBag.SupplierId = new SelectList(_context.Suppliers.Where(s => s.IsActive), "SupplierId", "Name");
    }

    public async Task<IActionResult> Index(string? search, MediaType? mediaType, string? available, int page = 1)
    {
        var query = _context.Products.WithCatalogIncludes().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p =>
                (p.Album != null && p.Album.Title.Contains(search))
                || (p.Album != null && p.Album.Artist != null && p.Album.Artist.Name.Contains(search))
                || (p.Game != null && p.Game.Title.Contains(search))
                || (p.Game != null && p.Game.GameStudio.Name.Contains(search))
                || (p.Movie != null && p.Movie.Title.Contains(search))
                || (p.Movie != null && p.Movie.FilmStudio.Name.Contains(search)));

        if (mediaType == MediaType.Music)
            query = query.Where(p => p.AlbumId != null);
        else if (mediaType == MediaType.Game)
            query = query.Where(p => p.GameId != null);
        else if (mediaType == MediaType.Movie)
            query = query.Where(p => p.MovieId != null);

        if (available == "true")
            query = query.Where(p => p.IsAvailable);
        else if (available == "false")
            query = query.Where(p => !p.IsAvailable);

        ViewBag.FilterSearch = search;
        ViewBag.FilterMediaType = mediaType;
        ViewBag.ShowMediaFilter = true;
        ViewBag.ShowAvailabilityFilter = true;
        ViewBag.FilterAvailableOnly = available == "true";
        ViewBag.FilterUnavailableOnly = available == "false";

        var products = await query.ToListAsync();
        var ordered = products
            .OrderBy(p => ProductCatalogHelper.GetMediaType(p))
            .ThenBy(p => ProductCatalogHelper.GetTitle(p))
            .ToList();

        return View(PaginatedList<Product>.Create(ordered, page, PageSize));
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.IsCreate = true;
        await LoadDropdownsAsync(forCreate: true);
        return View(new Product { IsAvailable = true, Stock = 10 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product model, string? catalogKey, string? coverImageUrl, IFormFile? coverFile)
    {
        ApplyCatalogKey(model, catalogKey);
        if (!HasCatalogLink(model))
            ModelState.AddModelError("", "Select a music album, game, or movie.");
        if (await IsDuplicateListingAsync(model))
            ModelState.AddModelError("", "This title already has a shop listing.");

        if (!ModelState.IsValid)
        {
            ViewBag.IsCreate = true;
            await LoadDropdownsAsync(model, forCreate: true);
            return View(model);
        }

        _context.Products.Add(model);
        await _context.SaveChangesAsync();
        await ApplyCoverAsync(model, coverImageUrl, coverFile);
        SetMessage("Product listed on shop.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Products.WithCatalogIncludes().FirstOrDefaultAsync(p => p.ProductId == id);
        if (item == null) return NotFound();
        ViewBag.CoverImageUrl = ProductCatalogHelper.GetCoverUrl(item);
        ViewBag.IsCreate = false;
        await LoadDropdownsAsync(item);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product model, string? catalogKey, string? coverImageUrl, IFormFile? coverFile)
    {
        if (id != model.ProductId) return NotFound();
        ApplyCatalogKey(model, catalogKey);
        if (!HasCatalogLink(model))
            ModelState.AddModelError("", "Select a music album, game, or movie.");
        if (!ModelState.IsValid)
        {
            ViewBag.CoverImageUrl = coverImageUrl;
            ViewBag.IsCreate = false;
            await LoadDropdownsAsync(model);
            return View(model);
        }
        _context.Update(model);
        await _context.SaveChangesAsync();
        await ApplyCoverAsync(model, coverImageUrl, coverFile);
        SetMessage("Product updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Products.WithCatalogIncludes().FirstOrDefaultAsync(p => p.ProductId == id);
        if (item == null) return RedirectToAction(nameof(Index));

        var orderCount = await _context.OrderItems.CountAsync(oi => oi.ProductId == id);
        if (orderCount > 0)
        {
            SetMessage($"Cannot delete — appears in {orderCount} order(s).", success: false);
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.Products.Remove(item);
            await _context.SaveChangesAsync();
            SetMessage("Product deleted.");
        }
        catch (DbUpdateException)
        {
            SetMessage("Cannot delete — other records depend on this product.", success: false);
        }

        return RedirectToAction(nameof(Index));
    }

    private static void ApplyCatalogKey(Product model, string? catalogKey)
    {
        model.AlbumId = null;
        model.GameId = null;
        model.MovieId = null;
        if (string.IsNullOrWhiteSpace(catalogKey)) return;

        var parts = catalogKey.Split(':', 2);
        if (parts.Length != 2 || !int.TryParse(parts[1], out var id)) return;

        switch (parts[0])
        {
            case "a": model.AlbumId = id; break;
            case "g": model.GameId = id; break;
            case "m": model.MovieId = id; break;
        }
    }

    private static bool HasCatalogLink(Product model) =>
        model.AlbumId > 0 || model.GameId > 0 || model.MovieId > 0;

    private async Task<bool> IsDuplicateListingAsync(Product model)
    {
        if (model.AlbumId.HasValue)
            return await _context.Products.AnyAsync(p => p.AlbumId == model.AlbumId && p.ProductId != model.ProductId);
        if (model.GameId.HasValue)
            return await _context.Products.AnyAsync(p => p.GameId == model.GameId && p.ProductId != model.ProductId);
        if (model.MovieId.HasValue)
            return await _context.Products.AnyAsync(p => p.MovieId == model.MovieId && p.ProductId != model.ProductId);
        return false;
    }

    private async Task ApplyCoverAsync(Product product, string? coverImageUrl, IFormFile? coverFile)
    {
        string? url = null;
        try
        {
            if (coverFile != null)
            {
                var folder = product.AlbumId != null ? "albums" : product.GameId != null ? "games" : "movies";
                url = await _upload.SaveImageAsync(coverFile, folder);
            }
            else if (!string.IsNullOrWhiteSpace(coverImageUrl))
                url = coverImageUrl.Trim();
            else
                return;
        }
        catch (Exception ex)
        {
            SetMessage($"Product saved, but cover failed: {ex.Message}", success: false);
            return;
        }

        if (product.AlbumId.HasValue)
        {
            var album = await _context.Albums.FindAsync(product.AlbumId);
            if (album != null) album.CoverImageUrl = url;
        }
        else if (product.GameId.HasValue)
        {
            var game = await _context.Games.FindAsync(product.GameId);
            if (game != null) game.CoverImageUrl = url;
        }
        else if (product.MovieId.HasValue)
        {
            var movie = await _context.Movies.FindAsync(product.MovieId);
            if (movie != null) movie.CoverImageUrl = url;
        }

        await _context.SaveChangesAsync();
    }
}
