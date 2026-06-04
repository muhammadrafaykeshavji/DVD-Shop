using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Controllers;

[Authorize(Roles = "Member")]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;
    private readonly OrderService _orderService;
    private readonly UserManager<ApplicationUser> _userManager;
    private const int PageSize = 10;

    public OrderController(
        ApplicationDbContext context,
        CartService cartService,
        OrderService orderService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _cartService = cartService;
        _orderService = orderService;
        _userManager = userManager;
    }

    public IActionResult Cart()
    {
        ViewBag.Subtotal = _cartService.GetSubtotal();
        ViewBag.Discount = _cartService.GetDiscountAmountAsync().GetAwaiter().GetResult();
        ViewBag.Total = _cartService.GetTotalAsync().GetAwaiter().GetResult();
        return View(_cartService.GetCart());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UpdateCart(int productId, int quantity)
    {
        _cartService.UpdateQuantity(productId, quantity);
        return RedirectToAction(nameof(Cart));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult RemoveFromCart(int productId)
    {
        _cartService.RemoveFromCart(productId);
        return RedirectToAction(nameof(Cart));
    }

    public async Task<IActionResult> Checkout()
    {
        var user = await _userManager.GetUserAsync(User);
        ViewBag.Balance = user?.Balance ?? 0;
        ViewBag.Subtotal = _cartService.GetSubtotal();
        ViewBag.Discount = await _cartService.GetDiscountAmountAsync();
        ViewBag.Total = await _cartService.GetTotalAsync();
        return View(_cartService.GetCart());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var (success, message, _) = await _orderService.PlaceOrderAsync(user.Id);
        TempData[success ? "Success" : "Error"] = message;
        return success ? RedirectToAction(nameof(MyOrders)) : RedirectToAction(nameof(Checkout));
    }

    public async Task<IActionResult> MyOrders(int page = 1)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var list = await PaginatedList<Order>.CreateAsync(
            _context.Orders
                .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Album)
                .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Game)
                .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Movie)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate),
            page, PageSize);
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var order = await _context.Orders
            .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Album)
            .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Game)
            .Include(o => o.OrderItems).ThenInclude(i => i.Product!.Movie)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == user!.Id);
        if (order == null) return NotFound();
        return View(order);
    }
}
