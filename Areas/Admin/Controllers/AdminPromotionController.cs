using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminPromotionController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 15;

    public AdminPromotionController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(int page = 1)
    {
        return View(await PaginatedList<Promotion>.CreateAsync(
            _context.Promotions.OrderByDescending(p => p.StartDate), page, PageSize));
    }

    public IActionResult Create() => View(new Promotion { IsActive = true, StartDate = DateTime.UtcNow.Date, EndDate = DateTime.UtcNow.Date.AddMonths(1) });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Promotion model)
    {
        if (!ModelState.IsValid) return View(model);
        _context.Promotions.Add(model);
        await _context.SaveChangesAsync();
        SetMessage("Promotion created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Promotions.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Promotion model)
    {
        if (id != model.PromotionId) return NotFound();
        if (!ModelState.IsValid) return View(model);

        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("Promotion updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Promotions.FindAsync(id);
        if (item != null)
        {
            _context.Promotions.Remove(item);
            await _context.SaveChangesAsync();
        }
        SetMessage("Promotion deleted.");
        return RedirectToAction(nameof(Index));
    }
}
