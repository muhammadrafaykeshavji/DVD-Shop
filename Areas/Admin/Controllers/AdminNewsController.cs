using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminNewsController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly RssNewsSyncService _rssSync;
    private const int PageSize = 15;

    public AdminNewsController(ApplicationDbContext context, RssNewsSyncService rssSync)
    {
        _context = context;
        _rssSync = rssSync;
    }

    public async Task<IActionResult> Index(string? search, string? category, int page = 1, bool showInactive = false)
    {
        var query = _context.News
            .Where(n => n.Title != RealWorldCatalogData.CatalogMarker
                        && n.Title != RealWorldCatalogData.LegacyFakeMarker)
            .AsQueryable();
        if (!showInactive) query = query.Where(n => n.IsActive);
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(n => n.Title.Contains(search));
        if (!string.IsNullOrWhiteSpace(category) && NewsCategories.IsIndustryCategory(category))
            query = query.Where(n => n.Category == category);

        ViewBag.ShowInactive = showInactive;
        ViewBag.Category = category;
        return View(await PaginatedList<News>.CreateAsync(query.OrderByDescending(n => n.CreatedDate), page, PageSize));
    }

    public IActionResult Create() => View(new News { IsActive = true, CreatedDate = DateTime.UtcNow });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(News model)
    {
        if (!ModelState.IsValid) return View(model);
        model.CreatedDate = DateTime.UtcNow;
        _context.News.Add(model);
        await _context.SaveChangesAsync();
        SetMessage("News created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.News.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, News model)
    {
        if (id != model.NewsId) return NotFound();
        if (!ModelState.IsValid) return View(model);
        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("News updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.News.FindAsync(id);
        if (item != null) { item.IsActive = false; await _context.SaveChangesAsync(); }
        SetMessage("News deactivated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Recover(int id)
    {
        var item = await _context.News.FindAsync(id);
        if (item != null) { item.IsActive = true; await _context.SaveChangesAsync(); }
        SetMessage("News recovered.");
        return RedirectToAction(nameof(Index), new { showInactive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SyncRss()
    {
        var count = await _rssSync.SyncAllAsync();
        SetMessage(count > 0
            ? $"Synced {count} articles from IGN, Pitchfork, and Deadline."
            : "RSS sync finished (no new articles or feeds unreachable).");
        return RedirectToAction(nameof(Index));
    }
}
