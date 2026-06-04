using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Helpers;
using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Controllers;

public class ForumController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private const int ChatMessageLimit = 250;
    private const int QnaPageSize = 20;

    public ForumController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index() => RedirectToAction(nameof(General));

    /// <summary>#general — live community chat (no threads).</summary>
    public async Task<IActionResult> General()
    {
        var messages = await _context.ForumPosts
            .Include(f => f.User)
            .Where(f => f.Channel == ForumChannel.General && f.ParentPostId == null)
            .OrderBy(f => f.CreatedDate)
            .Take(ChatMessageLimit)
            .ToListAsync();

        return View(messages);
    }

    [Authorize(Roles = "Member,Admin"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SendChat(ForumChatInput model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please enter a message.";
            return RedirectToAction(nameof(General));
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        _context.ForumPosts.Add(new ForumPost
        {
            Channel = ForumChannel.General,
            UserId = user.Id,
            Title = "-",
            Content = model.Content.Trim(),
            CreatedDate = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(General));
    }

    /// <summary>#qna — user questions; admin answers in thread.</summary>
    public async Task<IActionResult> Qna(string? search, int page = 1)
    {
        var query = _context.ForumPosts
            .Include(f => f.User)
            .Include(f => f.Replies)
            .Where(f => f.Channel == ForumChannel.Qna && f.ParentPostId == null);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(f => f.Title.Contains(search) || f.Content.Contains(search));

        ViewBag.Search = search;

        var list = await PaginatedList<ForumPost>.CreateAsync(
            query.OrderByDescending(f =>
                f.Replies.Any() ? f.Replies.Max(r => r.CreatedDate) : f.CreatedDate),
            page, QnaPageSize);

        return View(list);
    }

    public async Task<IActionResult> QnaDetails(int id)
    {
        var question = await _context.ForumPosts
            .Include(f => f.User)
            .Include(f => f.Replies).ThenInclude(r => r.User)
            .FirstOrDefaultAsync(f => f.PostId == id && f.Channel == ForumChannel.Qna && f.ParentPostId == null);

        if (question == null) return NotFound();
        return View(question);
    }

    [Authorize(Roles = "Member,Admin")]
    public IActionResult Ask() => View(new ForumQuestionInput());

    [Authorize(Roles = "Member,Admin"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Ask(ForumQuestionInput model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var post = new ForumPost
        {
            Channel = ForumChannel.Qna,
            UserId = user.Id,
            Title = model.Title.Trim(),
            Content = model.Content.Trim(),
            CreatedDate = DateTime.UtcNow
        };
        _context.ForumPosts.Add(post);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Your question was posted.";
        return RedirectToAction(nameof(QnaDetails), new { id = post.PostId });
    }

    [Authorize(Roles = "Admin"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Answer(ForumAnswerInput model)
    {
        var question = await _context.ForumPosts
            .FirstOrDefaultAsync(f => f.PostId == model.QuestionId && f.Channel == ForumChannel.Qna && f.ParentPostId == null);

        if (question == null) return NotFound();

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please enter an answer.";
            return RedirectToAction(nameof(QnaDetails), new { id = model.QuestionId });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        _context.ForumPosts.Add(new ForumPost
        {
            Channel = ForumChannel.Qna,
            UserId = user.Id,
            Title = $"Re: {question.Title}",
            Content = model.Content.Trim(),
            ParentPostId = question.PostId,
            CreatedDate = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        TempData["Success"] = "Answer posted.";
        return RedirectToAction(nameof(QnaDetails), new { id = model.QuestionId });
    }
}
