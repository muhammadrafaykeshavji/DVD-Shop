using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Controllers;

[Authorize(Roles = "Member,Admin")]
public class WishlistController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public WishlistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var items = await _context.Wishlists
            .Include(w => w.Product!.Album)
            .Include(w => w.Product!.Game)
            .Include(w => w.Product!.Movie)
            .Where(w => w.UserId == user!.Id)
            .OrderByDescending(w => w.AddedDate)
            .ToListAsync();
        return View(items);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, string? returnUrl = null)
    {
        var user = await _userManager.GetUserAsync(User);
        if (await _context.Wishlists.AnyAsync(w => w.UserId == user!.Id && w.ProductId == productId))
        {
            TempData["Error"] = "Already in wishlist.";
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        _context.Wishlists.Add(new Wishlist { UserId = user!.Id, ProductId = productId });
        await _context.SaveChangesAsync();
        TempData["Success"] = "Added to wishlist.";
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Details", "Product", new { id = productId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var item = await _context.Wishlists.FirstOrDefaultAsync(w => w.WishlistId == id && w.UserId == user!.Id);
        if (item != null)
        {
            _context.Wishlists.Remove(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
