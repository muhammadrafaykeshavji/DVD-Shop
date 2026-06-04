using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Controllers;

public class NewsController : Controller
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 10;

    public NewsController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? category, int page = 1)
    {
        var query = _context.News.PublicArticles();

        if (!string.IsNullOrWhiteSpace(category))
        {
            if (!NewsCategories.IsIndustryCategory(category))
                return RedirectToAction(nameof(Index));

            query = query.Where(n => n.Category == category);
        }

        ViewBag.Category = category;
        ViewBag.CategoryLabel = NewsCategories.GetDisplayName(category);
        var list = await PaginatedList<Models.Entities.News>.CreateAsync(
            query.OrderByDescending(n => n.CreatedDate), page, PageSize);
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var news = await _context.News.PublicArticles()
            .FirstOrDefaultAsync(n => n.NewsId == id);
        if (news == null) return NotFound();
        return View(news);
    }
}
