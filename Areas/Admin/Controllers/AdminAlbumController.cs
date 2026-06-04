using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

/// <summary>Music albums only — games and movies are managed under Games / Movies.</summary>
public class AdminAlbumController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly FileUploadService _upload;
    private const int PageSize = 15;

    public AdminAlbumController(ApplicationDbContext context, FileUploadService upload)
    {
        _context = context;
        _upload = upload;
    }

    private async Task LoadDropdownsAsync(int? selectedArtistId = null, int? selectedMusicStudioId = null, int? selectedCategoryId = null)
    {
        ViewBag.ArtistId = new SelectList(
            await _context.Artists.OrderBy(a => a.Name).ToListAsync(),
            "ArtistId", "Name", selectedArtistId);
        ViewBag.MusicStudioId = new SelectList(
            await _context.MusicStudios.OrderBy(s => s.Name).ToListAsync(),
            "MusicStudioId", "Name", selectedMusicStudioId);
        ViewBag.CategoryId = new SelectList(
            await _context.Categories
                .Where(c => CatalogCategories.MusicGenres.Select(g => g.Name).Contains(c.Name))
                .OrderBy(c => c.Name)
                .ToListAsync(),
            "CategoryId", "Name", selectedCategoryId);
    }

    /// <summary>Album posts only FK ids; navigation properties must not participate in MVC validation.</summary>
    private static void ClearAlbumNavigationValidation(ModelStateDictionary modelState)
    {
        modelState.Remove(nameof(Album.Category));
        modelState.Remove(nameof(Album.Artist));
        modelState.Remove(nameof(Album.MusicStudio));
        modelState.Remove(nameof(Album.Products));
        modelState.Remove(nameof(Album.Songs));
    }

    private static IQueryable<Album> MusicAlbums(ApplicationDbContext context) => context.Albums;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = MusicAlbums(_context)
            .Include(a => a.Artist)
            .Include(a => a.MusicStudio)
            .Include(a => a.Category)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(a => a.Title.Contains(search));
        return View(await PaginatedList<Album>.CreateAsync(query.OrderByDescending(a => a.ReleaseDate), page, PageSize));
    }

    public async Task<IActionResult> Create()
    {
        await LoadDropdownsAsync();
        return View(new Album { ReleaseDate = DateTime.UtcNow.Date, BannerVisibility = 50 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Album model, IFormFile? coverFile, IFormFile? bannerFile)
    {
        ClearAlbumNavigationValidation(ModelState);
        if (string.IsNullOrWhiteSpace(model.Title))
            ModelState.AddModelError(nameof(Album.Title), "Enter an album title.");
        if (!model.ArtistId.HasValue || model.ArtistId <= 0)
            ModelState.AddModelError(nameof(Album.ArtistId), "Select an artist (singer / composer).");
        if (model.CategoryId <= 0)
            ModelState.AddModelError(nameof(Album.CategoryId), "Select a category.");
        if (!ModelState.IsValid) { await LoadDropdownsAsync(model.ArtistId, model.MusicStudioId, model.CategoryId); return View(model); }
        try
        {
            model.CoverImageUrl = await _upload.SaveImageAsync(coverFile, "albums") ?? model.CoverImageUrl;
            model.BannerImageUrl = await _upload.SaveImageAsync(bannerFile, "album-banners") ?? model.BannerImageUrl;
            if (!string.IsNullOrWhiteSpace(model.BannerImageUrl))
                model.BannerImageUrl = await _upload.SaveImageFromUrlAsync(model.BannerImageUrl.Trim(), "album-banners")
                    ?? model.BannerImageUrl.Trim();
            model.BannerVisibility = Math.Clamp(model.BannerVisibility, 0, 100);
        }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); await LoadDropdownsAsync(model.ArtistId, model.MusicStudioId, model.CategoryId); return View(model); }
        try
        {
            _context.Albums.Add(model);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Could not save the album. Check that artist and category are selected.");
            await LoadDropdownsAsync(model.ArtistId, model.MusicStudioId, model.CategoryId);
            return View(model);
        }
        SetMessage("Music album created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await MusicAlbums(_context).FirstOrDefaultAsync(a => a.AlbumId == id);
        if (item == null) return NotFound();
        await LoadDropdownsAsync(item.ArtistId, item.MusicStudioId);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Album model, IFormFile? coverFile, IFormFile? bannerFile)
    {
        if (id != model.AlbumId) return NotFound();
        ClearAlbumNavigationValidation(ModelState);
        if (string.IsNullOrWhiteSpace(model.Title))
            ModelState.AddModelError(nameof(Album.Title), "Enter an album title.");
        if (!model.ArtistId.HasValue || model.ArtistId <= 0)
            ModelState.AddModelError(nameof(Album.ArtistId), "Select an artist (singer / composer).");
        if (model.CategoryId <= 0)
            ModelState.AddModelError(nameof(Album.CategoryId), "Select a category.");
        if (!ModelState.IsValid) { await LoadDropdownsAsync(model.ArtistId, model.MusicStudioId, model.CategoryId); return View(model); }
        var album = await MusicAlbums(_context).FirstOrDefaultAsync(a => a.AlbumId == id);
        if (album == null) return NotFound();

        album.Title = model.Title.Trim();
        album.ArtistId = model.ArtistId;
        album.MusicStudioId = model.MusicStudioId;
        album.CategoryId = model.CategoryId;
        album.ReleaseDate = model.ReleaseDate;
        album.Description = model.Description;
        album.BannerVisibility = Math.Clamp(model.BannerVisibility, 0, 100);
        if (!string.IsNullOrWhiteSpace(model.CoverImageUrl))
            album.CoverImageUrl = model.CoverImageUrl.Trim();
        if (!string.IsNullOrWhiteSpace(model.BannerImageUrl))
            album.BannerImageUrl = await _upload.SaveImageFromUrlAsync(model.BannerImageUrl.Trim(), "album-banners")
                ?? model.BannerImageUrl.Trim();

        if (coverFile != null || bannerFile != null)
        {
            try
            {
                if (coverFile != null)
                    album.CoverImageUrl = await _upload.SaveImageAsync(coverFile, "albums");
                if (bannerFile != null)
                    album.BannerImageUrl = await _upload.SaveImageAsync(bannerFile, "album-banners");
            }
            catch (Exception ex) { ModelState.AddModelError("", ex.Message); await LoadDropdownsAsync(model.ArtistId, model.MusicStudioId, model.CategoryId); return View(model); }
        }

        await _context.SaveChangesAsync();
        SetMessage("Music album updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await MusicAlbums(_context).FirstOrDefaultAsync(a => a.AlbumId == id);
        if (item == null) return RedirectToAction(nameof(Index));

        var inOrders = await _context.OrderItems.AnyAsync(oi => oi.Product.AlbumId == id);
        if (inOrders)
        {
            SetMessage($"Cannot delete \"{item.Title}\" — it appears in customer orders.", success: false);
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.Albums.Remove(item);
            await _context.SaveChangesAsync();
            SetMessage("Music album deleted.");
        }
        catch (DbUpdateException)
        {
            SetMessage("Cannot delete this album because songs or products still depend on it.", success: false);
        }

        return RedirectToAction(nameof(Index));
    }
}
