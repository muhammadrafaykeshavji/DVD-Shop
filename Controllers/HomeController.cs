using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace E_project_DVD_Shop.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .WithCatalogIncludes()
            .Where(p => p.IsAvailable)
            .ToListAsync();

        products = products.OrderByDescending(ProductCatalogHelper.GetReleaseDate).ToList();

        var now = DateTime.UtcNow;
        var publicNews = _context.News.PublicArticles();
        var banners = await _context.Advertisements
            .Where(a => a.IsActive && a.Position == AdPosition.HomeBanner)
            .ToListAsync();

        var spotlight = products.FirstOrDefault();
        var model = new HomeViewModel
        {
            FeaturedProducts = products.Take(4).ToList(),
            NewReleases = products.Take(8).ToList(),
            SpotlightProduct = spotlight,
            HeroImageUrl = banners.FirstOrDefault()?.ImageUrl
                ?? (spotlight != null ? ProductCatalogHelper.GetCoverUrl(spotlight) : null),
            Banners = banners,
            SidebarAds = await _context.Advertisements
                .Where(a => a.IsActive && a.Position == AdPosition.Sidebar)
                .ToListAsync(),
            Promotions = await _context.Promotions
                .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
                .ToListAsync(),
            LatestGamingNews = await publicNews
                .Where(n => n.Category == NewsCategories.Gaming)
                .OrderByDescending(n => n.CreatedDate)
                .FirstOrDefaultAsync(),
            LatestMusicNews = await publicNews
                .Where(n => n.Category == NewsCategories.Music)
                .OrderByDescending(n => n.CreatedDate)
                .FirstOrDefaultAsync(),
            LatestFilmNews = await publicNews
                .Where(n => n.Category == NewsCategories.Film)
                .OrderByDescending(n => n.CreatedDate)
                .FirstOrDefaultAsync()
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
