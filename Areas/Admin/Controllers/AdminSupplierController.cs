using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminSupplierController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 15;

    public AdminSupplierController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, int page = 1, bool showInactive = false)
    {
        var query = _context.Suppliers.AsQueryable();
        if (!showInactive) query = query.Where(s => s.IsActive);
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(s => s.Name.Contains(search));
        ViewBag.ShowInactive = showInactive;
        return View(await PaginatedList<Supplier>.CreateAsync(query.OrderBy(s => s.Name), page, PageSize));
    }

    public IActionResult Create() => View(new Supplier { IsActive = true });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Supplier model)
    {
        if (!ModelState.IsValid) return View(model);
        _context.Suppliers.Add(model);
        await _context.SaveChangesAsync();
        SetMessage("Supplier created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Suppliers.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Supplier model)
    {
        if (id != model.SupplierId) return NotFound();
        if (!ModelState.IsValid) return View(model);
        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("Supplier updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Suppliers.FindAsync(id);
        if (item != null) { item.IsActive = false; await _context.SaveChangesAsync(); }
        SetMessage("Supplier deactivated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Recover(int id)
    {
        var item = await _context.Suppliers.FindAsync(id);
        if (item != null) { item.IsActive = true; await _context.SaveChangesAsync(); }
        SetMessage("Supplier recovered.");
        return RedirectToAction(nameof(Index), new { showInactive = true });
    }
}
