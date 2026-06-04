using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminArtistController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly FileUploadService _upload;
    private const int PageSize = 15;

    public AdminArtistController(ApplicationDbContext context, FileUploadService upload)
    {
        _context = context;
        _upload = upload;
    }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Artists.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(a => a.Name.Contains(search));
        return View(await PaginatedList<Artist>.CreateAsync(query.OrderBy(a => a.Name), page, PageSize));
    }

    public IActionResult Create() => View(new Artist());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Artist model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid) return View(model);
        try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "artists") ?? model.ImageUrl; }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(model); }
        _context.Artists.Add(model);
        await _context.SaveChangesAsync();
        SetMessage("Artist created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Artists.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Artist model, IFormFile? imageFile)
    {
        if (id != model.ArtistId) return NotFound();
        if (!ModelState.IsValid) return View(model);
        if (imageFile != null)
        {
            try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "artists"); }
            catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(model); }
        }
        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("Artist updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Artists.FindAsync(id);
        if (item == null) return RedirectToAction(nameof(Index));

        var albumCount = await _context.Albums.CountAsync(a => a.ArtistId == id);
        if (albumCount > 0)
        {
            SetMessage(
                $"Cannot delete \"{item.Name}\" — {albumCount} album(s) still linked. Delete or reassign those albums first.",
                success: false);
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.Artists.Remove(item);
            await _context.SaveChangesAsync();
            SetMessage("Artist deleted.");
        }
        catch (DbUpdateException)
        {
            SetMessage("Cannot delete this artist because other records still depend on it.", success: false);
        }

        return RedirectToAction(nameof(Index));
    }
}
