using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminUserController : AdminBaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly FileUploadService _upload;
    private const int PageSize = 15;

    public AdminUserController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        FileUploadService upload)
    {
        _userManager = userManager;
        _context = context;
        _upload = upload;
    }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.UserName!.Contains(search) || u.Email!.Contains(search));
        return View(await PaginatedList<ApplicationUser>.CreateAsync(query.OrderBy(u => u.UserName), page, PageSize));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        ViewBag.Roles = await _userManager.GetRolesAsync(user);
        return View(user);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        string id,
        decimal balance,
        bool isActive,
        string role,
        string? profileImageUrl,
        IFormFile? avatarFile)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.Balance = balance;
        user.IsActive = isActive;

        try
        {
            if (avatarFile != null)
                user.ProfileImageUrl = await _upload.SaveImageAsync(avatarFile, "avatars");
            else if (!string.IsNullOrWhiteSpace(profileImageUrl))
                user.ProfileImageUrl = profileImageUrl.Trim();
        }
        catch (Exception ex)
        {
            ViewBag.Roles = await _userManager.GetRolesAsync(user);
            ModelState.AddModelError("", ex.Message);
            return View(user);
        }

        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, roles);
        if (!string.IsNullOrEmpty(role)) await _userManager.AddToRoleAsync(user, role);

        SetMessage("User updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Recover(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null) { user.IsActive = true; await _userManager.UpdateAsync(user); }
        SetMessage("User recovered.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null) { user.IsActive = false; await _userManager.UpdateAsync(user); }
        SetMessage("User deactivated.");
        return RedirectToAction(nameof(Index));
    }
}
