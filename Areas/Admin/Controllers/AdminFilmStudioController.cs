using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminFilmStudioController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly FileUploadService _upload;
    private const int PageSize = 15;

    public AdminFilmStudioController(ApplicationDbContext context, FileUploadService upload)
    {
        _context = context;
        _upload = upload;
    }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.FilmStudios.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(s => s.Name.Contains(search));
        return View(await PaginatedList<FilmStudio>.CreateAsync(query.OrderBy(s => s.Name), page, PageSize));
    }

    public IActionResult Create() => View(new FilmStudio());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FilmStudio model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid) return View(model);
        try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "film-studios") ?? model.ImageUrl; }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(model); }
        _context.FilmStudios.Add(model);
        await _context.SaveChangesAsync();
        SetMessage("Film studio created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.FilmStudios.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FilmStudio model, IFormFile? imageFile)
    {
        if (id != model.FilmStudioId) return NotFound();
        if (!ModelState.IsValid) return View(model);
        if (imageFile != null)
        {
            try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "film-studios"); }
            catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(model); }
        }
        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("Film studio updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.FilmStudios.FindAsync(id);
        if (item == null) return RedirectToAction(nameof(Index));

        var movieCount = await _context.Movies.CountAsync(m => m.FilmStudioId == id);
        if (movieCount > 0)
        {
            SetMessage($"Cannot delete \"{item.Name}\" — {movieCount} movie(s) still linked.", success: false);
            return RedirectToAction(nameof(Index));
        }

        _context.FilmStudios.Remove(item);
        await _context.SaveChangesAsync();
        SetMessage("Film studio deleted.");
        return RedirectToAction(nameof(Index));
    }
}
