using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminFeedbackController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 15;

    public AdminFeedbackController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Feedbacks.Include(f => f.User).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(f => f.Message.Contains(search));
        return View(await PaginatedList<Feedback>.CreateAsync(query.OrderByDescending(f => f.CreatedDate), page, PageSize));
    }

    public async Task<IActionResult> Reply(int id)
    {
        var item = await _context.Feedbacks.Include(f => f.User).FirstOrDefaultAsync(f => f.FeedbackId == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(int id, string adminReply)
    {
        var item = await _context.Feedbacks.FindAsync(id);
        if (item == null) return NotFound();
        item.AdminReply = adminReply;
        await _context.SaveChangesAsync();
        SetMessage("Reply saved.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Feedbacks.FindAsync(id);
        if (item != null) { _context.Feedbacks.Remove(item); await _context.SaveChangesAsync(); }
        SetMessage("Feedback deleted.");
        return RedirectToAction(nameof(Index));
    }
}
