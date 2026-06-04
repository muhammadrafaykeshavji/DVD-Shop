using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminAdministratorController : AdminBaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private const int PageSize = 15;

    public AdminAdministratorController(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public async Task<IActionResult> Index(int page = 1)
    {
        var adminList = new List<ApplicationUser>();
        foreach (var u in _userManager.Users)
            if (await _userManager.IsInRoleAsync(u, "Admin")) adminList.Add(u);

        var ordered = adminList.OrderBy(u => u.UserName).ToList();
        var count = ordered.Count;
        var items = ordered.Skip((page - 1) * PageSize).Take(PageSize).ToList();
        return View(new PaginatedList<ApplicationUser>(items, count, page, PageSize));
    }

    public IActionResult Create() => View(new RegisterViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }
        await _userManager.AddToRoleAsync(user, "Admin");
        SetMessage("Administrator created.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null) { user.IsActive = false; await _userManager.UpdateAsync(user); }
        SetMessage("Administrator deactivated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Recover(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null) { user.IsActive = true; await _userManager.UpdateAsync(user); }
        SetMessage("Administrator recovered.");
        return RedirectToAction(nameof(Index));
    }
}
