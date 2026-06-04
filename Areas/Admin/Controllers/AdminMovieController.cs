using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminMovieController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly FileUploadService _upload;
    private const int PageSize = 15;

    public AdminMovieController(ApplicationDbContext context, FileUploadService upload)
    {
        _context = context;
        _upload = upload;
    }

    private async Task LoadReleaseDropdownsAsync(Movie? movie = null)
    {
        ViewBag.FilmStudioId = new SelectList(
            await _context.FilmStudios.OrderBy(s => s.Name).ToListAsync(),
            "FilmStudioId", "Name", movie?.FilmStudioId);
        ViewBag.CategoryId = new SelectList(
            await _context.Categories
                .Where(c => CatalogCategories.MovieGenres.Select(g => g.Name).Contains(c.Name))
                .OrderBy(c => c.Name)
                .ToListAsync(),
            "CategoryId", "Name", movie?.CategoryId);
        ViewBag.ReleaseDate = (movie?.ReleaseDate ?? DateTime.UtcNow.Date).ToString("yyyy-MM-dd");
        ViewBag.DvdCoverUrl = movie?.CoverImageUrl;
    }

    private static void ClearNavigationValidation(ModelStateDictionary modelState)
    {
        modelState.Remove(nameof(Movie.FilmStudio));
        modelState.Remove(nameof(Movie.Category));
        modelState.Remove(nameof(Movie.Products));
    }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Movies
            .Include(m => m.FilmStudio)
            .Include(m => m.Category)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(m => m.Title.Contains(search));
        return View(await PaginatedList<Movie>.CreateAsync(query.OrderBy(m => m.Title), page, PageSize));
    }

    public async Task<IActionResult> Create()
    {
        await LoadReleaseDropdownsAsync();
        return View(new Movie { ReleaseDate = DateTime.UtcNow.Date });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        Movie model,
        int filmStudioId,
        int categoryId,
        DateTime releaseDate,
        string? dvdCoverUrl,
        IFormFile? imageFile,
        IFormFile? dvdCoverFile)
    {
        ClearNavigationValidation(ModelState);
        if (filmStudioId <= 0) ModelState.AddModelError("", "Select a film studio.");
        if (categoryId <= 0) ModelState.AddModelError("", "Select a category.");
        if (!ModelState.IsValid) { await LoadReleaseDropdownsAsync(); return View(model); }

        var cover = dvdCoverUrl?.Trim();
        try
        {
            if (dvdCoverFile != null) cover = await _upload.SaveImageAsync(dvdCoverFile, "movies");
        }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); await LoadReleaseDropdownsAsync(); return View(model); }

        model.FilmStudioId = filmStudioId;
        model.CategoryId = categoryId;
        model.ReleaseDate = releaseDate;
        model.CoverImageUrl = cover;

        try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "movies") ?? model.ImageUrl ?? cover; }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); await LoadReleaseDropdownsAsync(); return View(model); }

        _context.Movies.Add(model);
        await _context.SaveChangesAsync();
        SetMessage("Movie DVD added. List it on the shop under Products.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Movies.FirstOrDefaultAsync(m => m.MovieId == id);
        if (item == null) return NotFound();
        await LoadReleaseDropdownsAsync(item);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        Movie model,
        int filmStudioId,
        int categoryId,
        DateTime releaseDate,
        string? dvdCoverUrl,
        IFormFile? imageFile,
        IFormFile? dvdCoverFile)
    {
        if (id != model.MovieId) return NotFound();
        ClearNavigationValidation(ModelState);
        if (filmStudioId <= 0 || categoryId <= 0)
            ModelState.AddModelError("", "Studio and category are required.");
        if (!ModelState.IsValid)
        {
            await LoadReleaseDropdownsAsync(model);
            return View(model);
        }

        model.FilmStudioId = filmStudioId;
        model.CategoryId = categoryId;
        model.ReleaseDate = releaseDate;

        try
        {
            if (dvdCoverFile != null)
                model.CoverImageUrl = await _upload.SaveImageAsync(dvdCoverFile, "movies");
            else if (!string.IsNullOrWhiteSpace(dvdCoverUrl))
                model.CoverImageUrl = dvdCoverUrl.Trim();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadReleaseDropdownsAsync(model);
            return View(model);
        }

        if (imageFile != null)
        {
            try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "movies"); }
            catch (Exception ex) { ModelState.AddModelError("", ex.Message); await LoadReleaseDropdownsAsync(model); return View(model); }
        }

        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("Movie updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Movies.FirstOrDefaultAsync(m => m.MovieId == id);
        if (item == null) return RedirectToAction(nameof(Index));

        var inOrders = await _context.OrderItems.AnyAsync(oi => oi.Product.MovieId == id);
        if (inOrders)
        {
            SetMessage($"Cannot delete \"{item.Title}\" — it appears in customer orders.", success: false);
            return RedirectToAction(nameof(Index));
        }

        _context.Movies.Remove(item);
        await _context.SaveChangesAsync();
        SetMessage("Movie deleted.");
        return RedirectToAction(nameof(Index));
    }
}
