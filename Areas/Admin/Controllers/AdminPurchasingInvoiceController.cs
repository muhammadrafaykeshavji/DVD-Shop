using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminPurchasingInvoiceController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 15;

    public AdminPurchasingInvoiceController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.PurchasingInvoices.Include(p => p.Supplier).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(p => p.Supplier.Name.Contains(search));
        return View(await PaginatedList<PurchasingInvoice>.CreateAsync(query.OrderByDescending(p => p.InvoiceDate), page, PageSize));
    }

    public IActionResult Create()
    {
        ViewBag.SupplierId = new SelectList(_context.Suppliers.Where(s => s.IsActive), "SupplierId", "Name");
        return View(new PurchasingInvoice { InvoiceDate = DateTime.UtcNow.Date });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PurchasingInvoice model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.SupplierId = new SelectList(_context.Suppliers.Where(s => s.IsActive), "SupplierId", "Name");
            return View(model);
        }
        _context.PurchasingInvoices.Add(model);
        await _context.SaveChangesAsync();
        SetMessage("Invoice created.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.PurchasingInvoices.FindAsync(id);
        if (item == null) return NotFound();
        ViewBag.SupplierId = new SelectList(_context.Suppliers, "SupplierId", "Name", item.SupplierId);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PurchasingInvoice model)
    {
        if (id != model.PurchasingInvoiceId) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.SupplierId = new SelectList(_context.Suppliers, "SupplierId", "Name", model.SupplierId);
            return View(model);
        }
        _context.Update(model);
        await _context.SaveChangesAsync();
        SetMessage("Invoice updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.PurchasingInvoices.FindAsync(id);
        if (item != null) { _context.PurchasingInvoices.Remove(item); await _context.SaveChangesAsync(); }
        SetMessage("Invoice deleted.");
        return RedirectToAction(nameof(Index));
    }
}
