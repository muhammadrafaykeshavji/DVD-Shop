using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Models.ViewModels;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_project_DVD_Shop.Controllers;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;
    private const int PageSize = 12;

    public ProductController(ApplicationDbContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index(string? search, int? categoryId, int? artistId, int? albumId, MediaType? mediaType, string? media, int page = 1)
    {
        if (!mediaType.HasValue && !string.IsNullOrWhiteSpace(media)
            && Enum.TryParse<MediaType>(media, true, out var parsedMedia))
            mediaType = parsedMedia;

        var query = _context.Products.WithCatalogIncludes().Where(p => p.IsAvailable);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p =>
                (p.Album != null && p.Album.Title.Contains(search))
                || (p.Album != null && p.Album.Artist != null && p.Album.Artist.Name.Contains(search))
                || (p.Game != null && p.Game.Title.Contains(search))
                || (p.Game != null && p.Game.GameStudio != null && p.Game.GameStudio.Name.Contains(search))
                || (p.Movie != null && p.Movie.Title.Contains(search))
                || (p.Movie != null && p.Movie.FilmStudio != null && p.Movie.FilmStudio.Name.Contains(search))
                || (p.Album != null && p.Album.Category != null && p.Album.Category.Name.Contains(search))
                || (p.Game != null && p.Game.Category != null && p.Game.Category.Name.Contains(search))
                || (p.Movie != null && p.Movie.Category != null && p.Movie.Category.Name.Contains(search)));

        if (categoryId.HasValue)
            query = query.Where(p =>
                (p.Album != null && p.Album.CategoryId == categoryId)
                || (p.Game != null && p.Game.CategoryId == categoryId)
                || (p.Movie != null && p.Movie.CategoryId == categoryId));

        if (artistId.HasValue)
            query = query.Where(p => p.Album != null && p.Album.ArtistId == artistId);

        if (albumId.HasValue)
            query = query.Where(p => p.AlbumId == albumId);

        if (mediaType == MediaType.Music)
            query = query.Where(p => p.AlbumId != null);
        else if (mediaType == MediaType.Game)
            query = query.Where(p => p.GameId != null);
        else if (mediaType == MediaType.Movie)
            query = query.Where(p => p.MovieId != null);

        var categoryNames = CatalogCategories.GetGenreNames(mediaType);
        ViewBag.Categories = await _context.Categories
            .Where(c => categoryNames.Contains(c.Name))
            .OrderBy(c => c.Name)
            .ToListAsync();
        ViewBag.Artists = await _context.Artists.ToListAsync();
        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;
        ViewBag.ArtistId = artistId;
        ViewBag.MediaType = mediaType;

        var products = await query.ToListAsync();
        var ordered = products.OrderByDescending(ProductCatalogHelper.GetReleaseDate).ToList();
        ViewBag.TotalCount = ordered.Count;
        var list = PaginatedList<Product>.Create(ordered, page, PageSize);
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .WithCatalogIncludes()
            .Include(p => p.Reviews).ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var model = new ProductDetailViewModel
        {
            Product = product,
            Reviews = product.Reviews.OrderByDescending(r => r.CreatedDate).ToList(),
            AverageRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
            UserHasReviewed = userId != null && product.Reviews.Any(r => r.UserId == userId),
            Songs = product.AlbumId.HasValue
                ? await _context.Songs.Where(s => s.AlbumId == product.AlbumId).ToListAsync()
                : [],
            Games = product.GameId.HasValue
                ? await _context.Games.Where(g => g.GameId == product.GameId).ToListAsync()
                : [],
            Movies = product.MovieId.HasValue
                ? await _context.Movies.Where(m => m.MovieId == product.MovieId).ToListAsync()
                : []
        };
        return View(model);
    }

    [Authorize(Roles = "Member,Admin"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int id, int quantity = 1, string? returnUrl = null)
    {
        await _cartService.AddToCartAsync(id, quantity);
        TempData["Success"] = "Added to cart.";
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction(nameof(Details), new { id });
    }
}
