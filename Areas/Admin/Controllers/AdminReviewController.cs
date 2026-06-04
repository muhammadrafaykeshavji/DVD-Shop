using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminReviewController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 15;

    public AdminReviewController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product!.Album)
            .Include(r => r.Product!.Game)
            .Include(r => r.Product!.Movie)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(r => r.Comment!.Contains(search));
        return View(await PaginatedList<Review>.CreateAsync(query.OrderByDescending(r => r.CreatedDate), page, PageSize));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Reviews.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Review model)
    {
        if (id != model.ReviewId) return NotFound();
        if (!ModelState.IsValid) return View(model);
        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("Review updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Reviews.FindAsync(id);
        if (item != null) { _context.Reviews.Remove(item); await _context.SaveChangesAsync(); }
        SetMessage("Review deleted.");
        return RedirectToAction(nameof(Index));
    }
}
