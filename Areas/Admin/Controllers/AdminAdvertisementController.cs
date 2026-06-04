using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminAdvertisementController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly FileUploadService _upload;
    private const int PageSize = 15;

    public AdminAdvertisementController(ApplicationDbContext context, FileUploadService upload)
    {
        _context = context;
        _upload = upload;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        return View(await PaginatedList<Advertisement>.CreateAsync(
            _context.Advertisements.OrderBy(a => a.Position), page, PageSize));
    }

    public IActionResult Create() => View(new Advertisement { IsActive = true });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Advertisement model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid) return View(model);
        try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "banners") ?? model.ImageUrl; }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(model); }
        _context.Advertisements.Add(model);
        await _context.SaveChangesAsync();
        SetMessage("Advertisement created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Advertisements.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Advertisement model, IFormFile? imageFile)
    {
        if (id != model.AdId) return NotFound();
        if (!ModelState.IsValid) return View(model);
        if (imageFile != null)
        {
            try { model.ImageUrl = await _upload.SaveImageAsync(imageFile, "banners"); }
            catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(model); }
        }
        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("Advertisement updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Advertisements.FindAsync(id);
        if (item != null) { _context.Advertisements.Remove(item); await _context.SaveChangesAsync(); }
        SetMessage("Advertisement deleted.");
        return RedirectToAction(nameof(Index));
    }
}
