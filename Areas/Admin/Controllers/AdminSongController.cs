using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminSongController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly FileUploadService _upload;
    private const int PageSize = 15;

    public AdminSongController(ApplicationDbContext context, FileUploadService upload)
    {
        _context = context;
        _upload = upload;
    }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Songs.Include(s => s.Album).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(s => s.Title.Contains(search));
        return View(await PaginatedList<Song>.CreateAsync(query.OrderBy(s => s.Title), page, PageSize));
    }

    public IActionResult Create()
    {
        ViewBag.AlbumId = new SelectList(
            _context.Albums.OrderBy(a => a.Title),
            "AlbumId", "Title");
        return View(new Song());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Song model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AlbumId = new SelectList(
            _context.Albums.OrderBy(a => a.Title),
            "AlbumId", "Title");
            return View(model);
        }
        try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "songs") ?? model.ImageUrl; }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); ViewBag.AlbumId = new SelectList(_context.Albums, "AlbumId", "Title"); return View(model); }
        _context.Songs.Add(model);
        await _context.SaveChangesAsync();
        SetMessage("Song created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Songs.FindAsync(id);
        if (item == null) return NotFound();
        ViewBag.AlbumId = new SelectList(_context.Albums, "AlbumId", "Title", item.AlbumId);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Song model, IFormFile? imageFile)
    {
        if (id != model.SongId) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.AlbumId = new SelectList(
            _context.Albums.OrderBy(a => a.Title),
            "AlbumId", "Title");
            return View(model);
        }
        if (imageFile != null)
        {
            try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "songs"); }
            catch (Exception ex) { ModelState.AddModelError("", ex.Message); ViewBag.AlbumId = new SelectList(_context.Albums, "AlbumId", "Title"); return View(model); }
        }
        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("Song updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Songs.FindAsync(id);
        if (item != null) { _context.Songs.Remove(item); await _context.SaveChangesAsync(); }
        SetMessage("Song deleted.");
        return RedirectToAction(nameof(Index));
    }
}
