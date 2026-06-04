using System.ComponentModel.DataAnnotations;
using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.Entities;

namespace E_project_DVD_Shop.Models.ViewModels;

public class ReportViewModel
{
    public decimal TotalSales { get; set; }
    public int OrdersToday { get; set; }
    public int TotalOrders { get; set; }
    public int NewUsersLast30Days { get; set; }
    public int TotalUsers { get; set; }
    public List<TopProductViewModel> TopProducts { get; set; } = new();
}

public class TopProductViewModel
{
    public int ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int UnitsSold { get; set; }
    public decimal Revenue { get; set; }
}

public class DashboardViewModel
{
    public decimal Balance { get; set; }
    public int TotalOrders { get; set; }
    public int WishlistCount { get; set; }
    public List<Order> RecentOrders { get; set; } = new();
}

public class HomeViewModel
{
    public List<Product> FeaturedProducts { get; set; } = new();
    public List<Product> NewReleases { get; set; } = new();
    public Product? SpotlightProduct { get; set; }
    public string? HeroImageUrl { get; set; }
    public List<Advertisement> Banners { get; set; } = new();
    public List<Advertisement> SidebarAds { get; set; } = new();
    public List<Promotion> Promotions { get; set; } = new();
    public News? LatestGamingNews { get; set; }
    public News? LatestMusicNews { get; set; }
    public News? LatestFilmNews { get; set; }
}

public class ProductDetailViewModel
{
    public Product Product { get; set; } = null!;
    public List<Review> Reviews { get; set; } = new();
    public double AverageRating { get; set; }
    public bool UserHasReviewed { get; set; }
    public List<Song> Songs { get; set; } = new();
    public List<Game> Games { get; set; } = new();
    public List<Movie> Movies { get; set; } = new();
}

public class ReviewInputViewModel
{
    public int ProductId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }
}

public class FeedbackViewModel
{
    [Required, StringLength(2000)]
    public string Message { get; set; } = string.Empty;
}

public class AdminDashboardViewModel
{
    public decimal TotalSales { get; set; }
    public decimal SalesThisMonth { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int TotalOrders { get; set; }
    public int OrdersToday { get; set; }
    public int TotalUsers { get; set; }
    public int NewUsersLast30Days { get; set; }
    public int TotalProducts { get; set; }
    public int LowStockCount { get; set; }
    public int PendingFeedbackCount { get; set; }
    public int PendingReviewsCount { get; set; }
    public List<TopProductViewModel> TopProducts { get; set; } = new();
    public List<Order> RecentOrders { get; set; } = new();
    public List<LowStockProductViewModel> LowStockProducts { get; set; } = new();
    public List<MonthlySalesViewModel> MonthlySales { get; set; } = new();
    public List<OrderStatusCountViewModel> OrderStatusCounts { get; set; } = new();
}

public class MonthlySalesViewModel
{
    public string Label { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class OrderStatusCountViewModel
{
    public OrderStatus Status { get; set; }
    public int Count { get; set; }
}

public class LowStockProductViewModel
{
    public int ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Stock { get; set; }
}
