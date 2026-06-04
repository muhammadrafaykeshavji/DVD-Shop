using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Services;

/// <summary>Loads real analytics data for the admin dashboard.</summary>
public class DashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context) => _context = context;

    public async Task<AdminDashboardViewModel> GetAdminDashboardAsync()
    {
        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var year = today.Year;

        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Album)
            .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Game)
            .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Movie)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var totalSales = orders.Sum(o => o.TotalAmount);
        var totalOrders = orders.Count;

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
            .OrderByDescending(p => p.Revenue)
            .Take(5)
            .ToList();

        var monthlySales = Enumerable.Range(1, 12)
            .Select(m => new MonthlySalesViewModel
            {
                Label = new DateTime(year, m, 1).ToString("MMM"),
                Amount = orders
                    .Where(o => o.OrderDate.Year == year && o.OrderDate.Month == m)
                    .Sum(o => o.TotalAmount)
            })
            .ToList();

        var orderStatusCounts = Enum.GetValues<OrderStatus>()
            .Select(s => new OrderStatusCountViewModel
            {
                Status = s,
                Count = orders.Count(o => o.Status == s)
            })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .ToList();

        if (!orderStatusCounts.Any())
        {
            orderStatusCounts = Enum.GetValues<OrderStatus>()
                .Select(s => new OrderStatusCountViewModel { Status = s, Count = 0 })
                .Take(4)
                .ToList();
        }

        var lowStockProducts = await _context.Products
            .WithCatalogIncludes()
            .Where(p => p.IsAvailable && p.Stock <= 10)
            .OrderBy(p => p.Stock)
            .Take(8)
            .ToListAsync();
        var lowStock = lowStockProducts.Select(p => new LowStockProductViewModel
        {
            ProductId = p.ProductId,
            Title = ProductCatalogHelper.GetTitle(p),
            Stock = p.Stock
        }).ToList();

        var pendingFeedback = await _context.Feedbacks.CountAsync(f => f.AdminReply == null);
        var pendingReviews = await _context.Reviews.CountAsync();

        return new AdminDashboardViewModel
        {
            TotalSales = totalSales,
            SalesThisMonth = orders.Where(o => o.OrderDate >= startOfMonth).Sum(o => o.TotalAmount),
            AverageOrderValue = totalOrders > 0 ? Math.Round(totalSales / totalOrders, 2) : 0,
            TotalOrders = totalOrders,
            OrdersToday = orders.Count(o => o.OrderDate.Date == today),
            TotalUsers = await _context.Users.CountAsync(u => u.IsActive),
            NewUsersLast30Days = await _context.Users.CountAsync(u => u.CreatedDate >= today.AddDays(-30)),
            TotalProducts = await _context.Products.CountAsync(p => p.IsAvailable),
            LowStockCount = await _context.Products.CountAsync(p => p.IsAvailable && p.Stock <= 10),
            PendingFeedbackCount = pendingFeedback,
            PendingReviewsCount = pendingReviews,
            TopProducts = topProducts,
            RecentOrders = orders.Take(10).ToList(),
            LowStockProducts = lowStock,
            MonthlySales = monthlySales,
            OrderStatusCounts = orderStatusCounts
        };
    }
}
