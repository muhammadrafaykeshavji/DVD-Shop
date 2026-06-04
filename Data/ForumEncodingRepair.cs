using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Data;

/// <summary>Fixes broken dash characters in all forum posts on startup.</summary>
public static class ForumEncodingRepair
{
    public static async Task RepairAsync(ApplicationDbContext context)
    {
        var posts = await context.ForumPosts.ToListAsync();
        var changed = false;

        foreach (var post in posts)
        {
            var content = ForumTextHelper.ToAsciiDashes(post.Content);
            var title = ForumTextHelper.ToAsciiDashes(post.Title);

            if (content != post.Content)
            {
                post.Content = content;
                changed = true;
            }

            if (title != post.Title)
            {
                post.Title = title;
                changed = true;
            }
        }

        if (changed)
            await context.SaveChangesAsync();
    }
}
