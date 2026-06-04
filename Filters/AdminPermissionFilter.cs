using E_project_DVD_Shop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_project_DVD_Shop.Filters;

/// <summary>Enforces module-level permissions for admin area controllers only.</summary>
public class AdminPermissionFilter : IAsyncActionFilter
{
    private readonly ApplicationDbContext _context;

    public AdminPermissionFilter(ApplicationDbContext context) => _context = context;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.RouteData.Values["area"]?.ToString() != "Admin")
        {
            await next();
            return;
        }

        var user = context.HttpContext.User;
        if (!user.IsInRole("Admin"))
        {
            context.Result = new ForbidResult();
            return;
        }

        if (user.Identity?.Name == "admin")
        {
            await next();
            return;
        }

        var module = context.RouteData.Values["controller"]?.ToString()?.Replace("Admin", "") ?? "";
        var action = context.RouteData.Values["action"]?.ToString() ?? "";
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        var permission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.AdminId == userId && p.ModuleName == module);

        if (permission == null)
        {
            await next();
            return;
        }

        var allowed = action switch
        {
            "Index" or "Details" => permission.CanView,
            "Create" => permission.CanAdd,
            "Edit" => permission.CanEdit,
            "Delete" or "Recover" => permission.CanDelete,
            _ => permission.CanView
        };

        if (!allowed)
            context.Result = new ForbidResult();
        else
            await next();
    }
}
