using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminOrderController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 15;

    public AdminOrderController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Orders.Include(o => o.User).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(o => o.User.UserName!.Contains(search) || o.OrderId.ToString() == search);
        return View(await PaginatedList<Order>.CreateAsync(query.OrderByDescending(o => o.OrderDate), page, PageSize));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Album)
            .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Game)
            .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Movie)
            .FirstOrDefaultAsync(o => o.OrderId == id);
        return order == null ? NotFound() : View(order);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();
        order.Status = status;
        await _context.SaveChangesAsync();
        SetMessage("Order updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == id);
        if (order != null)
        {
            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
        SetMessage("Order deleted.");
        return RedirectToAction(nameof(Index));
    }
}
