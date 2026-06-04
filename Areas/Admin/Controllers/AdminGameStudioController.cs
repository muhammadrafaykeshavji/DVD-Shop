using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminGameStudioController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly FileUploadService _upload;
    private const int PageSize = 15;

    public AdminGameStudioController(ApplicationDbContext context, FileUploadService upload)
    {
        _context = context;
        _upload = upload;
    }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.GameStudios.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(s => s.Name.Contains(search));
        return View(await PaginatedList<GameStudio>.CreateAsync(query.OrderBy(s => s.Name), page, PageSize));
    }

    public IActionResult Create() => View(new GameStudio());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GameStudio model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid) return View(model);
        try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "game-studios") ?? model.ImageUrl; }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(model); }
        _context.GameStudios.Add(model);
        await _context.SaveChangesAsync();
        SetMessage("Game studio created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.GameStudios.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GameStudio model, IFormFile? imageFile)
    {
        if (id != model.GameStudioId) return NotFound();
        if (!ModelState.IsValid) return View(model);
        if (imageFile != null)
        {
            try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "game-studios"); }
            catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(model); }
        }
        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("Game studio updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.GameStudios.FindAsync(id);
        if (item == null) return RedirectToAction(nameof(Index));

        var gameCount = await _context.Games.CountAsync(g => g.GameStudioId == id);
        if (gameCount > 0)
        {
            SetMessage($"Cannot delete \"{item.Name}\" — {gameCount} game(s) still linked.", success: false);
            return RedirectToAction(nameof(Index));
        }

        _context.GameStudios.Remove(item);
        await _context.SaveChangesAsync();
        SetMessage("Game studio deleted.");
        return RedirectToAction(nameof(Index));
    }
}
