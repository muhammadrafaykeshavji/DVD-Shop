using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Controllers;

[Authorize(Roles = "Member,Admin")]
public class ReviewController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewInputViewModel model)
    {
        if (!ModelState.IsValid) return RedirectToAction("Details", "Product", new { id = model.ProductId });

        var user = await _userManager.GetUserAsync(User);
        if (await _context.Reviews.AnyAsync(r => r.UserId == user!.Id && r.ProductId == model.ProductId))
        {
            TempData["Error"] = "You already reviewed this product.";
            return RedirectToAction("Details", "Product", new { id = model.ProductId });
        }

        _context.Reviews.Add(new Review
        {
            UserId = user!.Id,
            ProductId = model.ProductId,
            Rating = model.Rating,
            Comment = model.Comment,
            CreatedDate = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        TempData["Success"] = "Review submitted.";
        return RedirectToAction("Details", "Product", new { id = model.ProductId });
    }
}
