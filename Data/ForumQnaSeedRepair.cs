using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Data;

/// <summary>Replaces #qna sample threads with curated question + admin answer pairs.</summary>
public static class ForumQnaSeedRepair
{
    public const string MarkerTitle = "Forum QnA sample threads v3";

    public static async Task EnsureAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        var alreadySeeded = await context.News.AnyAsync(n => n.Title == MarkerTitle);
        var needsEncodingFix = await context.ForumPosts
            .AnyAsync(p => p.Channel == ForumChannel.Qna &&
                           (p.Content.Contains('\u2014') || p.Content.Contains('\u2013') ||
                            p.Content.Contains("â€")));

        if (alreadySeeded && !needsEncodingFix)
            return;

        await DeleteQnaPostsAsync(context);

        var admin = await userManager.FindByNameAsync("admin");
        if (admin == null)
            return;

        var members = await userManager.GetUsersInRoleAsync("Member");
        var authors = members.Where(m => m.IsActive).OrderBy(m => m.UserName).ToList();
        if (authors.Count == 0)
            return;

        var rng = new Random(20260527);
        var daysAgo = new[] { 5, 5, 8, 12, 18, 22, 27, 30 };

        for (var i = 0; i < RealWorldCatalogData.ForumQnaThreads.Length; i++)
        {
            var (title, question, answer) = RealWorldCatalogData.ForumQnaThreads[i];
            var author = authors[i % authors.Count];
            var askedAt = DateTime.UtcNow.AddDays(-daysAgo[i % daysAgo.Length]).AddHours(-rng.Next(2, 10));

            var post = new ForumPost
            {
                Channel = ForumChannel.Qna,
                UserId = author.Id,
                Title = ForumTextHelper.ToAsciiDashes(title),
                Content = ForumTextHelper.ToAsciiDashes(question),
                CreatedDate = askedAt
            };
            context.ForumPosts.Add(post);
            await context.SaveChangesAsync();

            context.ForumPosts.Add(new ForumPost
            {
                Channel = ForumChannel.Qna,
                UserId = admin.Id,
                Title = ForumTextHelper.ToAsciiDashes($"Re: {title}"),
                Content = ForumTextHelper.ToAsciiDashes(answer),
                ParentPostId = post.PostId,
                CreatedDate = askedAt.AddHours(rng.Next(4, 36))
            });
        }

        await context.SaveChangesAsync();

        context.News.Add(new News
        {
            Title = MarkerTitle,
            Content = "Curated Q&A forum sample data applied.",
            Category = "System",
            Type = NewsType.National,
            IsActive = false,
            CreatedDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }

    private static async Task DeleteQnaPostsAsync(ApplicationDbContext context)
    {
        var parentIds = await context.ForumPosts
            .Where(p => p.Channel == ForumChannel.Qna && p.ParentPostId == null)
            .Select(p => p.PostId)
            .ToListAsync();

        if (parentIds.Count == 0)
            return;

        var replies = await context.ForumPosts
            .Where(p => p.ParentPostId != null && parentIds.Contains(p.ParentPostId.Value))
            .ToListAsync();
        context.ForumPosts.RemoveRange(replies);

        var questions = await context.ForumPosts
            .Where(p => parentIds.Contains(p.PostId))
            .ToListAsync();
        context.ForumPosts.RemoveRange(questions);

        await context.SaveChangesAsync();
    }
}
