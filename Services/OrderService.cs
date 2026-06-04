using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Services;

/// <summary>Handles order placement, stock deduction, and balance updates.</summary>
public class OrderService
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;

    public OrderService(ApplicationDbContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public async Task<(bool Success, string Message, Order? Order)> PlaceOrderAsync(string userId)
    {
        var cart = _cartService.GetCart();
        if (!cart.Any()) return (false, "Your cart is empty.", null);

        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.IsActive) return (false, "Invalid user account.", null);

        var productIds = cart.Select(c => c.ProductId).ToList();
        var products = await _context.Products.Where(p => productIds.Contains(p.ProductId)).ToListAsync();

        foreach (var item in cart)
        {
            var product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
            if (product == null || !product.IsAvailable)
                return (false, $"Product '{item.Title}' is no longer available.", null);
            if (product.Stock < item.Quantity)
                return (false, $"Insufficient stock for '{item.Title}'.", null);
        }

        var subtotal = _cartService.GetSubtotal();
        var discount = await _cartService.GetDiscountAmountAsync();
        var total = subtotal - discount;

        if (user.Balance < total)
            return (false, $"Insufficient balance. Required: ${total:F2}, Available: ${user.Balance:F2}", null);

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = total,
            DiscountApplied = discount,
            Status = OrderStatus.Processing
        };

        foreach (var item in cart)
        {
            var product = products.First(p => p.ProductId == item.ProductId);
            product.Stock -= item.Quantity;
            order.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            });
        }

        user.Balance -= total;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        _cartService.ClearCart();

        return (true, "Order placed successfully!", order);
    }
}
