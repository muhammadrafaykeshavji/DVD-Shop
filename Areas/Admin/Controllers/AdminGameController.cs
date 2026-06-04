using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminGameController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly FileUploadService _upload;
    private const int PageSize = 15;

    public AdminGameController(ApplicationDbContext context, FileUploadService upload)
    {
        _context = context;
        _upload = upload;
    }

    private async Task LoadReleaseDropdownsAsync(Game? game = null)
    {
        ViewBag.GameStudioId = new SelectList(
            await _context.GameStudios.OrderBy(s => s.Name).ToListAsync(),
            "GameStudioId", "Name", game?.GameStudioId);
        ViewBag.CategoryId = new SelectList(
            await _context.Categories
                .Where(c => CatalogCategories.GameGenres.Select(g => g.Name).Contains(c.Name))
                .OrderBy(c => c.Name)
                .ToListAsync(),
            "CategoryId", "Name", game?.CategoryId);
        ViewBag.ReleaseDate = (game?.ReleaseDate ?? DateTime.UtcNow.Date).ToString("yyyy-MM-dd");
        ViewBag.DvdCoverUrl = game?.CoverImageUrl;
    }

    private static void ClearNavigationValidation(ModelStateDictionary modelState)
    {
        modelState.Remove(nameof(Game.GameStudio));
        modelState.Remove(nameof(Game.Category));
        modelState.Remove(nameof(Game.Products));
    }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Games
            .Include(g => g.GameStudio)
            .Include(g => g.Category)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(g => g.Title.Contains(search));
        return View(await PaginatedList<Game>.CreateAsync(query.OrderBy(g => g.Title), page, PageSize));
    }

    public async Task<IActionResult> Create()
    {
        await LoadReleaseDropdownsAsync();
        return View(new Game { ReleaseDate = DateTime.UtcNow.Date });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        Game model,
        int gameStudioId,
        int categoryId,
        DateTime releaseDate,
        string? dvdCoverUrl,
        IFormFile? imageFile,
        IFormFile? dvdCoverFile)
    {
        ClearNavigationValidation(ModelState);
        if (gameStudioId <= 0) ModelState.AddModelError("", "Select a developer / studio.");
        if (categoryId <= 0) ModelState.AddModelError("", "Select a category.");
        if (!ModelState.IsValid) { await LoadReleaseDropdownsAsync(); return View(model); }

        var cover = dvdCoverUrl?.Trim();
        try
        {
            if (dvdCoverFile != null) cover = await _upload.SaveImageAsync(dvdCoverFile, "games");
        }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); await LoadReleaseDropdownsAsync(); return View(model); }

        model.GameStudioId = gameStudioId;
        model.CategoryId = categoryId;
        model.ReleaseDate = releaseDate;
        model.CoverImageUrl = cover;

        try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "games") ?? model.ImageUrl ?? cover; }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); await LoadReleaseDropdownsAsync(); return View(model); }

        _context.Games.Add(model);
        await _context.SaveChangesAsync();
        SetMessage("Game DVD added. List it on the shop under Products.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Games.FirstOrDefaultAsync(g => g.GameId == id);
        if (item == null) return NotFound();
        await LoadReleaseDropdownsAsync(item);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        Game model,
        int gameStudioId,
        int categoryId,
        DateTime releaseDate,
        string? dvdCoverUrl,
        IFormFile? imageFile,
        IFormFile? dvdCoverFile)
    {
        if (id != model.GameId) return NotFound();
        ClearNavigationValidation(ModelState);
        if (gameStudioId <= 0 || categoryId <= 0)
            ModelState.AddModelError("", "Studio and category are required.");
        if (!ModelState.IsValid)
        {
            await LoadReleaseDropdownsAsync(model);
            return View(model);
        }

        model.GameStudioId = gameStudioId;
        model.CategoryId = categoryId;
        model.ReleaseDate = releaseDate;

        try
        {
            if (dvdCoverFile != null)
                model.CoverImageUrl = await _upload.SaveImageAsync(dvdCoverFile, "games");
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
            try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "games"); }
            catch (Exception ex) { ModelState.AddModelError("", ex.Message); await LoadReleaseDropdownsAsync(model); return View(model); }
        }

        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("Game updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Games.FirstOrDefaultAsync(g => g.GameId == id);
        if (item == null) return RedirectToAction(nameof(Index));

        var inOrders = await _context.OrderItems.AnyAsync(oi => oi.Product.GameId == id);
        if (inOrders)
        {
            SetMessage($"Cannot delete \"{item.Title}\" — it appears in customer orders.", success: false);
            return RedirectToAction(nameof(Index));
        }

        _context.Games.Remove(item);
        await _context.SaveChangesAsync();
        SetMessage("Game deleted.");
        return RedirectToAction(nameof(Index));
    }
}
