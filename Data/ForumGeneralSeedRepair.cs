using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Data;

/// <summary>Replaces #general chat with curated community sample messages.</summary>
public static class ForumGeneralSeedRepair
{
    public const string MarkerTitle = "Forum general sample chat v3";

    public static async Task EnsureAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        var alreadySeeded = await context.News.AnyAsync(n => n.Title == MarkerTitle);
        var needsEncodingFix = await context.ForumPosts
            .AnyAsync(p => p.Channel == ForumChannel.General &&
                           (p.Content.Contains('\u2014') || p.Content.Contains('\u2013') ||
                            p.Content.Contains("â€")));

        if (alreadySeeded && !needsEncodingFix)
            return;

        await DeleteGeneralPostsAsync(context);

        var userByName = new Dictionary<string, ApplicationUser>(StringComparer.OrdinalIgnoreCase);
        foreach (var (author, _) in RealWorldCatalogData.ForumGeneralChat)
        {
            if (userByName.ContainsKey(author))
                continue;
            var user = await userManager.FindByNameAsync(author);
            if (user != null)
                userByName[author] = user;
        }

        if (userByName.Count == 0)
            return;

        var rng = new Random(20260528);
        var baseTime = DateTime.UtcNow.AddDays(-6);
        var gapMinutes = 0;

        foreach (var (author, content) in RealWorldCatalogData.ForumGeneralChat)
        {
            if (!userByName.TryGetValue(author, out var user))
                continue;

            gapMinutes += rng.Next(25, 180);
            context.ForumPosts.Add(new ForumPost
            {
                Channel = ForumChannel.General,
                UserId = user.Id,
                Title = "-",
                Content = ForumTextHelper.ToAsciiDashes(content),
                CreatedDate = baseTime.AddMinutes(gapMinutes)
            });
        }

        await context.SaveChangesAsync();

        context.News.Add(new News
        {
            Title = MarkerTitle,
            Content = "Curated #general forum sample chat applied.",
            Category = "System",
            Type = NewsType.National,
            IsActive = false,
            CreatedDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }

    private static async Task DeleteGeneralPostsAsync(ApplicationDbContext context)
    {
        var posts = await context.ForumPosts
            .Where(p => p.Channel == ForumChannel.General)
            .ToListAsync();

        if (posts.Count == 0)
            return;

        context.ForumPosts.RemoveRange(posts);
        await context.SaveChangesAsync();
    }
}
