using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminProducerController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 15;

    public AdminProducerController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, int page = 1, bool showInactive = false)
    {
        var query = _context.Producers.AsQueryable();
        if (!showInactive) query = query.Where(p => p.IsActive);
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(p => p.Name.Contains(search));
        ViewBag.ShowInactive = showInactive;
        return View(await PaginatedList<Producer>.CreateAsync(query.OrderBy(p => p.Name), page, PageSize));
    }

    public IActionResult Create() => View(new Producer { IsActive = true });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Producer model)
    {
        if (!ModelState.IsValid) return View(model);
        _context.Producers.Add(model);
        await _context.SaveChangesAsync();
        SetMessage("Producer created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Producers.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Producer model)
    {
        if (id != model.ProducerId) return NotFound();
        if (!ModelState.IsValid) return View(model);
        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("Producer updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Producers.FindAsync(id);
        if (item != null) { item.IsActive = false; await _context.SaveChangesAsync(); }
        SetMessage("Producer deactivated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Recover(int id)
    {
        var item = await _context.Producers.FindAsync(id);
        if (item != null) { item.IsActive = true; await _context.SaveChangesAsync(); }
        SetMessage("Producer recovered.");
        return RedirectToAction(nameof(Index), new { showInactive = true });
    }
}
