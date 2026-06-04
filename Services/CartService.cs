using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Services;

/// <summary>Session-based shopping cart with promotion support.</summary>
public class CartService
{
    private const string CartKey = "DVDShop_Cart";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;

    public CartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    private ISession Session => _httpContextAccessor.HttpContext!.Session;

    public List<CartItem> GetCart() => Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();

    public int GetItemCount() => GetCart().Sum(c => c.Quantity);

    public void SaveCart(List<CartItem> cart) => Session.SetObject(CartKey, cart);

    public async Task AddToCartAsync(int productId, int quantity = 1)
    {
        var product = await _context.Products
            .WithCatalogIncludes()
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsAvailable);
        if (product == null) return;

        var cart = GetCart();
        var existing = cart.FirstOrDefault(c => c.ProductId == productId);
        if (existing != null)
            existing.Quantity += quantity;
        else
            cart.Add(new CartItem
            {
                ProductId = product.ProductId,
                Title = ProductCatalogHelper.GetTitle(product),
                CoverImageUrl = ProductCatalogHelper.GetCoverUrl(product),
                UnitPrice = product.Price,
                Quantity = quantity
            });
        SaveCart(cart);
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(c => c.ProductId == productId);
        if (item == null) return;
        if (quantity <= 0) cart.Remove(item);
        else item.Quantity = quantity;
        SaveCart(cart);
    }

    public void RemoveFromCart(int productId)
    {
        var cart = GetCart();
        cart.RemoveAll(c => c.ProductId == productId);
        SaveCart(cart);
    }

    public void ClearCart() => Session.Remove(CartKey);

    public decimal GetSubtotal() => GetCart().Sum(c => c.LineTotal);

    public async Task<Promotion?> GetActivePromotionAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Promotions
            .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
            .OrderByDescending(p => p.DiscountPercent)
            .FirstOrDefaultAsync();
    }

    public async Task<decimal> GetDiscountAmountAsync()
    {
        var promo = await GetActivePromotionAsync();
        if (promo == null) return 0;
        return Math.Round(GetSubtotal() * promo.DiscountPercent / 100m, 2);
    }

    public async Task<decimal> GetTotalAsync() => GetSubtotal() - await GetDiscountAmountAsync();
}
