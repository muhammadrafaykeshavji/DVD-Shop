using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminPermissionController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    private static readonly string[] Modules =
    {
        // Core catalog
        "Dashboard",
        "Category", "Artist", "Album", "Song",
        "MusicStudio", "GameStudio", "FilmStudio",
        "Game", "Movie", "Product",

        // Customers & orders
        "User", "Order",

        // Content & marketing
        "News", "Promotion", "Advertisement", "Review", "Feedback",

        // Operations & admin
        "Supplier", "Producer", "PurchasingInvoice", "Report", "Administrator"
    };

    public AdminPermissionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? adminId)
    {
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        ViewBag.Admins = admins;
        ViewBag.SelectedAdminId = adminId;
        ViewBag.Modules = Modules;

        if (string.IsNullOrEmpty(adminId)) return View(new List<Permission>());

        var permissions = await _context.Permissions.Where(p => p.AdminId == adminId).ToListAsync();
        return View(permissions);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(string adminId, string moduleName, bool canView, bool canAdd, bool canEdit, bool canDelete)
    {
        var perm = await _context.Permissions.FirstOrDefaultAsync(p => p.AdminId == adminId && p.ModuleName == moduleName);
        if (perm == null)
        {
            perm = new Permission { AdminId = adminId, ModuleName = moduleName };
            _context.Permissions.Add(perm);
        }
        perm.CanView = canView;
        perm.CanAdd = canAdd;
        perm.CanEdit = canEdit;
        perm.CanDelete = canDelete;
        await _context.SaveChangesAsync();
        SetMessage("Permissions saved.");
        return RedirectToAction(nameof(Index), new { adminId });
    }
}
