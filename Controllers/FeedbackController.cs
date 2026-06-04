using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Controllers;

[Authorize(Roles = "Member")]
public class FeedbackController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public FeedbackController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var items = await _context.Feedbacks
            .Where(f => f.UserId == user!.Id)
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();
        return View(items);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FeedbackViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.GetUserAsync(User);
        _context.Feedbacks.Add(new Feedback { UserId = user!.Id, Message = model.Message });
        await _context.SaveChangesAsync();
        TempData["Success"] = "Feedback sent.";
        return RedirectToAction(nameof(Index));
    }
}
