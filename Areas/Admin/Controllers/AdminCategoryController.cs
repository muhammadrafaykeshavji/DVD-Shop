using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminCategoryController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 15;

    public AdminCategoryController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Categories.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.Contains(search));
        return View(await PaginatedList<Category>.CreateAsync(query.OrderBy(c => c.Name), page, PageSize));
    }

    public IActionResult Create() => View(new Category());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category model)
    {
        if (!ModelState.IsValid) return View(model);
        _context.Categories.Add(model);
        await _context.SaveChangesAsync();
        SetMessage("Category created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.Categories.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category model)
    {
        if (id != model.CategoryId) return NotFound();
        if (!ModelState.IsValid) return View(model);
        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("Category updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Categories.FindAsync(id);
        if (item == null) return RedirectToAction(nameof(Index));

        var albumCount = await _context.Albums.CountAsync(a => a.CategoryId == id);
        var gameCount = await _context.Games.CountAsync(g => g.CategoryId == id);
        var movieCount = await _context.Movies.CountAsync(m => m.CategoryId == id);
        var total = albumCount + gameCount + movieCount;
        if (total > 0)
        {
            SetMessage(
                $"Cannot delete \"{item.Name}\" — {albumCount} album(s), {gameCount} game(s), and {movieCount} movie(s) still use this category. Delete or reassign those titles first.",
                success: false);
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.Categories.Remove(item);
            await _context.SaveChangesAsync();
            SetMessage("Category deleted.");
        }
        catch (DbUpdateException)
        {
            SetMessage("Cannot delete this category because other records still depend on it.", success: false);
        }

        return RedirectToAction(nameof(Index));
    }
}
