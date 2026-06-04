using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Services;

/// <summary>Generates admin sales and user reports.</summary>
public class ReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context) => _context = context;

    public async Task<ReportViewModel> GetReportAsync()
    {
        var today = DateTime.UtcNow.Date;
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p!.Album)
            .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Game)
            .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Movie)
            .ToListAsync();

        var topProducts = orders
            .SelectMany(o => o.OrderItems)
            .GroupBy(i => i.ProductId)
            .Select(g => new TopProductViewModel
            {
                ProductId = g.Key,
                Title = ProductCatalogHelper.GetTitle(g.First().Product),
                UnitsSold = g.Sum(i => i.Quantity),
                Revenue = g.Sum(i => i.UnitPrice * i.Quantity)
            })
            .OrderByDescending(p => p.UnitsSold)
            .Take(10)
            .ToList();

        var newUsers = await _context.Users
            .Where(u => u.CreatedDate >= today.AddDays(-30))
            .CountAsync();

        return new ReportViewModel
        {
            TotalSales = orders.Sum(o => o.TotalAmount),
            OrdersToday = orders.Count(o => o.OrderDate.Date == today),
            TotalOrders = orders.Count,
            TopProducts = topProducts,
            NewUsersLast30Days = newUsers,
            TotalUsers = await _context.Users.CountAsync()
        };
    }
}
